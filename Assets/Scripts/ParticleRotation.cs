using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotation : MonoBehaviour
{
    public float speed = 1;

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, speed * 0.5f * Time.deltaTime, speed * Time.deltaTime));
    }
}
