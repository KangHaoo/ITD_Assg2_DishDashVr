using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salmonplating : MonoBehaviour
{
    // References to the salmon and rice prefabs to unhide
    public GameObject[] salmons; // Assign Salmon1, Salmon2, Salmon3, Salmon4 in the inspector
    public GameObject[] rice; // Assign Rice1, Rice2, Rice3, Rice4 in the inspector

    // Track how many items have been revealed
    private int salmonIndex = 0;
    private int riceIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the tag "CutSalmon"
        if (other.CompareTag("CutSalmon") && salmonIndex < salmons.Length)
        {
            // Unhide the next salmon object
            salmons[salmonIndex].SetActive(true);
            salmonIndex++;

            // Destroy the triggering object
            Destroy(other.gameObject);
        }

        // Check if the entering object has the tag "CutRice"
        if (other.CompareTag("CutRice") && riceIndex < rice.Length)
        {
            // Unhide the next rice object
            rice[riceIndex].SetActive(true);
            riceIndex++;

            // Destroy the triggering object
            Destroy(other.gameObject);
        }
    }

    private void Start()
    {
        // Ensure all salmon and rice objects are hidden at the start
        foreach (GameObject salmon in salmons)
        {
            salmon.SetActive(false);
        }

        foreach (GameObject rice in rice)
        {
            rice.SetActive(false);
        }
    }
}
