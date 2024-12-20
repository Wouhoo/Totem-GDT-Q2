using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // This script handles everything related to upgrades & the upgrade menu.
    [SerializeField] GameObject upgradeMenu;
    private GameManager gameManager;

    // Fuel upgrades
    private PlayerFuel playerFuelScript;
    private int fuelCapacityLevel = 0;                                             // Current fuel capacity upgrade level
    private float[] capacityAtLevel = new float[] { 80f, 120f, 170f, 230f, 300f }; // Fuel capacity at each upgrade level
    private int[] costAtLevel = new int[] { 10, 25, 45, 70, 100 };                 // Cost of upgrading to the next level
    [SerializeField] Image fuelUpgradeMeter;
    [SerializeField] TextMeshProUGUI fuelUpgradeButtonText;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerFuelScript = FindObjectOfType<PlayerFuel>();
        upgradeMenu.SetActive(false);

        fuelUpgradeButtonText.text = string.Format("Upgrade (${0})", costAtLevel[0]);
    }

    public void OpenUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        Time.timeScale = 0; // Freeze time while in upgrade menu
    }

    public void CloseUpgradeMenu() 
    { 
        upgradeMenu.SetActive(false);
        Time.timeScale = 1; // Resume simulation at normal speed
    }

    public void UpgradeFuelCapacity()
    {
        if (fuelCapacityLevel < capacityAtLevel.Length)
        {
            if(gameManager.playerScore >= costAtLevel[fuelCapacityLevel])
            {
                gameManager.UpdateScore(-costAtLevel[fuelCapacityLevel]);
                playerFuelScript.SetCapacity(capacityAtLevel[fuelCapacityLevel]);
                fuelCapacityLevel++;
                fuelUpgradeMeter.fillAmount = (float)fuelCapacityLevel / (float)capacityAtLevel.Length;
                if (fuelCapacityLevel < costAtLevel.Length)
                    fuelUpgradeButtonText.text = string.Format("Upgrade (${0})", costAtLevel[fuelCapacityLevel]);
                else
                    fuelUpgradeButtonText.text = "MAX LEVEL REACHED";
            }
            else
            {
                Debug.Log("Not enough money!");
            }
        }
        else
        {
            Debug.Log("Max level reached!");
        }
    }
}
