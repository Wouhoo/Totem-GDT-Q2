using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceManager : MonoBehaviour
{
    // Script for spawning & managing ice areas
    public CarController carController;

    [SerializeField] int areasToSpawn = 40;    // Amount of areas to spawn randomly across the map
    [SerializeField] GameObject iceAreaPrefab; // Ice area prefab (should have correct initial scale!)    
    // For now areas are spawned randomly in a circle with the following center and radius
    Vector3 spawnCircleCenter = new Vector3(-200, 0, 230);
    float spawnCircleRadius = 700;
    int spawnLayer; // LayerMask for areas in which ice is allowed to spawn (this is only the IceSpawnArea layer)

    void Start()
    {
        spawnLayer = (1 << LayerMask.NameToLayer("Road")) | (1 << LayerMask.NameToLayer("DirtRoad")); // Allow spawning on regular or dirt road 
        SpawnIceAreas();
    }

    private void SpawnIceAreas()
    {
        Vector2 unitCirclePosition = Vector2.zero;
        Vector3 spawnPosition = Vector3.zero;
        for (int i = 0; i < areasToSpawn; i++)
        {
            do 
            {
                unitCirclePosition = Random.insideUnitCircle; // Generate random point on unit circle
                spawnPosition = new Vector3(spawnCircleCenter.x + spawnCircleRadius * unitCirclePosition.x, spawnCircleCenter.y, spawnCircleCenter.z + spawnCircleRadius * unitCirclePosition.y); // Translate to actual spawn position
                //Debug.Log(string.Format("Area {0} spawn position: {1}", i, spawnPosition)); // TEST
                //Debug.Log(string.Format("Area {0} sphere check: {1}", i, Physics.CheckSphere(spawnPosition, 0.1f, spawnLayer, QueryTriggerInteraction.Collide))); // TEST
            } 
            while (!Physics.CheckSphere(spawnPosition, 0.1f, spawnLayer, QueryTriggerInteraction.Collide)); // Reroll spawn position until it overlaps with a valid spawn area (road)
            Instantiate(iceAreaPrefab, spawnPosition, Quaternion.identity, transform); // Spawn object at spawnPosition as child of IceManager
        }
    }
}
