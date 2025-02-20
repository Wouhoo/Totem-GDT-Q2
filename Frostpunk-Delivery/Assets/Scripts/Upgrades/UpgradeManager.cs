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
            if (gameManager.playerMoney >= costAtLevel[fuelCapacityLevel])
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
