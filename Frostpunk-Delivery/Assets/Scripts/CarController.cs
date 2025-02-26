using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting;
using Unity.Burst.Intrinsics;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;




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
    public float tractionFactor = 0; // how much we "push" a friction to the standard friction amount (road + no ice) [0=normal, 1="road + no ice"]

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

    [Header("Friction modifiers based on road material")]
    private LayerMask roadLayer;
    private LayerMask dirtRoadLayer;
    private LayerMask iceLayer;
    [SerializeField] float roadFriction = 1.0f;
    [SerializeField] float dirtFriction = 0.8f;
    [SerializeField] float offroadFriction = 0.6f;
    [Tooltip("Base friction coefficient will be multiplied by this value if on ice")][SerializeField] float iceModifier = 0.3f;
    private Vector3 checkOffset = new Vector3(0, -2.0f, 0); // Offset & radius of the sphere centered on transform.position that checks if the player is on road/ice
    private float checkRadius = 1.0f;

    [Header("Other")]
    public float landFrictionCoef = 1f;
    public bool canDrive = true;

    [Header("Audio")]
    [SerializeField] private AudioSource iceScrapingAudioSource;
    [SerializeField] private float minSpeedForIceSound = 2f;

    private PlayerFuel playerFuel;
    WheelController[] wheels;
    private Rigidbody rigidBody;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerFuel = GetComponent<PlayerFuel>();
        carHealth = GetComponent<CarHealth>();

        // Get layermasks
        roadLayer = 1 << LayerMask.NameToLayer("Road");
        dirtRoadLayer = 1 << LayerMask.NameToLayer("DirtRoad");
        iceLayer = 1 << LayerMask.NameToLayer("Ice");

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelController>();

        if (iceScrapingAudioSource != null)
        {
            iceScrapingAudioSource.Stop();
            iceScrapingAudioSource.loop = true;
            iceScrapingAudioSource.playOnAwake = false;
        }

        ResetFriction();
    }

    public void Set_MaxSpeed(float amount)
    {
        maxSpeed = amount;
    }


    void FixedUpdate()
    {
        speedText.text = string.Format("Speed: {0:#.0}", rigidBody.velocity.magnitude);

        CheckFriction(); // Set car friction based on material you're driving on

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


    private int _priorFirctionState = 01; // used for optimization (_X : 1=road, 2=dirt, 3=offroad) (X_ : 0=normal, 1=ice)
    private int _firctionState = 01; // used for optimization (_X : 1=road, 2=dirt, 3=offroad) (X_ : 0=normal, 1=ice)
    private float friction = 1.0f; // save time not initiating
    private void CheckFriction()
    {
        // Find the "best quality" road that the car is on
        // Note: could maybe be optimized by only getting a list of overlapping colliders once and checking if any of them are tagged road or dirt road.

        if (Physics.CheckSphere(transform.position + checkOffset, checkRadius, roadLayer, QueryTriggerInteraction.Collide))
        {
            friction = roadFriction;
            _firctionState = 1;
            //Debug.Log("ON ROAD");
        }
        else if (Physics.CheckSphere(transform.position + checkOffset, checkRadius, dirtRoadLayer, QueryTriggerInteraction.Collide))
        {
            friction = dirtFriction;
            _firctionState = 2;
            //Debug.Log("ON DIRT");
        }
        else
        {
            friction = offroadFriction;
            _firctionState = 3;
            //Debug.Log("OFFROAD");
        }

        // Reduce friction if on ice
        if (Physics.CheckSphere(transform.position + checkOffset, checkRadius, iceLayer, QueryTriggerInteraction.Collide))
        {
            friction *= iceModifier;
            _firctionState += 10;
            //Debug.Log("ON ICE");
            if (!iceScrapingAudioSource.isPlaying && rigidBody.velocity.magnitude > minSpeedForIceSound)
            {
                iceScrapingAudioSource.loop = true;
                iceScrapingAudioSource.Play();
            }
            else if (iceScrapingAudioSource.isPlaying && rigidBody.velocity.magnitude <= minSpeedForIceSound)
            {
                iceScrapingAudioSource.Stop();
            }
        }
        else
        {
            if (iceScrapingAudioSource.isPlaying)
            {
                iceScrapingAudioSource.Stop();
            }
            if (_firctionState == _priorFirctionState)
                return;
            else
            {
                _priorFirctionState = _firctionState;
                SetFriction(friction);
            }
        }
    }

    public void Set_TractionFactor(float amount)
    {
        tractionFactor = Mathf.Clamp(amount, 0, 1);
    }

    public void SetFriction(float frictionCoef)
    {
        frictionCoef = tractionFactor * roadFriction + (1 - tractionFactor) * frictionCoef; // "push the friction to the road amount based on "tractionFactor"

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
