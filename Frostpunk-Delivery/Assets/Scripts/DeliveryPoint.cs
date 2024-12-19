using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    // Code for managing a delivery point and its assigned quest
    public bool questActive = false;
    public Quest quest;

    GameManager gameManager;
    TextMeshProUGUI deliveryText;

    Material pointMaterial;
    Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // gray
    Color activeColor = new Color(1.0f, 0.6f, 0.0f);   // orange

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        deliveryText = GetComponentInChildren<TextMeshProUGUI>(); // has to be found before the text is parented to the canvas, hence Awake() instead of Start()
        deliveryText.gameObject.SetActive(false);
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
        deliveryText.gameObject.SetActive(true);
        deliveryText.text = string.Format("{0}", quest.fuelToDeliver); // Displays only fuel to deliver for now; might display more/other info later.
    }

    public void CompleteQuest()
    {
        // Complete this point's assigned quest (called from PlayerFuel)
        CancelInvoke("FailQuest"); // Cancel failure countdown
        gameManager.UpdateScore(quest.pointReward);
        quest = null;
        questActive = false;
        deliveryText.gameObject.SetActive(false);
        pointMaterial.color = inactiveColor;
    }

    void FailQuest()
    {
        // Fail quest if fuel is not delivered within time limit (invoked in AssignQuest and canceled in CompleteQuest)
        gameManager.GameOver(); // For now, failing a quest will be an instant game over
        quest = null;
        questActive = false;
        deliveryText.gameObject.SetActive(false);
        pointMaterial.color = inactiveColor;
    }
}
