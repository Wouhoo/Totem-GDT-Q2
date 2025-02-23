using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingText : MonoBehaviour
{
    // Script for floating text (e.g. showing the amount of fuel to deliver or time left at a delivery point)
    // The stuff that's commented out is for rendering the text in a way that *should* look good on the main camera;
    // however, with a camera "from above" like we currently have, the rotations just look hella off.
    // Still, I'm keeping it in for now in case we switch to a camera that is at more or less the same height as the text.

    //Transform minimapCamera;
    //Transform anchorObject; // gameObject that this text should hover above (usually a delivery point)
    Transform worldSpaceCanvas;
    //[SerializeField] Vector3 offset;

    private void Start()
    {
        //minimapCamera = GameObject.Find("MinimapCamera").GetComponent<Transform>();
        //anchorObject = transform.parent;
        worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas").transform;

        transform.SetParent(worldSpaceCanvas); // The text object is shown as a child of the anchorObject in the editor,
                                               // but should be made a child of the canvas at runtime so it actually renders.
    }

    /*
    private void Update()
    {
        transform.position = anchorObject.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - minimapCamera.transform.position); // Face towards main camera
    } */
}
