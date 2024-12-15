/// <author>Karlyn</author>
/// <date>2024-12-8</date>
/// <summary>
/// This script manage the plating system within.
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeefPlating : MonoBehaviour
{
    // References to the cooked beef, broccoli, baked potato, and burnt dish prefabs
    public GameObject cookedBeef; // Assign the single cooked beef prefab in the Inspector
    public GameObject[] broccolis; // Assign Broccoli1, Broccoli2, Broccoli3, Broccoli4 in the Inspector
    public GameObject bakedPotato; // Assign the baked potato prefab in the Inspector
    public GameObject burntDish; // Assign the burnt dish prefab in the Inspector

    // Track how many broccolis have been revealed
    private int broccoliIndex = 0;

    // A flag to track if the dish has been burnt
    private bool isBurnt = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the tag "BurntDish"
        if (other.CompareTag("Burnfood"))
        {
            // If a burnt dish is placed, hide all good plating and show the burnt dish
            HideGoodPlating();
            burntDish.SetActive(true);
            isBurnt = true;

            // Destroy the triggering object
            Destroy(other.gameObject);
            return; // Exit to prevent further interactions
        }

        // Ignore further interactions if the dish is burnt
        if (isBurnt) return;

        // Check if the entering object has the tag "CookedBeef"
        if (other.CompareTag("Cookedbeef") && !cookedBeef.activeSelf)
        {
            // Unhide the cooked beef
            cookedBeef.SetActive(true);

            // Destroy the triggering object
            Destroy(other.gameObject);
        }

        // Check if the entering object has the tag "CookedBroccoli"
        if (other.CompareTag("CookedBroccoli") && broccoliIndex < broccolis.Length)
        {
            // Unhide the next broccoli object
            broccolis[broccoliIndex].SetActive(true);
            broccoliIndex++;

            // Destroy the triggering object
            Destroy(other.gameObject);
        }

        // Check if the entering object has the tag "BakedPotato"
        if (other.CompareTag("BakePotato") && !bakedPotato.activeSelf)
        {
            // Unhide the baked potato
            bakedPotato.SetActive(true);

            // Destroy the triggering object
            Destroy(other.gameObject);
        }
    }

    private void Start()
    {
        // Ensure all objects are hidden at the start
        cookedBeef.SetActive(false);
        bakedPotato.SetActive(false);
        burntDish.SetActive(false);

        foreach (GameObject broccoli in broccolis)
        {
            broccoli.SetActive(false);
        }
    }

    // Hides all good plating objects
    private void HideGoodPlating()
    {
        cookedBeef.SetActive(false);
        bakedPotato.SetActive(false);

        foreach (GameObject broccoli in broccolis)
        {
            broccoli.SetActive(false);
        }
    }
}
