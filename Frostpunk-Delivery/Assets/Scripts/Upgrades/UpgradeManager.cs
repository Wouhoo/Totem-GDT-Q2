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
    private float[] fuelCapacityAtLevel = new float[] { 80f, 120f, 170f, 230f, 300f }; // Fuel capacity at each upgrade level
    private int[] fuelcapacityCostAtLevel = new int[] { 10, 25, 45, 70, 100 };                 // Cost of upgrading to the next level
    [SerializeField] Image fuelCapacityUpgradeMeter;
    [SerializeField] TextMeshProUGUI fuelCapacityUpgradeButtonText;

    // Health upgrades (for now always repair for free when stopping at the shop)
    private CarHealth playerHealthScript;

    // Flamethrower upgrades
    private FlameThrower flameThrower;
    [SerializeField] Image flamethrowerUpgradeMeter;
    [SerializeField] TextMeshProUGUI flamethrowerUpgradeButtonText;
    private int flamethrowerLevel = 0;
    private float[] flamethrowerConsumptionAtLevel = new float[] { 5.0f, 4.0f, 3.0f, 2.0f, 1.0f};
    private int[] flamethrowerCostAtLevel = new int[] { 30, 10, 15, 20, 25 };  // First upgrade unlocks the flamethrower, therefore more expensive

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerFuelScript = FindObjectOfType<PlayerFuel>();
        playerHealthScript = FindObjectOfType<CarHealth>();
        flameThrower = FindObjectOfType<FlameThrower>();
        upgradeMenu.SetActive(false);

        fuelCapacityUpgradeButtonText.text = string.Format("Upgrade (${0})", fuelcapacityCostAtLevel[0]);
        flamethrowerUpgradeButtonText.text = string.Format("Unlock (${0})", flamethrowerCostAtLevel[0]); // Flamethrower's first upgrade unlocks it
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
        playerHealthScript.Set_Health(playerHealthScript._maxCarHealth); // Also fully repair the car

    }

    public void UpgradeFuelCapacity()
    {
        if (fuelCapacityLevel < fuelCapacityAtLevel.Length)  // Check if max level has been reached
        {
            if (gameManager.playerMoney >= fuelcapacityCostAtLevel[fuelCapacityLevel])  // Check if player has enough money
            {
                gameManager.UpdateScore(-fuelcapacityCostAtLevel[fuelCapacityLevel]);  // Pay money
                playerFuelScript.SetCapacity(fuelCapacityAtLevel[fuelCapacityLevel]);  // Increase capacity
                fuelCapacityLevel++;
                fuelCapacityUpgradeMeter.fillAmount = (float)fuelCapacityLevel / (float)fuelCapacityAtLevel.Length;  // Fill upgrade meter in shop screen
                // Update upgrade button text with next cost
                if (fuelCapacityLevel < fuelcapacityCostAtLevel.Length)
                    fuelCapacityUpgradeButtonText.text = string.Format("Upgrade (${0})", fuelcapacityCostAtLevel[fuelCapacityLevel]);  
                else
                    fuelCapacityUpgradeButtonText.text = "MAX LEVEL REACHED";
            }
            else
                Debug.Log("Not enough money!");
        }
        else
            Debug.Log("Max level reached!");
    }

    public void UpgradeFlamethrower()
    {
        if (flamethrowerLevel < flamethrowerConsumptionAtLevel.Length)  // Check if max level has been reached
        {
            if (gameManager.playerMoney >= flamethrowerCostAtLevel[flamethrowerLevel])  // Check if player has enough money
            {
                gameManager.UpdateScore(-flamethrowerCostAtLevel[flamethrowerLevel]);  // Pay money
                flameThrower.SetConsumptionRate(flamethrowerConsumptionAtLevel[flamethrowerLevel]);  // Set new fuel consumption rate
                flamethrowerLevel++;
                flamethrowerUpgradeMeter.fillAmount = (float)flamethrowerLevel / (float)flamethrowerConsumptionAtLevel.Length;  // Fill upgrade meter in shop screen
                // Update upgrade button text with next cost
                if (flamethrowerLevel < flamethrowerCostAtLevel.Length)
                    flamethrowerUpgradeButtonText.text = string.Format("Upgrade (${0})", flamethrowerCostAtLevel[flamethrowerLevel]);
                else
                    flamethrowerUpgradeButtonText.text = "MAX LEVEL REACHED";
            }
            else
                Debug.Log("Not enough money!");
        }
        else
            Debug.Log("Max level reached!");
    }

    // UPGRADES (boiler plate stuff)

    [Header("Fuel Capacity")]
    public float _fuelCapacity;
    public float _fuelCapacity_increment;
    private float _fuelCapacity_buff = 1f;
    [SerializeField] PlayerFuel playerFuel;
    public void FuelCapacity_Upgrade()
    {
        _fuelCapacity += _fuelCapacity_increment;
        FuelCapacity_ApplyBuff(_fuelCapacity_buff);
    }
    public void FuelCapacity_ApplyBuff(float buffPercent)
    {
        _fuelCapacity_buff = buffPercent;
        playerFuel.SetCapacity(_fuelCapacity * _fuelCapacity_buff);
    }

    [Header("Max Speed")]
    public float _maxSpeed;
    public float _maxSpeed_increment;
    private float _maxSpeed_buff = 1f;
    [SerializeField] CarController carController;
    public void MaxSpeed_Upgrade()
    {
        _maxSpeed += _maxSpeed_increment;
        MaxSpeed_ApplyBuff(_fuelCapacity_buff);
    }
    public void MaxSpeed_ApplyBuff(float buffPercent)
    {
        _maxSpeed_buff = buffPercent;
        carController.maxSpeed = _maxSpeed * _maxSpeed_buff;
    }

    [Header("Max Car Health")]
    public float _maxCarHealth;
    public float _maxCarHealth_increment;
    private float _maxCarHealth_buff = 1f;
    [SerializeField] CarHealth carHealth;
    public void MaxCarHealth_Upgrade()
    {
        _maxCarHealth += _maxCarHealth_increment;
        MaxCarHealth_ApplyBuff(_fuelCapacity_buff);
    }
    public void MaxCarHealth_ApplyBuff(float buffPercent)
    {
        _maxCarHealth_buff = buffPercent;
        carHealth._maxCarHealth = _maxCarHealth * _maxCarHealth_buff;
    }

    // Pickups
    [Header("Pickups")]
    [SerializeField] Vector2Int moneyRandRange;
    public void MoneyPickup()
    {
        int money = Random.Range(moneyRandRange.x, moneyRandRange.y);
        gameManager.UpdateScore(money);
    }

    [SerializeField] Vector2Int fuelRandRange;
    public void FuelPickup()
    {
        int fuel = Random.Range(fuelRandRange.x, fuelRandRange.y);
        playerFuel.AddFuel(fuel);
    }
}
