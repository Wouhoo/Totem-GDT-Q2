using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private CarController carController;
    private GameManager gameManager;

    public float _freezeTime = 6f;
    private float _deathTimer = 0f;
    private bool _noFuel = false;
    private bool _carBroke = false;

    private bool _dying = false;


    void Awake()
    {
        carController = GetComponent<CarController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void CarBroke(bool state)
    {
        _carBroke = state;
        UpdateDyingState();
        //Debug.Log("car broke");
        //Debug.Log(state);
    }

    public void NoFuel(bool state)
    {
        _noFuel = state;
        UpdateDyingState();
        //Debug.Log("no fule");
        //Debug.Log(state);
    }

    private void UpdateDyingState()
    {
        _dying = (_noFuel || _carBroke);
        if (!_dying)
            _deathTimer = 0f;
        carController.canDrive = !_dying;
    }

    // We see if we are freezing to death and if so we update that time
    void Update()
    {
        if (_noFuel || _carBroke)
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer > _freezeTime)
                gameManager.GameOver();
        }
    }
}
