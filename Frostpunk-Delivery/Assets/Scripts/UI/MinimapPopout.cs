using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPopout : MonoBehaviour
{
    // Script for making the minimap larger while holding tab
    [SerializeField] Vector3 normalScale = new Vector3(1, 1, 1);
    [SerializeField] Vector3 enlargedScale = new Vector3(1.5f, 1.5f, 1.5f);

    private void Update()
    {
        // Enlarge when tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.localScale = enlargedScale;
        }
        // Go back to normal when tab is released
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            transform.localScale = normalScale;
        }
    }
}
