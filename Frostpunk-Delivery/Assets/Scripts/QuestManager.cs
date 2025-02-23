using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    DeliveryPoint[] deliveryPoints;

    // Quests will ask for an amount of fuel between questMinFuel and questMaxFuel.
    // The amount of fuel determines the reward and time limit. 
    [Header("Quest settings")]
    [Tooltip("Seconds until first quest is spawned")] [SerializeField] float startDelay = 3.0f;
    [Tooltip("Seconds delay between quests")] [SerializeField] float questDelay = 30.0f;
    [SerializeField] int questMinFuel = 10;
    [SerializeField] int questMaxFuel = 20;
    [SerializeField] int questMinTime = 70;
    [SerializeField] int questMaxTime = 100;
    [Tooltip("This number * fuelToDeliver will be added to quest time limit")] [SerializeField] float largeDeliveryModifier = 2f;

    void Start()
    {
        deliveryPoints = FindObjectsOfType<DeliveryPoint>();
        InvokeRepeating("SpawnQuest", startDelay, questDelay);
    }

    void SpawnQuest()
    {
        // Select a random delivery point which doesn't already have a quest assigned to it
        DeliveryPoint[] inactivePoints = deliveryPoints.Where(deliveryPoint => (!deliveryPoint.questActive && !deliveryPoint.pointFrozen)).ToArray();
        if (inactivePoints.Length == 0) // If all points are already active somehow, don't assign a new quest (the player will lose soon enough)
            return;
        DeliveryPoint deliveryPoint = inactivePoints[Random.Range(0, inactivePoints.Length)];

        // Generate a new quest
        Quest quest = new Quest();
        float fuelToDeliver = Random.Range(questMinFuel, questMaxFuel);
        float baseTimeLimit = Random.Range(questMinTime, questMaxTime);
        quest.fuelToDeliver = fuelToDeliver;
        quest.pointReward = (int)fuelToDeliver; // For now, point reward will be equal to fuel delivered
        quest.timeLimit = baseTimeLimit + largeDeliveryModifier*fuelToDeliver; // Add extra time for larger deliveries

        // Assign the quest
        deliveryPoint.AssignQuest(quest);
        Debug.Log(string.Format("Assigned quest to point {0}. Fuel: {1}, Reward: {2}, Time limit: {3}", deliveryPoint.name, quest.fuelToDeliver, quest.pointReward, quest.timeLimit));
    }
}
