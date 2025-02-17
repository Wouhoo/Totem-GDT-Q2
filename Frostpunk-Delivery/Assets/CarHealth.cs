using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealth : MonoBehaviour
{
    private PlayerState playerState;
    public float _maxCarHealth = 10;
    private float _carHealth;
    private Vector3 oldVelocity = new Vector3(0f, 0f, 0f);
    private float acceleration;
    public float minAccForDamage = 5f;
    public float accToDamageFactor = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        playerState = GetComponent<PlayerState>();
        _carHealth = _maxCarHealth;
    }

    public void NewVelocity(Vector3 velocity, float deltaTime)
    {
        acceleration = Vector3.Distance(velocity, oldVelocity) / deltaTime;
        oldVelocity = velocity;
        if (acceleration > minAccForDamage)
        {
            Set_Health(_carHealth - acceleration * accToDamageFactor);
        }
    }

    public void Set_Health(float amount)
    {
        _carHealth = Mathf.Clamp(amount, 0f, _maxCarHealth);
        playerState.CarBroke(_carHealth < 0.1f);
    }
}
