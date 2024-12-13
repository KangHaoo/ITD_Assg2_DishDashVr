using UnityEngine;

public class VegetableWasher : MonoBehaviour
{
    // List of allowed tags that can trigger the canvas
    public string[] allowedTags;
    public GameObject washerCanvas;

    // Pairs of dirty and clean objects
    [System.Serializable]
    public class VegetablePair
    {
        public GameObject dirtyVegetable;
        public GameObject cleanVegetable;
    }

    public VegetablePair[] vegetablePairs;

    private GameObject currentDirtyVegetable;

    private void Start()
    {
        // Ensure the canvas starts as inactive
        washerCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's tag is in the allowed list
        foreach (string tag in allowedTags)
        {
            if (other.CompareTag(tag))
            {
                currentDirtyVegetable = other.gameObject;
                washerCanvas.SetActive(true);
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Deactivate the canvas if the object leaves
        if (currentDirtyVegetable == other.gameObject)
        {
            currentDirtyVegetable = null;
            washerCanvas.SetActive(false);
        }
    }

    // Method to clean the vegetable
    public void CleanVegetable()
    {
        if (currentDirtyVegetable == null) return;

        // Find the pair for the current dirty vegetable
        foreach (var pair in vegetablePairs)
        {
            if (pair.dirtyVegetable == currentDirtyVegetable)
            {
                // Hide the dirty vegetable and show the clean vegetable
                pair.dirtyVegetable.SetActive(false);
                pair.cleanVegetable.SetActive(true);

                // Deactivate the canvas
                washerCanvas.SetActive(false);
                currentDirtyVegetable = null;
                break;
            }
        }
    }
}
