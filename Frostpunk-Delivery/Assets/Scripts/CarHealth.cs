using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealth : MonoBehaviour
{
    private PlayerState playerState;
    public float _maxCarHealth = 10;
    public float _carHealth;
    private Vector3 oldVelocity = new Vector3(0f, 0f, 0f);
    private float acceleration;
    public float minAccForDamage = 5f;
    public float accToDamageFactor = 0.5f;

    private CameraController cameraController;
    [SerializeField] float accToShakeFactor = 0.01f;
    private GameManager gameManager;
    
    [Header("Smoke effects")]
    private ParticleSystem smokeParticles;
    private ParticleSystem.MainModule smokeMain;
    [SerializeField] float smokeThreshold = 50.0f; // You'll start seeing smoke if your health falls below this amount
    [SerializeField] float smokeMultiplier = 3.0f; // Smoke gets more intense as you get more damaged, up to this multiplier at 0 health
    // Smoke size formula: 0 if health > smokeThreshold, else 1 + (smokeThreshold - health)/smokeThreshold * smokeMultiplier

    [Header("Collision Sound")]
    [SerializeField] private float collisionSoundCooldown = 0.1f; // Adjust this value to control minimum time between sounds
    private float lastCollisionSoundTime = 0f;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerState = GetComponent<PlayerState>();
        cameraController = FindObjectOfType<CameraController>();
        smokeParticles = transform.Find("Smoke particles").GetComponent<ParticleSystem>();
        smokeMain = smokeParticles.main;
        _carHealth = _maxCarHealth;
    }

    public void NewVelocity(Vector3 velocity, float deltaTime)
    {
        acceleration = Vector3.Distance(velocity, oldVelocity) / deltaTime;
        oldVelocity = velocity;
        if (acceleration > minAccForDamage)
        {
            Debug.Log(acceleration);
            Set_Health(_carHealth - acceleration * accToDamageFactor);
            cameraController.StartShaking(acceleration * accToShakeFactor);
            
            // To prevent our ears from bleeding
            if (Time.time - lastCollisionSoundTime >= collisionSoundCooldown)
            {
                gameManager.PlayCollisionSound();
                lastCollisionSoundTime = Time.time;
            }
        }
    }

    public void Set_Health(float amount)
    {
        _carHealth = Mathf.Clamp(amount, 0f, _maxCarHealth);
        playerState.CarBroke(_carHealth < 0.1f);
        // Show smoke when health gets below threshold, size increases the lower health gets
        if (_carHealth < smokeThreshold)
        {
            smokeParticles.Play();
            smokeMain.startSizeMultiplier = 1 + smokeMultiplier * (smokeThreshold - _carHealth) / smokeThreshold;
        }
        else
        {
            smokeParticles.Stop();
        }
    }

    public void Add_Health(float amount)
    {
        Set_Health(_carHealth + amount);
    }

    public void Set_MaxHealth(float amount)
    {
        _maxCarHealth = amount;
    }
}
