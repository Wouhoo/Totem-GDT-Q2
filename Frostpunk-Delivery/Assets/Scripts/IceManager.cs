using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceManager : MonoBehaviour
{
    // Script for spawning & managing ice areas
    public float iceFrictionCoef = 0.3f; // land friction is 0.8
    public CarController carController;
    private float iceAreaCounter = 0f;

    [SerializeField] int areasToSpawn = 40;    // Amount of areas to spawn randomly across the map
    [SerializeField] GameObject iceAreaPrefab; // Ice area prefab (should have correct initial scale!)    
    // For now areas are spawned randomly in a circle with the following center and radius
    Vector3 spawnCircleCenter = new Vector3(-200, 0, 230);
    float spawnCircleRadius = 700;
    int spawnLayer; // LayerMask for areas in which ice is allowed to spawn (this is only the IceSpawnArea layer)

    void Start()
    {
        spawnLayer = ~LayerMask.NameToLayer("IceSpawnArea"); // Yes, the ~ here serves a vital purpose. Do ask me (Wouter) about it if you're curious, it's too long to explain here.
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

    public void InsideIceArea(bool isInside) // true if entered, false if exited
    {
        // this function keeps track of which ice areas we are in and changes the players friction

        if (isInside)
        {
            if (iceAreaCounter == 0)
                carController.SetFriction(iceFrictionCoef);
            Debug.Log("Changed");
            iceAreaCounter += 1;
        }
        else
        {
            iceAreaCounter -= 1;
            if (iceAreaCounter == 0)
                carController.ResetFriction();
        }
    }
}
