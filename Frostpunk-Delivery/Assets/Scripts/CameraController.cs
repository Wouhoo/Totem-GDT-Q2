using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform car;

    [Header("3ed PoV")]
    public Vector3 offset = new Vector3(-5f, 5f, 0f);
    public float minimumHeight = 4f;
    public float tiltAngle = 20f;
    public float posLerpRate = 5f;
    public float rotSlerpRate = 5f;

    [Header("Screenshake effect")]
    [SerializeField] AnimationCurve intensityCurve;
    [SerializeField] float duration = 1f;
    private float initialIntensity = 0f;
    private float elapsedTime = 0f;
    private bool shaking = false;

    void LateUpdate()
    {
        //Vector3 posOffset = car.up * yPosOffset - car.forward * xPosOffset;
        //transform.position = car.position + posOffset;

        //Quaternion targetRot = Quaternion(car.)
        //Vector3 hTargetRot = new Vector3(0f, car.eulerAngles.y, 0f);

        //transform.eulerAngles = hTargetRot;
        //transform.rotation = Quaternion.Slerp(transform.rotation, car.ro, slerpRate * Time.deltaTime);
        // transform.position = Vector3.Lerp(transform.position, car.position + cameraOffset, slerpRate * Time.deltaTime);

        //transform.eulerAngles


        // Calculate the target position and apply minimum height
        Vector3 targetPosition = car.position + car.TransformDirection(offset);
        targetPosition.y = Mathf.Max(targetPosition.y, car.position.y + minimumHeight);

        // Smoothly interpolate the position
        transform.position = Vector3.Lerp(transform.position, targetPosition, posLerpRate * Time.deltaTime);

        // Compute the target forward direction (without twist)
        Vector3 targetForward = (car.position - transform.position).normalized;

        // Compute the target up direction, aligning with car's up vector
        Vector3 targetUp = Vector3.Lerp(Vector3.up, car.up, 0.5f).normalized;

        // Compute the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);

        // Add tilt offset to look slightly above the car
        targetRotation *= Quaternion.Euler(tiltAngle, 0, 0);

        // Smoothly interpolate the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSlerpRate * Time.deltaTime);

        // Add random offset each frame during screenshake effect
        if(shaking)
        {
            elapsedTime += Time.deltaTime;
            float curveIntensity = intensityCurve.Evaluate(elapsedTime / duration);
            transform.position = transform.position + Random.insideUnitSphere * curveIntensity * initialIntensity; // Move camera by a random offset scaled based on curve and initial impact intensity
        }
    }

    public void StartShaking(float crashIntensity)
    {
        if(!shaking) // Only start shaking if we're not already shaking
        {
            initialIntensity = crashIntensity;
            elapsedTime = 0;
            shaking = true;
            Invoke("StopShaking", duration);
        }
    }

    private void StopShaking()
    {
        shaking = false;
    }
}