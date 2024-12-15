/// <author>karlyn</author>
/// <date>2024-12-8</date>
/// <summary>
/// This script help to manage the cutting of salmon within the game
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalmonCutting : MonoBehaviour
{
    public GameObject uncutLayer;             // Reference to the uncut salmon layer
    public GameObject firstCutLayer;          // Reference to the first cut salmon layer
    public GameObject secondCutLayer;         // Reference to the second cut salmon layer
    public GameObject thirdCutLayer;          // Reference to the third cut salmon layer
    public GameObject fullyCutLayer;          // Reference to the fully cut salmon layer
    public AudioClip cuttingSound;            // Sound to play when cutting

    private AudioSource audioSource;          // Reference to the AudioSource component
    private int cutState = 0; // 0 = uncut, 1 = first cut, 2 = second cut, 3 = third cut, 4 = fully cut
    private bool canCut = true; // Flag to check if the knife can cut again
    private float cooldownTime = 2f; // Cooldown time in seconds

    private void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on this GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the knife hits the salmon and the cooldown allows it
        if (other.CompareTag("Knife") && canCut)
        {
            // Start the cutting process based on the current cutState
            StartCutting();

            // Play cutting sound
            PlayCuttingSound();

            // Start the cooldown timer
            canCut = false;
            Invoke("ResetCutting", cooldownTime); // Reset canCut flag after cooldown
        }
    }

    private void StartCutting()
    {
        // Change the state of the salmon based on the current cutState
        switch (cutState)
        {
            case 0: // From uncut to first cut
                uncutLayer.SetActive(false);         // Hide uncut salmon layer
                firstCutLayer.SetActive(true);       // Show first cut salmon layer
                cutState = 1;                        // Update state
                break;
            case 1: // From first cut to second cut
                firstCutLayer.SetActive(false);      // Hide first cut salmon layer
                secondCutLayer.SetActive(true);      // Show second cut salmon layer
                cutState = 2;                        // Update state
                break;
            case 2: // From second cut to third cut
                secondCutLayer.SetActive(false);     // Hide second cut salmon layer
                thirdCutLayer.SetActive(true);       // Show third cut salmon layer
                cutState = 3;                        // Update state
                break;
            case 3: // From third cut to fully cut
                thirdCutLayer.SetActive(false);      // Hide third cut salmon layer
                fullyCutLayer.SetActive(true);       // Show fully cut salmon layer
                cutState = 4;                        // Update state
                break;
                // Fully cut is the last state; no further action required.
        }
    }

    private void PlayCuttingSound()
    {
        // Play the cutting sound if an audio source and clip are available
        if (audioSource != null && cuttingSound != null)
        {
            audioSource.PlayOneShot(cuttingSound);
        }
    }

    private void ResetCutting()
    {
        // Reset the canCut flag to allow the player to cut again after the cooldown
        canCut = true;
    }
}
