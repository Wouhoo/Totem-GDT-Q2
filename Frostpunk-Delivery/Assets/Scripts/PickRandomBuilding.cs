using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickRandomBuilding : MonoBehaviour
{
    // Script that makes buildings choose a random building variant upon instantiation
    [SerializeField] GameObject[] buildings;


    void Start()
    {
        // Destroy placeholder
        GameObject placeholder = transform.GetChild(0).gameObject;
        Destroy(placeholder);
        // Instantiate random house
        GameObject selectedBuilding = buildings[Random.Range(0, buildings.Length)];
        Instantiate(selectedBuilding, transform);
    }
}
