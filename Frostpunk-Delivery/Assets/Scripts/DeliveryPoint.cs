using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    // Code for managing a delivery point and its assigned quest
    public bool questActive = false;
    public Quest quest;

    private GameManager gameManager;

    Material pointMaterial;
    Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // gray
    Color activeColor = new Color(1.0f, 0.6f, 0.0f);   // orange

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pointMaterial = GetComponent<Renderer>().material;
        pointMaterial.color = inactiveColor;
    }

    public void AssignQuest(Quest newQuest)
    {
        // Assign a new quest to this delivery point (called from QuestManager)
        quest = newQuest;
        questActive = true;
        Invoke("FailQuest", quest.timeLimit); // Start counting down towards quest failure
        pointMaterial.color = activeColor;
    }

    public void CompleteQuest()
    {
        // Complete this point's assigned quest (called from PlayerFuel)
        CancelInvoke("FailQuest"); // Cancel failure countdown
        gameManager.UpdateScore(quest.pointReward);
        quest = null;
        questActive = false;
        pointMaterial.color = inactiveColor;
    }

    void FailQuest()
    {
        // Fail quest if fuel is not delivered within time limit (invoked in AssignQuest and canceled in CompleteQuest)
        gameManager.GameOver(); // For now, failing a quest will be an instant game over
        quest = null;
        questActive = false;
        pointMaterial.color = inactiveColor;
    }
}
