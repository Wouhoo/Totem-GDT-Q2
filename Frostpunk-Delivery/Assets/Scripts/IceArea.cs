using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IceArea : MonoBehaviour
{
    // This script tells the ice manager if you have entered this icey area or if you have left it
    private IceManager iceManager;

    void Start()
    {
        iceManager = GetComponentInParent<IceManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("triggered");
        if (collider.tag == "Player")
            iceManager.InsideIceArea(true);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
            iceManager.InsideIceArea(false);
    }
}
