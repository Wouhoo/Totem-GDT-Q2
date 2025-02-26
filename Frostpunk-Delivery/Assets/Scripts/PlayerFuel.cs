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
    [SerializeField] float maxSpeedForDelivery = 10.0f; // max speed the player is allowed to have at a pickup/delivery point to pick up/deliver fuel
    private bool atPoint; // true if a player is at a pickup/delivery point *but has not yet done the thing there* (because they're going too fast)
    private bool sceneJustLoaded = true;
    private Rigidbody playerRb;
    private UpgradeManager upgradeManager;
    private PlayerState playerState;

    [SerializeField] TextMeshProUGUI currentFuelText;
    [SerializeField] TextMeshProUGUI notEnoughFuelText;
    [SerializeField] float notEnoughFuelTextDuration = 3.0f;

    private ParticleSystem upgradeParticles; // Particles that are played when the player upgrades or refuels their car
                                             // (can probably use the same effect for both, I'm lazy)
    private GameManager gameManager;
    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerState = GetComponent<PlayerState>();
        upgradeParticles = transform.Find("Upgrade particles").GetComponent<ParticleSystem>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
        fuelLevel = maxFuelLevel;  // Start with a full tank of fuel
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(ResetSceneJustLoadedFlag());
    }
    private IEnumerator ResetSceneJustLoadedFlag()
    {
        yield return new WaitForSeconds(1.0f); // Wait for 1 second to ensure the scene is fully loaded for audio glitches
        sceneJustLoaded = false;
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
        currentFuelText.text = string.Format("Fuel: {0:#.0} / {1:#.0}", fuelLevel, maxFuelLevel);
    }

    private bool AtPointCheck(Collider other)
    {
        return other.CompareTag("Delivery Point") || other.CompareTag("Pickup Point") || other.CompareTag("Upgrade Point");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (AtPointCheck(other))
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
                atPoint = false;
            }
            else if (other.tag == "Delivery Point")
            {
                DeliveryPoint deliveryPointScript = other.GetComponent<DeliveryPoint>();
                if (deliveryPointScript.questActive) // Only deliver if delivery point actually has an active quest
                {
                    AudioSource[] deliveryAudios = other.GetComponents<AudioSource>();
                    DeliverFuel(deliveryPointScript, deliveryAudios);
                }
                atPoint = false;
            }
            else if (other.tag == "Upgrade Point")
            {
                upgradeManager.OpenUpgradeMenu();
                //playerRb.velocity = new Vector3(0, 0, 0); // Reset player's velocity if they go into the shop to upgrade
                upgradeParticles.Play();
                atPoint = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (AtPointCheck(other))
            atPoint = false;
    }
    void PickupFuel()
    {   
        if (sceneJustLoaded) return;
        // At least for now, picking up fuel at a pickup point completely fills your fuel level to the max
        SetFuel(maxFuelLevel);
        upgradeParticles.Play();
        //Debug.Log("Picked up fuel!");
        gameManager.PlayRefuelSound();
    }

    void DeliverFuel(DeliveryPoint deliveryPoint, AudioSource[] deliveryAudios)
    {
        float fuelToDeliver = deliveryPoint.quest.fuelToDeliver;

        if (fuelLevel > fuelToDeliver)
        {
            // Complete delivery quest
            AddFuel(-fuelToDeliver);
            deliveryPoint.CompleteQuest();
            Debug.Log("Succesfully delivered fuel!");
            gameManager.PlaySuccessfulDeliverySound();
            if (deliveryAudios != null)
            {
                int randomIndex = Random.Range(0, deliveryAudios.Length);
                deliveryAudios[randomIndex].Play(); // Plays a random "thank you" line, will add more lines later
            }
        }
        else
        {
            // Display pop-up text "Not enough fuel!"
            notEnoughFuelText.gameObject.SetActive(true);
            Invoke("HideNotEnoughFuelText", notEnoughFuelTextDuration);
            Debug.Log("Not enough fuel to deliver!");
            gameManager.PlayUnsuccessfulDeliverySound();
        }
    }

    void HideNotEnoughFuelText()
    {
        notEnoughFuelText.gameObject.SetActive(false);
    }
}
