using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePoint : MonoBehaviour
{
    [SerializeField] GameObject[] upgrades;
    private float timer = 0f;
    private bool isTimerRunning = false;

    void Start()
    {
        SpawnRandomUpgrade();
    }

    void SpawnRandomUpgrade()
    {
        if (upgrades.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to the array.");
            return;
        }

        // Choose a random prefab from the array
        int randomIndex = Random.Range(0, upgrades.Length);
        GameObject selectedPrefab = upgrades[randomIndex];

        // Spawn the selected prefab 1 unit above this game object
        Vector3 spawnPosition = transform.position + Vector3.up;
        GameObject spawnedPrefab = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Make the spawned prefab a child of this game object
        spawnedPrefab.transform.parent = transform;
    }

    void Update()
    {
        // Check if the game object has any children
        if (transform.childCount == 0)
        {
            // Update the timer
            timer += Time.deltaTime;

            // Check if the timer has reached 10 seconds
            if (timer >= 10f)
            {
                // Reset the timer and spawn a new random prefab
                isTimerRunning = false;
                SpawnRandomUpgrade();
                // Reset the timer if there are children
                timer = 0f;
            }
        }
    }
}