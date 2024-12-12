using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFuel : MonoBehaviour
{
    // This script regulates everything related to the player's fuel, including:
    // - Picking up fuel at pickup points
    // - Delivering fuel in exchange for points/money at delivery points
    // - Consuming fuel over time for driving around (fuel is removed by the PlayerController script)
    // - (Possibly) using fuel for special tools, like a flamethrower for thawing the road/removing obstacles

    private float fuelLevel; // set this with the public SetFuel or AddFuel methods

    private float maxFuelLevel = 50f;
    private float maxSpeedForDelivery = 2.0f; // max speed the player is allowed to have at a pickup/delivery point to pick up/deliver fuel
    private bool atPoint; // true if a player is at a pickup/delivery point *but has not yet done the thing there* (because they're going too fast)

    private Rigidbody playerRb;
    private GameManager gameManager;
    [SerializeField] TextMeshProUGUI fuelText;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void SetFuel(float fuel)
    {
        // Set fuel level and update fuel text (and later, hopefully, also update fuel meters on the UI and/or the vehicle itself)
        if(fuel > maxFuelLevel) { fuel = maxFuelLevel; }
        fuelLevel = fuel;
        fuelText.text = string.Format("Fuel: {0:#.0}", fuelLevel);
    }

    public void AddFuel(float fuelToAdd)
    {
        // Instead of setting the fuel level, you can also add/remove by calling this function
        SetFuel(fuelLevel + fuelToAdd);
    }

    private void OnTriggerEnter(Collider other)
    {
        atPoint = true;
    }

    private void OnTriggerStay(Collider other)
    {
        // Checks if the player is near a pickup or delivery point.
        // The player must be (nearly) standing still to pick up or deliver fuel; they won't get the benefit if they just speed through.
        if (atPoint && playerRb.velocity.magnitude < maxSpeedForDelivery)
        {
            if (other.tag == "Pickup Point")
                PickupFuel();
            else if (other.tag == "Delivery Point")
                DeliverFuel(30f, 10);
            atPoint = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        atPoint = false;
    }

    void PickupFuel()
    {
        // At least for now, picking up fuel at a pickup point completely fills your fuel level to the max
        SetFuel(maxFuelLevel);
        Debug.Log("Picked up fuel!");
    }

    void DeliverFuel(float fuelToDeliver, int pointsToGive)
    {
        // TODO
        // For now (for testing purposes) this just always removes 30 fuel and adds 10 points
        if(fuelLevel > fuelToDeliver)
        {
            AddFuel(-fuelToDeliver);
            gameManager.UpdateScore(pointsToGive);
            Debug.Log("Succesfully delivered fuel!");
        }
        else
        {
            Debug.Log("Not enough fuel to deliver!");
        }
    }
}
