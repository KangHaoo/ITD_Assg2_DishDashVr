/// <author>Daanish</author>
/// <date>2024-06-27</date>
/// <summary>
/// This script help to manage the cutting function
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroccoliCutting : MonoBehaviour
{
    public GameObject uncutLayer;             // Reference to the uncut broccoli layer
    public GameObject firstCutLayer;          // Reference to the first cut broccoli layer
    public GameObject secondCutLayer;         // Reference to the second cut broccoli layer
    public GameObject thirdCutLayer;          // Reference to the third cut broccoli layer
    public GameObject fullyCutLayer;          // Reference to the fully cut broccoli layer

    public AudioClip cuttingSound;            // Reference to the cutting sound clip
    private AudioSource audioSource;          // Reference to the audio source

    private int cutState = 0;                 // 0 = uncut, 1 = first cut, 2 = second cut, 3 = third cut, 4 = fully cut
    private bool canCut = true;               // Flag to check if the knife can cut again
    private float cooldownTime = 2f;          // Cooldown time in seconds

    private void Start()
    {
        // Initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the cutting sound to the audio source if not set already
        audioSource.clip = cuttingSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the knife hits the broccoli and the cooldown allows it
        if (other.CompareTag("knife") && canCut)
        {
            // Start the cutting process based on the current cutState
            StartCutting();

            // Play the cutting sound
            PlayCuttingSound();

            // Start the cooldown timer
            canCut = false;
            Invoke("ResetCutting", cooldownTime); // Reset canCut flag after cooldown
        }
    }

    private void StartCutting()
    {
        // Change the state of the broccoli based on the current cutState
        switch (cutState)
        {
            case 0: // From uncut to first cut
                uncutLayer.SetActive(false);         // Hide uncut broccoli layer
                firstCutLayer.SetActive(true);       // Show first cut broccoli layer
                cutState = 1;                        // Update state
                break;
            case 1: // From first cut to second cut
                firstCutLayer.SetActive(false);      // Hide first cut broccoli layer
                secondCutLayer.SetActive(true);      // Show second cut broccoli layer
                cutState = 2;                        // Update state
                break;
            case 2: // From second cut to third cut
                secondCutLayer.SetActive(false);     // Hide second cut broccoli layer
                thirdCutLayer.SetActive(true);       // Show third cut broccoli layer
                cutState = 3;                        // Update state
                break;
            case 3: // From third cut to fully cut
                thirdCutLayer.SetActive(false);      // Hide third cut broccoli layer
                fullyCutLayer.SetActive(true);       // Show fully cut broccoli layer
                cutState = 4;                        // Update state
                break;
                // Fully cut is the last state; no further action required.
        }
    }

    private void ResetCutting()
    {
        // Reset the canCut flag to allow the player to cut again after the cooldown
        canCut = true;
    }

    private void PlayCuttingSound()
    {
        if (audioSource != null && cuttingSound != null)
        {
            audioSource.Play();
        }
    }
}
