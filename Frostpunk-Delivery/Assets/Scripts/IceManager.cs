using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceManager : MonoBehaviour
{
    public float iceFrictionCoef = 0.3f; // land friction is 0.8
    public PlayerController playerController;

    public float iceAreaCounter = 0f;

    void Start()
    {

    }

    public void InsideIceArea(bool isInside) // true if entered, false if exited
    {
        // this function keeps track of which ice areas we are in and changes the players friction

        if (isInside)
        {
            if (iceAreaCounter == 0)
                playerController.SetFriction(iceFrictionCoef);
            iceAreaCounter += 1;
        }
        else
        {
            iceAreaCounter -= 1;
            if (iceAreaCounter == 0)
                playerController.ResetFriction();
        }
    }
}
