using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxSpeed_Upgrade : MonoBehaviour, IUpgrade
{
    private UpgradeManager upgradeManager;

    void Awake()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
    }

    public void Upgrade()
    {
        upgradeManager.MaxSpeed_Upgrade();
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
