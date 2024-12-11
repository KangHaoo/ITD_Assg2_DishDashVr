using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CookingCombination
{
    public List<string> ingredients; // List of ingredient tags
    public GameObject resultPrefab; // Resulting dish prefab
}

public class CookingSystem : MonoBehaviour
{
    [Header("Cooking Configuration")]
    public List<string> validCookingTags; // List to hold valid cooking tags (e.g., "UncookedBeef", "Egg")
    public string fireTag = "Fire"; // Tag for the fire object

    [Header("Cooking Combinations")]
    public List<CookingCombination> cookingCombinations; // List of cooking combinations set in the Inspector

    [Header("Cooking UI")]
    public GameObject cookingUITimer; // UI element to show cooking timer
    public Text timerText; // Text element to display time remaining

    [Header("Cooking Time")]
    public float cookingTime = 15f; // Time in seconds to cook the food

    private List<GameObject> currentIngredients = new List<GameObject>(); // List to hold current cooking ingredient objects
    private bool isCooking = false; // Flag to track cooking state
    private float cookingTimer;
    private Vector3 cookingPosition; // Position to spawn the resulting dish

    void Start()
    {
        if (cookingUITimer != null)
        {
            cookingUITimer.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has a valid cooking tag
        foreach (string tag in validCookingTags)
        {
            if (other.CompareTag(tag))
            {
                currentIngredients.Add(other.gameObject);
                cookingPosition = other.transform.position;
                break;
            }
        }

        // Check if the fire condition is met and start cooking if conditions are satisfied
        if (other.CompareTag(fireTag) && !isCooking && currentIngredients.Count > 0)
        {
            StartCooking();
        }
    }

    void StartCooking()
    {
        isCooking = true;
        cookingTimer = cookingTime;

        if (cookingUITimer != null)
        {
            cookingUITimer.SetActive(true);
        }

        StartCoroutine(CookingCoroutine());
    }

    IEnumerator CookingCoroutine()
    {
        while (cookingTimer > 0)
        {
            cookingTimer -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        FinishCooking();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(cookingTimer).ToString();
        }
    }

    void FinishCooking()
    {
        if (currentIngredients.Count > 0)
        {
            foreach (CookingCombination combination in cookingCombinations)
            {
                if (AreIngredientsMatching(combination.ingredients, currentIngredients))
                {
                    Instantiate(combination.resultPrefab, cookingPosition, Quaternion.identity); // Spawn the resulting dish
                    break;
                }
            }

            // Destroy ingredient objects after cooking
            foreach (GameObject ingredient in currentIngredients)
            {
                Destroy(ingredient);
            }

            currentIngredients.Clear(); // Reset the ingredients list
        }

        if (cookingUITimer != null)
        {
            cookingUITimer.SetActive(false);
        }

        isCooking = false;
    }

    bool AreIngredientsMatching(List<string> requiredIngredients, List<GameObject> providedIngredients)
    {
        if (requiredIngredients.Count != providedIngredients.Count)
            return false;

        List<string> providedTags = new List<string>();
        foreach (GameObject ingredient in providedIngredients)
        {
            providedTags.Add(ingredient.tag);
        }

        foreach (string ingredient in requiredIngredients)
        {
            if (!providedTags.Contains(ingredient))
            {
                return false;
            }
        }

        return true;
    }
}
