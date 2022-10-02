using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using UnityEngine;

public class InputTracker : MonoBehaviour
{
    public int radius = 10;

    public List<TrackingPoint> trackingPointsLH;
    public List<TrackingPoint> trackingPointsRH;
    public List<TrackingPoint> trackingPointsE;

    public Transform leftController;
    public Transform rightController;
    public Transform eyes;

    private Vector3 hitLH;
    private Vector3 hitRH;
    private Vector3 hitE;

    public int pointInfluenceLimit = 2;

    private FMOD.Studio.EventInstance eventInstanceLH;
    public FMODUnity.EventReference eventRefLH;


    private void Start()
    {
        eventInstanceLH = FMODUnity.RuntimeManager.CreateInstance(eventRefLH);
    }

    private void Update()
    {
        int layerMask = 1 << 11;
        RaycastHit hit;

        // Left hand
        if (Physics.Raycast(leftController.position, leftController.forward, out hit, 100, layerMask))
        {
            hitLH = hit.point;
            Debug.DrawRay(leftController.position, leftController.forward * hit.distance, Color.red);

            foreach (TrackingPoint tp in trackingPointsLH)
            {
                tp.distFromHit = ArchDistance(hitLH, tp.transform.position);
            }

            trackingPointsLH.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));

            for (int i = 0; i < trackingPointsLH.Count; i++)
            {
                if (i < pointInfluenceLimit)
                {
                    Debug.Log("Left Hand " + i + ": " + Blend(trackingPointsLH, i));
                    eventInstanceLH.setParameterByName(trackingPointsLH[i].parameterName, Blend(trackingPointsLH, i));
                }
                else
                {
                    Debug.Log("Left Hand " + i + ": 0");
                    eventInstanceLH.setParameterByName(trackingPointsLH[i].parameterName, 0);
                }
            }
            
        }

        //// Right hand
        //if (Physics.Raycast(rightController.position, rightController.forward, out hit, 100, layerMask))
        //{
        //    hitRH = hit.point;
        //    Debug.DrawRay(rightController.position, rightController.forward * hit.distance, Color.green);

        //    foreach (TrackingPoint tp in trackingPointsRH)
        //    {
        //        tp.distFromHit = ArchDistance(hitRH, tp.transform.position);
        //    }

        //    trackingPointsRH.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));
        //}
        

        //// Eyes
        //if (Physics.Raycast(eyes.position, eyes.forward, out hit, 100, layerMask))
        //{
        //    hitE = hit.point;
        //    Debug.DrawRay(eyes.position, eyes.forward * hit.distance, Color.blue);

        //    foreach (TrackingPoint tp in trackingPointsE)
        //    {
        //        tp.distFromHit = ArchDistance(hitE, tp.transform.position);
        //    }

        //    trackingPointsE.Sort((p1, p2) => p1.distFromHit.CompareTo(p2.distFromHit));
        //}

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
        Gizmos.DrawSphere(hitLH, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitRH, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hitE, 0.2f);
    }

}
