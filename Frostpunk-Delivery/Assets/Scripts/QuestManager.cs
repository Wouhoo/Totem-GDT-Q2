using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    DeliveryPoint[] deliveryPoints;

    float startDelay = 2.0f;  // time until first quest is started
    float questDelay = 10.0f; // time between quest spawns

    // Quests will ask for an amount of fuel between questMinFuel and questMaxFuel.
    // The amount of fuel determines the reward and time limit. 
    int questMinFuel = 10;
    int questMaxFuel = 20;

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
        quest.fuelToDeliver = fuelToDeliver;
        quest.pointReward = (int)fuelToDeliver; // For now, point reward will be equal to fuel delivered
        quest.timeLimit = 10f + 0.5f*fuelToDeliver; // For now, time limit for quest is 10 + 0.5*fuel to deliver (i.e. larger deliveries give you more time)

        // Assign the quest
        deliveryPoint.AssignQuest(quest);
        Debug.Log(string.Format("Assigned quest to point {0}. Fuel: {1}, Reward: {2}, Time limit: {3}", deliveryPoint.name, quest.fuelToDeliver, quest.pointReward, quest.timeLimit));
    }
}
