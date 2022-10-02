using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTracker : MonoBehaviour
{
    public int radius = 10;

    public List<TrackingPoint> trackingPointsL;
    public List<TrackingPoint> trackingPointsR;
    public List<TrackingPoint> trackingPointsE;

    public Transform leftController;
    public Transform rightController;
    public Transform eyes;

    private Vector3 hitL;
    private Vector3 hitR;
    private Vector3 hitE;

    private int layerMask = 1 << 11;

    public int pointInfluenceLimit = 2;

    private FMOD.Studio.EventInstance eventInstanceL;
    public FMODUnity.EventReference eventRefL;

    private FMOD.Studio.EventInstance eventInstanceR;
    public FMODUnity.EventReference eventRefR;

    private FMOD.Studio.EventInstance eventInstanceE;
    public FMODUnity.EventReference eventRefE;


    public InputActionReference triggerL;
    public InputActionReference triggerR;

    public Easing.Type transitionType;

    private void Start()
    {
        eventInstanceL = FMODUnity.RuntimeManager.CreateInstance(eventRefL);
        eventInstanceL.start();

        eventInstanceR = FMODUnity.RuntimeManager.CreateInstance(eventRefR);
        eventInstanceR.start();

        eventInstanceE = FMODUnity.RuntimeManager.CreateInstance(eventRefE);
        eventInstanceE.start();
    }

    private void Update()
    {
        // Trigger Effects
        //float eL = triggerL.action.ReadValue<float>();
        //eventInstanceL.setParameterByName("L_Effect", eL);

        //float eR = triggerR.action.ReadValue<float>();
        //eventInstanceR.setParameterByName("R_Effect", eR);

        RaycastHit hit;

        // Left hand
        if (Physics.Raycast(leftController.position, leftController.forward, out hit, 100, layerMask))
        {
            hitL = hit.point;
            Debug.DrawRay(leftController.position, leftController.forward * hit.distance, Color.red);

            foreach (TrackingPoint tp in trackingPointsL)
            {
                tp.distFromHit = ArchDistance(hitL, tp.transform.position);
            }

            trackingPointsL.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));

            for (int i = 0; i < trackingPointsL.Count; i++)
            {
                if (i < pointInfluenceLimit)
                {
                    //Debug.Log("Left Hand " + i + ": " + Blend(trackingPointsLH, i));
                    var res = eventInstanceL.setParameterByName(trackingPointsL[i].parameterName, Easing.Ease(Blend(trackingPointsL, i), transitionType), false);
                    //Debug.Log(res);
                }
                else
                {
                    //Debug.Log("Left Hand " + i + ": 0");
                    eventInstanceL.setParameterByName(trackingPointsL[i].parameterName, 0, false);
                }
            }
            
        }

        // Right hand
        if (Physics.Raycast(rightController.position, rightController.forward, out hit, 100, layerMask))
        {
            hitR = hit.point;
            Debug.DrawRay(rightController.position, rightController.forward * hit.distance, Color.green);

            foreach (TrackingPoint tp in trackingPointsR)
            {
                tp.distFromHit = ArchDistance(hitR, tp.transform.position);
            }

            trackingPointsR.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));

            for (int i = 0; i < trackingPointsR.Count; i++)
            {
                if (i < pointInfluenceLimit)
                {
                    var res = eventInstanceR.setParameterByName(trackingPointsR[i].parameterName, Easing.Ease(Blend(trackingPointsL, i), transitionType), false);
                }
                else
                {
                    eventInstanceR.setParameterByName(trackingPointsR[i].parameterName, 0, false);
                }
            }
        }


        // Eyes
        if (Physics.Raycast(eyes.position, eyes.forward, out hit, 100, layerMask))
        {
            hitE = hit.point;
            Debug.DrawRay(eyes.position, eyes.forward * hit.distance, Color.blue);

            foreach (TrackingPoint tp in trackingPointsE)
            {
                tp.distFromHit = ArchDistance(hitE, tp.transform.position);
            }

            trackingPointsE.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));

            for (int i = 0; i < trackingPointsE.Count; i++)
            {
                if (i < pointInfluenceLimit)
                {
                    var res = eventInstanceE.setParameterByName(trackingPointsE[i].parameterName, Easing.Ease(Blend(trackingPointsL, i), transitionType), false);
                }
                else
                {
                    eventInstanceR.setParameterByName(trackingPointsE[i].parameterName, 0, false);
                }
            }
        }

    }

    public float Blend(List<TrackingPoint> points, int index)
    {
        float total = 0;
        for (int i = 0; i < pointInfluenceLimit; i++) 
            total += points[i].distFromHit;

        float val = 1 - (points[index].distFromHit / total);

        return val;
    }

    public float ArchDistance(Vector3 p1, Vector3 p2)
    {
        return 2 * radius * Mathf.Asin(Vector3.Distance(p1, p2) / (2 * radius));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitL, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitR, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hitE, 0.2f);
    }

}
