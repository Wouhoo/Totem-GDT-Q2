using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryPoint : MonoBehaviour
{
    // Code for managing a delivery point and its assigned quest
    public bool questActive = false;
    public Quest quest;

    GameManager gameManager;

    TextMeshProUGUI deliveryText;
    Image deliveryTimer;
    Image deliveryTimerFill;
    float remainingTime;

    Material pointMaterial;
    Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // gray
    Color activeColor = new Color(1.0f, 0.6f, 0.0f);   // orange

    private void Awake()
    {
        // Find related gameobjects & set stuff up
        gameManager = FindObjectOfType<GameManager>();
        deliveryText = transform.Find("DeliveryPointUI/DeliveryText").GetComponent<TextMeshProUGUI>(); // has to be found before the UI is parented to the canvas, hence Awake() instead of Start()
        deliveryTimer = transform.Find("DeliveryPointUI/TimerBG").GetComponent<Image>();               // same here
        deliveryTimerFill = transform.Find("DeliveryPointUI/TimerFill").GetComponent<Image>();         // and here
        pointMaterial = GetComponent<Renderer>().material;
        UISetActive(false);
    }

    private void Update()
    {
        if (questActive)
        {
            // Count down timer for active quest
            remainingTime -= Time.deltaTime;
            deliveryTimerFill.fillAmount = remainingTime / quest.timeLimit;
            if(remainingTime <= 0)
            {
                // Fail quest if time gets <= 0
                FailQuest();
            }
        }
    }

    public void AssignQuest(Quest newQuest)
    {
        // Assign a new quest to this delivery point (called from QuestManager)
        quest = newQuest;
        questActive = true;
        remainingTime = quest.timeLimit;
        UISetActive(true);
        deliveryText.text = string.Format("{0}", quest.fuelToDeliver); // Displays only fuel to deliver for now; might display more/other info later.
    }

    public void CompleteQuest()
    {
        // Complete this point's assigned quest (called from PlayerFuel)
        gameManager.UpdateScore(quest.pointReward);
        quest = null;
        questActive = false;
        UISetActive(false);
    }

    void FailQuest()
    {
        // Fail quest if fuel is not delivered within time limit (called from Update() if remaining time is < 0)
        gameManager.GameOver(); // For now, failing a quest will be an instant game over
        quest = null;
        questActive = false;
        UISetActive(false);
    }

    void UISetActive(bool active)
    {
        // (de)activate all UI elements
        deliveryText.gameObject.SetActive(active);
        deliveryTimer.gameObject.SetActive(active);
        deliveryTimerFill.gameObject.SetActive(active);
        if (active) pointMaterial.color = activeColor; 
        else pointMaterial.color = inactiveColor;
    }
}
