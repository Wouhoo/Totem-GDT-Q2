using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePoint : MonoBehaviour
{
    [SerializeField] GameObject[] upgrades;
    [SerializeField] float respawnTimer = 10.0f; // Seconds until another powerup spawns
    private Vector3 spawnOffset = new Vector3(0, 3.0f, 0);
    private float timer = 0f;

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

        // Spawn the selected prefab above this game object
        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject spawnedPrefab = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Make the spawned prefab a child of this game object
        spawnedPrefab.transform.parent = transform;
    }

    void Update()
    {
        // Check if the game object has no children besides the mesh
        if (transform.childCount <= 1)
        {
            // Update the timer
            timer += Time.deltaTime;

            // Check if the timer has reached 10 seconds
            if (timer >= respawnTimer)
            {
                // Reset the timer and spawn a new random prefab
                SpawnRandomUpgrade();
                // Reset the timer if there are children
                timer = 0f;
            }
        }
    }
}