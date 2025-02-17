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
    private UpgradeManager upgradeManager;
    private PlayerState playerState;

    [SerializeField] TextMeshProUGUI fuelText;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerState = GetComponent<PlayerState>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
        fuelLevel = maxFuelLevel;  // Start with a full tank of fuel
    }

    public void SetFuel(float fuel)
    {
        // Set fuel level and update fuel text (and later, hopefully, also update fuel meters on the UI and/or the vehicle itself)
        if (fuel > maxFuelLevel) { fuel = maxFuelLevel; }
        fuelLevel = fuel;
        UpdateFuelText();

        playerState.NoFuel(fuelLevel <= 0.01f);
    }

    public float GetFuelLevel()
    {
        // tells us how much fuel we have (for other scripts to use if necessary)
        return fuelLevel;
    }

    public void AddFuel(float fuelToAdd)
    {
        // Instead of setting the fuel level, you can also add/remove by calling this function
        SetFuel(fuelLevel + fuelToAdd);
    }

    public void SetCapacity(float capacity)
    {
        maxFuelLevel = capacity;
        // check max fuel constraints
        fuelLevel = Mathf.Min(fuelLevel, maxFuelLevel);

        UpdateFuelText();
    }

    private void UpdateFuelText()
    {
        fuelText.text = string.Format("Fuel: {0:#.0} / {1:#.0}", fuelLevel, maxFuelLevel);
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
            {
                PickupFuel();
            }
            else if (other.tag == "Delivery Point")
            {
                DeliveryPoint deliveryPointScript = other.GetComponent<DeliveryPoint>();
                if (deliveryPointScript.questActive) // Only deliver if delivery point actually has an active quest
                {
                    DeliverFuel(deliveryPointScript);
                }
            }
            else if (other.tag == "Upgrade Point")
            {
                upgradeManager.OpenUpgradeMenu();
                playerRb.velocity = new Vector3(0, 0, 0); // Reset player's velocity if they go into the shop to upgrade
            }
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

    void DeliverFuel(DeliveryPoint deliveryPoint)
    {
        float fuelToDeliver = deliveryPoint.quest.fuelToDeliver;

        if (fuelLevel > fuelToDeliver)
        {
            AddFuel(-fuelToDeliver);
            deliveryPoint.CompleteQuest();
            Debug.Log("Succesfully delivered fuel!");
        }
        else
        {
            Debug.Log("Not enough fuel to deliver!");
        }
    }
}
