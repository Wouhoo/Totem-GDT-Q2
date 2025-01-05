using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    // Script for making an object rotate around an axis.
    // Used in the main menu for something cool :)
    float rotationSpeed = 5.0f;
    Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
