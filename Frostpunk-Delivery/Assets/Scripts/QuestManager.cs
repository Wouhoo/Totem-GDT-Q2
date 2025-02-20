using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    DeliveryPoint[] deliveryPoints;

    float startDelay = 3.0f;  // time until first quest is started
    float questDelay = 25.0f; // time between quest spawns

    // Quests will ask for an amount of fuel between questMinFuel and questMaxFuel.
    // The amount of fuel determines the reward and time limit. 
    int questMinFuel = 10;
    int questMaxFuel = 20;
    int questMinTime = 60;
    int questMaxTime = 90;
    float largeDeliveryModifier = 2f; // largeDeliveryModifier * fuelToDeliver will be added to quest time limit

    void Start()
    {
        deliveryPoints = FindObjectsOfType<DeliveryPoint>();
        InvokeRepeating("SpawnQuest", startDelay, questDelay);
    }

    void SpawnQuest()
    {
        // Select a random delivery point which doesn't already have a quest assigned to it
        DeliveryPoint[] inactivePoints = deliveryPoints.Where(deliveryPoint => !deliveryPoint.questActive).ToArray();
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
