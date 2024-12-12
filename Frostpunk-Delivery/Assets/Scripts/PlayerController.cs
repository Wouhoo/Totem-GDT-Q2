using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Very basic movement for now, since I needed to be able to move in order to start testing the pickup/delivery system
    // @Tim: please rework this with an actually good controller :) and also make sure the camera follows the player

    private float moveSpeed = 15f;
    private float turnSpeed = 90f;
    private float fuelConsumption = 0.5f; // Rate of fuel consumption (currently has no physical meaning, tweak it until it feels good)
                                          // Currently dependent only on player speed; should somehow be dependent on acceleration as well (idk how cars work tho)

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody playerRb;
    private PlayerFuel playerFuel;
    [SerializeField] TextMeshProUGUI speedText;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerFuel = GetComponent<PlayerFuel>();
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        playerRb.AddRelativeForce(Vector3.forward * moveSpeed * verticalInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        speedText.text = string.Format("Speed: {0:#.00}", playerRb.velocity.magnitude);

        playerFuel.AddFuel(-fuelConsumption * Time.deltaTime * playerRb.velocity.magnitude);
    }
}
