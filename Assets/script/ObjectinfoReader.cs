using System.Collections.Generic;
using UnityEngine;

public class ObjectInfoReader : MonoBehaviour
{
    [Header("Objects in Trigger Zone")]
    [Tooltip("List of tags currently in the trigger zone.")]
    public List<string> tagsInTrigger = new List<string>();

    [Header("Canvas Settings")]
    [Tooltip("Canvas that will be shown/hidden.")]
    public GameObject canvas; // Reference to the canvas

    [Header("Prefab Settings")]
    [Tooltip("Prefab to spawn when conditions are met.")]
    public GameObject meatPrefab; // Reference to the prefab
    [Tooltip("Spawn location for the meat prefab.")]
    public Transform spawnPoint; // Location to spawn the prefab

    private void Start()
    {
        // Ensure the canvas is initially hidden
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore objects with the tag "CookingObject"
        if (other.CompareTag("CookingObject"))
        {
            return;
        }

        // Get the object's tag
        string objectTag = other.gameObject.tag;

        // Add to the list if not already present
        if (!tagsInTrigger.Contains(objectTag))
        {
            tagsInTrigger.Add(objectTag);
            Debug.Log($"Object Tag Entered Trigger: {objectTag}");

            // Show the canvas when the first object enters
            if (canvas != null && tagsInTrigger.Count == 1)
            {
                canvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Ignore objects with the tag "CookingObject"
        if (other.CompareTag("CookingObject"))
        {
            return;
        }

        // Get the object's tag
        string objectTag = other.gameObject.tag;

        // Remove the tag from the list when it leaves the trigger zone
        if (tagsInTrigger.Contains(objectTag))
        {
            tagsInTrigger.Remove(objectTag);
            Debug.Log($"Object Tag Exited Trigger: {objectTag}");

            // Hide the canvas when no objects remain in the trigger
            if (canvas != null && tagsInTrigger.Count == 0)
            {
                canvas.SetActive(false);
            }
        }
    }

    // Call this function via a UI button
    public void TrySpawnMeat()
    {
        // Check if both "meat" and "parsley" are in the trigger zone
        if (tagsInTrigger.Contains("Beef") && tagsInTrigger.Contains("Parsley"))
        {
            // Check if the meat prefab and spawn point are assigned
            if (meatPrefab != null && spawnPoint != null)
            {
                Instantiate(meatPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Meat prefab spawned!");
            }
            else
            {
                Debug.LogWarning("Meat prefab or spawn point is not assigned!");
            }
        }
        else
        {
            Debug.Log("Conditions not met: 'Beef' and 'Parsley' are not both in the trigger.");
        }
    }
}
