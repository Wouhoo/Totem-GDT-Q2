using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    // Script for handling the player's flamethrower
    [SerializeField] GameObject flamethrowerCone;
    [SerializeField] ParticleSystem flamethrowerParticles;
    [SerializeField] TextMeshProUGUI flamethrowerHelpText;
    [SerializeField] AudioSource flamethrowerAudioSource;
    private KeyCode flamethrowerButton = KeyCode.Space;

    private PlayerFuel playerFuel;
    private bool flamethrowerActive;
    [Tooltip("How much fuel is consumed by the flamethrower every second")] [SerializeField] float fuelConsumptionRate = -1f;

    private bool flamethrowerUnlocked = false;
    private bool helpTextShown = false;

    void Start()
    {
        playerFuel = FindObjectOfType<PlayerFuel>();
        flamethrowerCone.SetActive(false);
        flamethrowerAudioSource.loop = true;
        flamethrowerAudioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Activate flamethrower when space is pressed & deactivate when released
        if (flamethrowerUnlocked && Input.GetKeyDown(flamethrowerButton))
        {
            flamethrowerActive = true;
            flamethrowerCone.SetActive(true);
            flamethrowerParticles.Play();
            flamethrowerAudioSource.Play();
        }
        if (Input.GetKeyUp(flamethrowerButton) || playerFuel.GetFuelLevel() < 0.01f) // Also deactivate if no fuel left
        {
            flamethrowerActive = false;
            flamethrowerCone.SetActive(false);
            flamethrowerParticles.Stop();
            flamethrowerAudioSource.Stop();
        }
        // Consume fuel for every frame that the flamethrower is active
        if (flamethrowerActive)
        {
            playerFuel.AddFuel(-fuelConsumptionRate * Time.deltaTime);
        }
    }

    public void SetConsumptionRate(float consumptionRate)
    {
        fuelConsumptionRate = consumptionRate;
        flamethrowerUnlocked = fuelConsumptionRate > 0; // We'll check if the flamethrower is unlocked based on the consumption rate;
        // a rate <= 0 means the flamethrower is still locked.
        if(!helpTextShown && flamethrowerUnlocked) // Show help text when you first unlock the flamethrower
        {
            Invoke("ActivateHelpText", 0.01f); // Small delay so the message doesn't pop up while still in the upgrade menu
            helpTextShown = true;
            Invoke("DeactivateHelpText", 7.0f); // Deactivate help text after a delay
        }
    }

    private void ActivateHelpText()
    {
        flamethrowerHelpText.gameObject.SetActive(true);
    }

    private void DeactivateHelpText()
    {
        flamethrowerHelpText.gameObject.SetActive(false);
    }
}
