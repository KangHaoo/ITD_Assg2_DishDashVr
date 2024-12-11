using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceCooker : MonoBehaviour
{
    public GameObject cookedRicePrefab; // Assign the cooked rice prefab in the inspector
    public Transform spawnPoint; // Assign the spawn point (center of the rice cooker) in the inspector
    public Canvas cookCanvas; // Assign the canvas with the cook button in the inspector

    private bool hasWater = false;  
    private bool hasUncookedRice = false;

    private void Start()
    {
        // Ensure the canvas is initially hidden
        if (cookCanvas != null)
        {
            cookCanvas.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for "Water" and "UncookedRice" tags
        if (other.CompareTag("Water"))
        {
            hasWater = true;
        }
        else if (other.CompareTag("UncookedRice"))
        {
            hasUncookedRice = true;
        }

        // Show the canvas if both ingredients are present
        UpdateCanvasVisibility();
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the flags if the items are removed
        if (other.CompareTag("Water"))
        {
            hasWater = false;
        }
        else if (other.CompareTag("UncookedRice"))
        {
            hasUncookedRice = false;
        }

        // Hide the canvas if either ingredient is missing
        UpdateCanvasVisibility();
    }

    private void UpdateCanvasVisibility()
    {
        if (cookCanvas != null)
        {
            cookCanvas.gameObject.SetActive(hasWater && hasUncookedRice);
        }
    }

    public void CookRice()
    {
        // Ensure both items are present before cooking
        if (hasWater && hasUncookedRice)
        {
            // Spawn cooked rice prefab at the specified spawn point
            Instantiate(cookedRicePrefab, spawnPoint.position, Quaternion.identity);

            // Find and destroy the water and uncooked rice objects
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Water") || child.CompareTag("UncookedRice"))
                {
                    Destroy(child.gameObject);
                }
            }

            Debug.Log("Rice is cooked!");

            // Hide the canvas after cooking
            if (cookCanvas != null)
            {
                cookCanvas.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Cannot cook rice. Ensure both water and uncooked rice are present.");
        }
    }
}
