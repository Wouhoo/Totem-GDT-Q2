using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IceArea : MonoBehaviour
{
    // Script for handling ice areas.
    // --- Now repurposed to handle any area with different friction; default friction is offroad friction
    // Tells the ice manager if you are in the ice area (so it reduces friction accordingly),
    // and also makes the area grow over time and shrink when hit by a flamethrower hitbox.

    private IceManager iceManager;
    public bool dynamicSize = true; // size can grow / shrink (ie. most stuff except river)
    private float startDelay = 5.0f;        // Seconds until the ice areas start growing (give the player some time to get used to the game)
    private float growthDelay = 1.0f;       // Seconds between growth ticks
    private float growthMultiplier = 1.01f; // Growth multiplier in each tick (should be small enough so that the growth in one tick is not noticeable)
    // ^ Currently these are test values so the growth is very noticeable. SHOULD BE REDUCED/TWEAKED IN THE FINAL GAME
    private float meltMultiplier = 0.7f;    // How quickly the ice melts when exposed to a flame

    void Start()
    {
        iceManager = GetComponentInParent<IceManager>();
        InvokeRepeating("IncreaseSize", startDelay, growthDelay); // Start ice growth process
    }

    void IncreaseSize()
    {
        if (!dynamicSize)
            return;

        // Multiplies the area's x and z scale by the provided multiplier.
        transform.localScale = new Vector3(transform.localScale.x * growthMultiplier, transform.localScale.y, transform.localScale.z * growthMultiplier);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!dynamicSize)
            return;

        //Debug.Log(string.Format("Colliding with collider {0}", other.name));
        // Melt ice while touching a flame 
        if (other.CompareTag("Flame"))
        {
            transform.localScale = new Vector3(transform.localScale.x * (1 - meltMultiplier * Time.deltaTime), transform.localScale.y, transform.localScale.z * (1 - meltMultiplier * Time.deltaTime));
        }
    }
}
