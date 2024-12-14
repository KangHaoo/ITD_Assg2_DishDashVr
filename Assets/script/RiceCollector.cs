using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceCollector : MonoBehaviour
{
    // The object to unhide inside the bowl
    public GameObject hiddenObject;

    // The tag for the rice object
    public string bowlTag = "DirtyRice";

    // AudioClip to play when the hidden object is revealed
    public AudioClip revealSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object has the correct tag
        if (other.CompareTag(bowlTag))
        {
            // Unhide the hidden object
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
                Debug.Log("Hidden object revealed!");

                // Play reveal sound
                PlayRevealSound();
            }
            else
            {
                Debug.LogWarning("Hidden object is not assigned in the inspector.");
            }
        }
    }

    private void PlayRevealSound()
    {
        if (revealSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(revealSound);
        }
        else
        {
            Debug.LogWarning("Reveal sound or AudioSource is missing.");
        }
    }
}
