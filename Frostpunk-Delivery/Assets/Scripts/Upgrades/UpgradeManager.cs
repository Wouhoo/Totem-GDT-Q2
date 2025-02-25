using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // This script handles everything related to upgrades & the upgrade menu.
    [SerializeField] GameObject upgradeMenu;
    private GameManager _gameManager;
    private PlayerFuel _playerFuel;
    private CarHealth _carHealth;
    private FlameThrower _flameThrower;
    private CarController _carController;


    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerFuel = FindObjectOfType<PlayerFuel>();
        _carHealth = FindObjectOfType<CarHealth>();
        _flameThrower = FindObjectOfType<FlameThrower>();
        _carController = FindObjectOfType<CarController>();
        upgradeMenu.SetActive(false);

        fuelCapacityUpgradeButtonText.text = string.Format("Upgrade (${0})", fuelcapacityCostAtLevel[0]);
        flamethrowerUpgradeButtonText.text = string.Format("Unlock (${0})", flamethrowerCostAtLevel[0]); // Flamethrower's first upgrade unlocks it
        maxHealth_UpgradeButtonText.text = string.Format("Upgrade (${0})", maxHealth_CostAtLevel[0]);
        maxSpeed_UpgradeButtonText.text = string.Format("Upgrade (${0})", maxSpeed_CostAtLevel[0]);
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

    [Header("Upgrade Fuel Capacity")]

    private int fuelCapacityLevel = 0;                                                  // Current fuel capacity upgrade level
    private float[] fuelCapacityAtLevel = new float[] { 80f, 120f, 170f, 230f, 300f };  // Fuel capacity at each upgrade level
    private int[] fuelcapacityCostAtLevel = new int[] { 10, 25, 45, 70, 100 };          // Cost of upgrading to the next level
    [SerializeField] Image fuelCapacityUpgradeMeter;
    [SerializeField] TextMeshProUGUI fuelCapacityUpgradeButtonText;

    public void UpgradeFuelCapacity()
    {
        if (fuelCapacityLevel < fuelCapacityAtLevel.Length)  // Check if max level has been reached
        {
            if (_gameManager.playerMoney >= fuelcapacityCostAtLevel[fuelCapacityLevel])  // Check if player has enough money
            {
                _gameManager.UpdateScore(-fuelcapacityCostAtLevel[fuelCapacityLevel]);  // Pay money
                _playerFuel.SetCapacity(fuelCapacityAtLevel[fuelCapacityLevel]);  // Increase capacity
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

    [Header("Upgrade Flamethrower")]

    private int flamethrowerLevel = 0;
    private float[] flamethrowerConsumptionAtLevel = new float[] { 5.0f, 4.0f, 3.0f, 2.0f, 1.0f };
    private int[] flamethrowerCostAtLevel = new int[] { 30, 10, 15, 20, 25 };  // First upgrade unlocks the flamethrower, therefore more expensive
    [SerializeField] Image flamethrowerUpgradeMeter;
    [SerializeField] TextMeshProUGUI flamethrowerUpgradeButtonText;

    public void UpgradeFlamethrower()
    {
        if (flamethrowerLevel < flamethrowerConsumptionAtLevel.Length)  // Check if max level has been reached
        {
            if (_gameManager.playerMoney >= flamethrowerCostAtLevel[flamethrowerLevel])  // Check if player has enough money
            {
                _gameManager.UpdateScore(-flamethrowerCostAtLevel[flamethrowerLevel]);  // Pay money
                _flameThrower.SetConsumptionRate(flamethrowerConsumptionAtLevel[flamethrowerLevel]);  // Set new fuel consumption rate
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

    [Header("Upgrade Max Health")]

    private int maxHealth_Level = 0;
    private float[] maxHealth_AtLevel = new float[] { 100f, 110f, 120f, 130f, 140f };
    private int[] maxHealth_CostAtLevel = new int[] { 15, 30, 45, 60, 75 };
    [SerializeField] Image maxHealth_UpgradeMeter;
    [SerializeField] TextMeshProUGUI maxHealth_UpgradeButtonText;

    public void UpgradeMaxHealth()
    {
        if (maxHealth_Level < maxHealth_AtLevel.Length)  // Check if max level has been reached
        {
            if (_gameManager.playerMoney >= maxHealth_CostAtLevel[maxHealth_Level])  // Check if player has enough money
            {
                _gameManager.UpdateScore(-maxHealth_CostAtLevel[maxHealth_Level]);  // Pay money
                _carHealth.Set_MaxHealth(maxHealth_AtLevel[maxHealth_Level]);  // Increase capacity
                maxHealth_Level++;
                maxHealth_UpgradeMeter.fillAmount = (float)maxHealth_Level / (float)maxHealth_AtLevel.Length;  // Fill upgrade meter in shop screen
                // Update upgrade button text with next cost
                if (maxHealth_Level < maxHealth_CostAtLevel.Length)
                    maxHealth_UpgradeButtonText.text = string.Format("Upgrade (${0})", maxHealth_CostAtLevel[maxHealth_Level]);
                else
                    maxHealth_UpgradeButtonText.text = "MAX LEVEL REACHED";
            }
            else
                Debug.Log("Not enough money!");
        }
        else
            Debug.Log("Max level reached!");
    }

    [Header("Upgrade Max Speed")]

    private int maxSpeed_Level = 0;
    private float[] maxSpeed_AtLevel = new float[] { 35f, 37.5f, 40f, 42.5f, 45f };
    private int[] maxSpeed_CostAtLevel = new int[] { 10, 15, 20, 25, 30 };
    [SerializeField] Image maxSpeed_UpgradeMeter;
    [SerializeField] TextMeshProUGUI maxSpeed_UpgradeButtonText;

    public void UpgradeMaxSpeed()
    {
        if (maxSpeed_Level < maxSpeed_AtLevel.Length)  // Check if max level has been reached
        {
            if (_gameManager.playerMoney >= maxSpeed_CostAtLevel[maxSpeed_Level])  // Check if player has enough money
            {
                _gameManager.UpdateScore(-maxSpeed_CostAtLevel[maxSpeed_Level]);  // Pay money
                _carController.Set_MaxSpeed(maxSpeed_AtLevel[maxSpeed_Level]);  // Increase
                maxSpeed_Level++;
                maxSpeed_UpgradeMeter.fillAmount = (float)maxSpeed_Level / (float)maxSpeed_AtLevel.Length;  // Fill upgrade meter in shop screen
                // Update upgrade button text with next cost
                if (maxSpeed_Level < maxSpeed_CostAtLevel.Length)
                    maxSpeed_UpgradeButtonText.text = string.Format("Upgrade (${0})", maxSpeed_CostAtLevel[maxSpeed_Level]);
                else
                    maxSpeed_UpgradeButtonText.text = "MAX LEVEL REACHED";
            }
            else
                Debug.Log("Not enough money!");
        }
        else
            Debug.Log("Max level reached!");
    }


    // Pickups
    [Header("Pickups")]

    [SerializeField] Vector2Int moneyRandRange;
    public void MoneyPickup()
    {
        int money = Random.Range(moneyRandRange.x, moneyRandRange.y);
        _gameManager.UpdateScore(money);
    }

    [SerializeField] Vector2Int fuelRandRange;
    public void FuelPickup()
    {
        int fuel = Random.Range(fuelRandRange.x, fuelRandRange.y);
        _playerFuel.AddFuel(fuel);
    }

    [SerializeField] Vector2Int healthRandRange;
    public void HealthPickup()
    {
        int health = Random.Range(fuelRandRange.x, fuelRandRange.y);
        _carHealth.Add_Health(health);
    }


    [Header("Full Heal")]
    // full heal for in shop
    [SerializeField] int fullHealCost;
    public void PayForFullHeal()
    {
        if (_gameManager.playerMoney >= fullHealCost)  // Check if player has enough money
        {
            _gameManager.UpdateScore(-fullHealCost);  // Pay money
            _carHealth.Set_Health(_carHealth._maxCarHealth);
        }
        else
            Debug.Log("Not enough money!");
    }
}
