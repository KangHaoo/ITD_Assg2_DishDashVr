/// <author>Daanish</author>
/// <date>2024-12-13</date>
/// <summary>
/// This script help to manage the cutting function
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeefCutting : MonoBehaviour
{
    public GameObject uncutLayer;        // Reference to the uncut beef layer
    public GameObject halfCutLayer;      // Reference to the half-cut beef layer
    public GameObject threeQuarterCutLayer; // Reference to the three-quarters cut beef layer
    public GameObject fullyCutLayer;    // Reference to the fully-cut beef layer
    public AudioClip cuttingSound;      // Sound to play when cutting

    private AudioSource audioSource;    // Reference to the AudioSource component
    private int cutState = 0; // 0 = uncut, 1 = half-cut, 2 = three-quarters cut, 3 = fully-cut
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
        // Check if the knife hits the beef and the cooldown allows it
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
        // Change the state of the beef based on the current cutState
        switch (cutState)
        {
            case 0: // From uncut to half-cut
                uncutLayer.SetActive(false);          // Hide uncut beef layer
                halfCutLayer.SetActive(true);         // Show half-cut beef layer
                cutState = 1;                         // Update state
                break;
            case 1: // From half-cut to three-quarters cut
                halfCutLayer.SetActive(false);        // Hide half-cut beef layer
                threeQuarterCutLayer.SetActive(true); // Show three-quarters cut beef layer
                cutState = 2;                         // Update state
                break;
            case 2: // From three-quarters cut to fully cut
                threeQuarterCutLayer.SetActive(false); // Hide three-quarters cut beef layer
                fullyCutLayer.SetActive(true);         // Show fully-cut beef layer
                cutState = 3;                          // Update state
                break;
                // No need for further cases, as fully-cut is the last state.
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
