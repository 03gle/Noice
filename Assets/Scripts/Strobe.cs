using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strobe : MonoBehaviour
{
    private Light l;
    private float timer = 0;
    private bool isOn;

    [Range(0.01f, 0.5f)]
    public float delay = 0.2f;
    [Range(0.01f, 0.5f)]
    public float burnTime = 0.2f;
    [Range(0.0f, 100.0f)]
    public float intensity = 50.0f;

    void Start()
    {
        l = GetComponent<Light>();
        isOn = true;
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (isOn && timer > burnTime)
        {
            l.intensity = 0;
            timer = 0;
            isOn = false;
        }
        else if (!isOn && timer > delay)
        {
            l.intensity = intensity;
            timer = 0;
            isOn = true;
        }

    }
}
