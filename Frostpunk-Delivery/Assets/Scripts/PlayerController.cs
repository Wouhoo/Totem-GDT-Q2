using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Assertions.Must;

public class PlayerController : MonoBehaviour
{
    // NOTICE : WE ASSUME CONSTANT MASS OF 1 !

    [SerializeField] TextMeshProUGUI speedText;

    [Header("Car Specs")]
    public float maxAcceleration = 10f;     // m / s^2
    public float maxSpeed = 7.5f;            // m /s           --- top speed on land (not on ice!)
    public float maxTurnTorque = 10f;       // rad / s^2      --- for turning
    public float maxTurnSpeed = 7.5f;        // rad / s
    public float fuelEfficiency = 0.5f;     // Acceleration needed to lose 1 unit of fuel in 1 second (ie. fuelEfficiency = - acc * dt / dF )
    public PlayerFuel playerFuel;

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody playerRb;
    private float landFrictionCoef = 0.8f;
    public float friction;

    private void Start()
    {
        ResetFriction();

        playerRb = GetComponent<Rigidbody>();
        playerFuel = GetComponent<PlayerFuel>();

        // drags are responsible for setting the max speeds
        playerRb.drag = (maxAcceleration - landFrictionCoef * 9.81f) / maxSpeed;
        playerRb.angularDrag = maxTurnTorque / maxTurnSpeed; // im not doing moment of inertia, no rot fric
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        float inputAcceleration = maxAcceleration * verticalInput;
        playerRb.AddRelativeForce(Vector3.forward * inputAcceleration);     // apply driving force
        if (playerRb.velocity.magnitude > 0.1f)                             // apply friction
            playerRb.AddForce(-playerRb.velocity.normalized * friction);
        else
            playerRb.velocity.Set(0, 0, 0);

        float inputTorque = maxTurnTorque * horizontalInput;
        playerRb.AddRelativeTorque(Vector3.up * inputTorque);               // apply driving torque

        speedText.text = string.Format("Speed: {0:#.00}", playerRb.velocity.magnitude);

        playerFuel.AddFuel(-MathF.Abs(inputAcceleration) * Time.deltaTime / fuelEfficiency);
    }

    public void SetFriction(float newFrictionCoef)
    {
        friction = newFrictionCoef * 9.81f;
    }

    public void ResetFriction()
    {
        friction = landFrictionCoef * 9.81f;
    }
}
