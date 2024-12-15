/// <author>karlyn</author>
/// <date>2024-12-10</date>
/// <summary>
/// This script help to manage make ricemold for sushi
/// </summary>



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceMold : MonoBehaviour
{
    // Assign these in the Unity Editor
    public GameObject ricePrefab; // The rice prefab to spawn
    public Transform spawnPoint;  // The specific location for spawning the prefab

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the "Rice" tag
        if (other.CompareTag("Rice"))
        {
            // Use the position of the spawnPoint if assigned, otherwise default to the mold's position
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

            // Spawn the rice prefab at the determined position
            Instantiate(ricePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
