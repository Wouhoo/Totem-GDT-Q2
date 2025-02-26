using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryPoint : MonoBehaviour
{
    // Code for managing a delivery point and its assigned quest
    public bool questActive = false;
    public bool pointFrozen = false;
    public Quest quest;

    [SerializeField] GameObject iceArea;

    TextMeshProUGUI deliveryText;
    Image deliveryTimer;
    Image deliveryTimerFill;
    Color initialColor = Color.yellow; // Color the timer will have when full
    Color finalColor = Color.red;      // Color the timer will transition to while emptying
    float remainingTime;

    Material pointMaterial;
    Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // gray
    Color activeColor = new Color(1.0f, 0.6f, 0.0f);   // orange

    [Header("Flame area & particles")]
    [SerializeField] GameObject flameArea;
    [SerializeField] float flameDuration = 2.0f;
    private ParticleSystem flameParticles;

    [Header("Delivery Point Beacon")]
    public float _beamColor;
    Transform _player;
    [SerializeField] Transform _beam;

    [Header("Alarm Reminder")]
    private bool alarmPlayed = false;
    [SerializeField] private float alarmThreshold = 0.3f; // 30% of time remaining
    private GameManager gameManager;

    private void Awake()
    {
        // Find related gameobjects & set stuff up
        gameManager = FindObjectOfType<GameManager>();
        flameParticles = GetComponentInChildren<ParticleSystem>();
        deliveryText = transform.Find("DeliveryPointUI/DeliveryText").GetComponent<TextMeshProUGUI>(); // has to be found before the UI is parented to the canvas, hence Awake() instead of Start()
        deliveryTimer = transform.Find("DeliveryPointUI/TimerBG").GetComponent<Image>();               // same here
        deliveryTimerFill = transform.Find("DeliveryPointUI/TimerFill").GetComponent<Image>();         // and here
        pointMaterial = GetComponent<Renderer>().material;
        UISetActive(false);
        iceArea.SetActive(false);

        _player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (questActive)
        {
            // Count down timer for active quest
            remainingTime -= Time.deltaTime;
            float remainingFraction = remainingTime / quest.timeLimit; 
            // Play alarm sound
            if (!alarmPlayed && remainingFraction <= alarmThreshold)
            {
                gameManager.PlayAlarmSound();
                alarmPlayed = true;
            }
            // Change timer fill & color depending on remaining time
            deliveryTimerFill.fillAmount = remainingFraction;
            deliveryTimerFill.color = Color.Lerp(finalColor, initialColor, remainingFraction); // Reversed since fraction counts down from 1 to 0
            // Fail quest if time gets <= 0
            if (remainingTime <= 0)
            {
                FailQuest();
            }

            // if active quest; adjust the beam based on player distance
            float dist = Vector3.Distance(transform.position, _player.position);
            _beam.localScale = new Vector3(dist / 20 + 1, 99999f, dist / 20 + 1);
        }
    }

    public void AssignQuest(Quest newQuest)
    {
        // Assign a new quest to this delivery point (called from QuestManager)
        quest = newQuest;
        questActive = true;
        remainingTime = quest.timeLimit;
        alarmPlayed = false;
        UISetActive(true);
        deliveryText.text = string.Format("{0}", quest.fuelToDeliver); // Displays only fuel to deliver for now; might display more/other info later.

        // show beam
        _beam.gameObject.SetActive(true);
    }

    public void CompleteQuest()
    {
        // Complete this point's assigned quest (called from PlayerFuel)
        gameManager.UpdateScore(quest.pointReward);
        quest = null;
        questActive = false;
        UISetActive(false);

        // Melt ice around point by briefly spawning a flame area around it
        flameParticles.Play();
        flameArea.SetActive(true);
        Invoke("DeactivateFlameArea", flameDuration);

        // hide beam
        _beam.gameObject.SetActive(false);
    }

    void FailQuest()
    {
        // Fail quest if fuel is not delivered within time limit (called from Update() if remaining time is < 0)
        gameManager.AddFailedQuest();
        quest = null;
        questActive = false;
        UISetActive(false);
        pointFrozen = true;
        iceArea.SetActive(true); // Spawn ice area at failed point

        // hide beam
        _beam.gameObject.SetActive(false);
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

    void DeactivateFlameArea()
    {
        flameArea.SetActive(false);
    }
}
