using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel_Pickup : MonoBehaviour
{
    private UpgradeManager upgradeManager;
    [SerializeField] float rotationSpeed = 30f;

    void Awake()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        transform.eulerAngles = new Vector3(0, 0, 0f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            upgradeManager.FuelPickup();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World); // Rotate powerup (to make it draw attention)
    }
}
