/// <author>Kang Hao</author>
/// <date>2024-12-11</date>
/// <summary>
/// This script help to manage the cooking of rice within the game
/// </summary>

using System.Collections;
using UnityEngine;
using TMPro;

public class RiceCooker : MonoBehaviour
{
    public GameObject cookedRicePrefab; // Assign the cooked rice prefab in the inspector
    public Transform spawnPoint; // Assign the spawn point (center of the rice cooker) in the inspector
    public Canvas cookCanvas; // Assign the canvas with the cook button in the inspector
    public TextMeshProUGUI progressText; // Assign the TextMeshProUGUI element in the inspector
    public AudioSource cookingCompleteAudio; // Assign the audio source in the inspector

    public float cookingDuration = 5f; // Duration in seconds for cooking

    private bool hasUncookedRice = false;
    private bool isCooking = false;

    private void Start()
    {
        // Ensure the canvas and progress text are initially hidden
        if (cookCanvas != null)
        {
            cookCanvas.gameObject.SetActive(false);
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for "UncookedRice" tag
        if (other.CompareTag("UncookedRice"))
        {
            hasUncookedRice = true;
        }

        // Show the canvas if rice is present
        UpdateCanvasVisibility();
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the flag if the rice is removed
        if (other.CompareTag("UncookedRice"))
        {
            hasUncookedRice = false;
        }

        // Hide the canvas if rice is missing
        UpdateCanvasVisibility();
    }

    private void UpdateCanvasVisibility()
    {
        if (cookCanvas != null)
        {
            cookCanvas.gameObject.SetActive(hasUncookedRice && !isCooking);
        }
    }

    public void CookRice()
    {
        // Ensure rice is present before cooking
        if (hasUncookedRice && !isCooking)
        {
            StartCoroutine(CookRiceRoutine());
        }
        else
        {
            Debug.LogWarning("Cannot cook rice. Ensure uncooked rice is present.");
        }
    }

    private IEnumerator CookRiceRoutine()
    {
        isCooking = true;

        // Hide the canvas and show the progress text
        if (cookCanvas != null)
        {
            cookCanvas.gameObject.SetActive(false);
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
        }

        float elapsedTime = 0f;

        while (elapsedTime < cookingDuration)
        {
            elapsedTime += Time.deltaTime;

            // Update progress text
            if (progressText != null)
            {
                float progress = Mathf.Clamp01(elapsedTime / cookingDuration) * 100;
                progressText.text = $"Cooking... {progress:F0}%";
            }

            yield return null;
        }

        // Spawn cooked rice prefab at the specified spawn point
        Instantiate(cookedRicePrefab, spawnPoint.position, Quaternion.identity);

        // Find and destroy the uncooked rice objects
        foreach (Transform child in transform)
        {
            if (child.CompareTag("UncookedRice"))
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log("Rice is cooked!");

        // Play audio after cooking
        if (cookingCompleteAudio != null)
        {
            cookingCompleteAudio.Play();
        }

        // Hide the progress text after cooking
        if (progressText != null)
        {
            progressText.gameObject.SetActive(false);
        }

        isCooking = false;

        // Update the canvas visibility in case new ingredients are added
        UpdateCanvasVisibility();
    }
}
