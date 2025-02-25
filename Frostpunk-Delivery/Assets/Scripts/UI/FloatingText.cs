using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingText : MonoBehaviour
{
    // Script for floating text (e.g. showing the amount of fuel to deliver or time left at a delivery point)
    Transform worldSpaceCanvas;

    private void Start()
    {
        worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas").transform;

        transform.SetParent(worldSpaceCanvas); // The text object is shown as a child of the anchorObject in the editor,
                                               // but should be made a child of the canvas at runtime so it actually renders.
    }
}
