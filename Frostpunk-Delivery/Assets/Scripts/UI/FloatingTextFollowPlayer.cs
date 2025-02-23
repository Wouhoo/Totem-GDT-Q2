using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingTextFollowPlayer : MonoBehaviour
{
    // Script for floating text that follows the player (i.e. the minimap arrow)
    Transform worldSpaceCanvas;
    Transform playerTransform;
    Vector3 offset = new Vector3(0, 10, 0);

    private void Start()
    {
        playerTransform = transform.parent; // Note: arrow needs to be child of player gameObject in the editor
        worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas").transform;

        transform.SetParent(worldSpaceCanvas); // The text object is shown as a child of the anchorObject in the editor,
                                               // but should be made a child of the canvas at runtime so it actually renders.

    }
    private void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, 10f, playerTransform.position.z); // Make arrow follow player
        transform.eulerAngles = new Vector3(0, playerTransform.rotation.eulerAngles.y, 0);
    }
}
