using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject waterPrefab; // Assign the water prefab in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the kettle
        if (other.CompareTag("Kettle"))
        {
            // Unhide the water prefab
            if (waterPrefab != null)
            {
                waterPrefab.SetActive(true);
            }
        }
    }
}
