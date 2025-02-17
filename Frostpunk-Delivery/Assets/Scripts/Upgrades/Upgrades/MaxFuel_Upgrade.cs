using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxFuel_Upgrade : MonoBehaviour, IUpgrade
{
    private UpgradeManager upgradeManager;

    void Awake()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    public void Upgrade()
    {
        upgradeManager.FuelCapacity_Upgrade();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Upgrade();
            Destroy(gameObject);
        }
    }
}
