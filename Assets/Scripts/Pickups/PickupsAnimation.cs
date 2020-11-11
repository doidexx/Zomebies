using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupsAnimation : MonoBehaviour
{
    public float rotationSpeed = 20;

    float angle = 0;
    void Update()
    {
        angle = rotationSpeed * Time.deltaTime;
        transform.RotateAround(transform.position, Vector3.up, angle);
    }
}
