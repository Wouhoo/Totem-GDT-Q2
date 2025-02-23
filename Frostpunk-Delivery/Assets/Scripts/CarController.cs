using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting;
using Unity.Burst.Intrinsics;




public class CarController : MonoBehaviour
{
    // NOTICE : WE ASSUME CONSTANT MASS OF 1 !

    [SerializeField] TextMeshProUGUI speedText;
    private CarHealth carHealth;

    [Header("Car Specs")]
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;
    public float fuelEfficiency = 4000f;     // Amount of motor toruqe of 1 wheel for 1 second   needed 1 unit of fuel (ie. fuelEfficiency = - tor * dt / dF )

    [Header("Wheel friction curve modifiers")]
    [SerializeField] float forwardExtremumSlip = 0.4f;
    [SerializeField] float forwardExtremumValue = 1.0f;
    [SerializeField] float forwardAsymptoteSlip = 0.8f;
    [SerializeField] float forwardAsymptoteValue = 0.5f;
    [SerializeField] float sidewaysExtremumSlip = 0.2f;
    [SerializeField] float sidewaysExtremumValue = 1.0f;
    [SerializeField] float sidewaysAsymptoteSlip = 0.5f;
    [SerializeField] float sidewaysAsymptoteValue = 0.75f;
    [SerializeField] float baseWheelDampeningRate = 100f;
    private float wheelDampeningRate; // Set to 0.25 when driving, else to the current rate


    [Header("Other")]
    public float landFrictionCoef = 1f;
    public bool canDrive = true;

    private PlayerFuel playerFuel;
    WheelController[] wheels;
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerFuel = GetComponent<PlayerFuel>();
        carHealth = GetComponent<CarHealth>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelController>();

        ResetFriction();
    }



    void FixedUpdate()
    {
        speedText.text = string.Format("Speed: {0:#.00}", rigidBody.velocity.magnitude);

        // get damage based on aceleration change
        carHealth.NewVelocity(rigidBody.velocity, Time.deltaTime);

        float hInput;
        float vInput;
        if (!canDrive) // Input = 0 in case car is disabled
        {
            hInput = 0f;
            vInput = 0f;
        } 
        else 
        {
            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
        }

        // Calculate current speed in relation to the forward direction of the car (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

        // Calculate how close the car is to top speed as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // , forwardSpeed)

        // Calculate how much torque is available (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // Calculate how much to steer (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction as the car's velocity
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            // Adjust wheel dampening rate
            if (Mathf.Abs(vInput) > 0.1f) // We are trying to move
                wheel.WheelCollider.wheelDampingRate = Mathf.Lerp(wheelDampeningRate, 0.25f, speedFactor * 5f); // * 6 so that we reach min dampening already at only 20% top speed
            else // We are letting go of the gas and slightly breaking
                wheel.WheelCollider.wheelDampingRate = Mathf.Lerp(wheelDampeningRate, 0.25f, speedFactor * 0.8f);  // smooth step instead of lerp here maybe?

            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;

            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                    // Update fuel useage
                    playerFuel.AddFuel(-MathF.Abs(vInput * currentMotorTorque) * Time.deltaTime / fuelEfficiency);
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    public void SetFriction(float frictionCoef)
    {
        Debug.Log(frictionCoef);

        WheelFrictionCurve wheelFrictionCurve_forward = new WheelFrictionCurve();
        wheelFrictionCurve_forward.extremumSlip = forwardExtremumSlip;
        wheelFrictionCurve_forward.extremumValue = forwardExtremumValue;
        wheelFrictionCurve_forward.asymptoteSlip = forwardAsymptoteSlip;
        wheelFrictionCurve_forward.asymptoteValue = forwardAsymptoteValue;
        wheelFrictionCurve_forward.stiffness = frictionCoef;

        WheelFrictionCurve wheelFrictionCurve_sideways = new WheelFrictionCurve();
        wheelFrictionCurve_sideways.extremumSlip = sidewaysExtremumSlip;
        wheelFrictionCurve_sideways.extremumValue = sidewaysExtremumValue;
        wheelFrictionCurve_sideways.asymptoteSlip = sidewaysAsymptoteSlip;
        wheelFrictionCurve_sideways.asymptoteValue = sidewaysAsymptoteValue;
        wheelFrictionCurve_sideways.stiffness = frictionCoef;

        wheelDampeningRate = baseWheelDampeningRate * frictionCoef;

        foreach (var wheel in wheels)
        {
            wheel.WheelCollider.forwardFriction = wheelFrictionCurve_forward;
            wheel.WheelCollider.sidewaysFriction = wheelFrictionCurve_sideways;
        }
    }

    public void ResetFriction()
    {
        SetFriction(landFrictionCoef);
    }
}
