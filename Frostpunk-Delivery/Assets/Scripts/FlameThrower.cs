using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    // Script for handling the player's flamethrower
    [SerializeField] GameObject flamethrowerCone;
    private KeyCode flamethrowerButton = KeyCode.Space;

    void Start()
    {
        flamethrowerCone.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Activate flamethrower when space is pressed & deactivate when released
        if (Input.GetKeyDown(flamethrowerButton))
        {
            flamethrowerCone.SetActive(true);
        }
        if (Input.GetKeyUp(flamethrowerButton))
        {
            flamethrowerCone.SetActive(false);
        }
    }
}
