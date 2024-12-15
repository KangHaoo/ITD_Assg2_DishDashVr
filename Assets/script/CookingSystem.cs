/// <author>Kang Hao</author>
/// <date>2024-12-7</date>
/// <summary>
/// This script help to manage the cooking system.
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Added for TextMeshPro
using Firebase.Database;

[System.Serializable]
public class CookingCombination
{
    public List<string> ingredients; // List of ingredient tags
    public GameObject resultPrefab; // Resulting dish prefab
    public float cookingTime = 15f; // Time to cook this combination
    public GameObject burnedFoodPrefab; // Burned food prefab if overcooked
    public float burnTime = 20f; // Time after which the food burns
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
    public TextMeshProUGUI timerText; // TextMeshProUGUI element to display time remaining

    private List<GameObject> currentIngredients = new List<GameObject>(); // List to hold current cooking ingredient objects
    private bool isCooking = false; // Flag to track cooking state
    private float cookingTimer;
    private CookingCombination currentCombination;
    private Vector3 cookingPosition; // Position to spawn the resulting dish
    private bool foodCooked = false; // Flag to track if food is cooked
    private AuthManager authManager;

    void Start()
    {
        if (cookingUITimer != null)
        {
            cookingUITimer.SetActive(false);
        }
        authManager = FindObjectOfType<AuthManager>(); // Find AuthManager to update Firebase
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

    void OnTriggerExit(Collider other)
    {
        // Check if the fire or any ingredient leaves the cooking area
        if (other.CompareTag(fireTag) || currentIngredients.Contains(other.gameObject))
        {
            ResetCooking();
        }
    }

    void ResetCooking()
    {
        // Stop all cooking processes
        StopAllCoroutines();

        // Reset cooking state and UI
        isCooking = false;
        foodCooked = false;
        currentIngredients.Clear();

        if (cookingUITimer != null)
        {
            cookingUITimer.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.text = "0"; // Reset timer UI
        }

        currentCombination = null;
        Debug.Log("Cooking has been reset.");
    }


    void StartCooking()
    {
        isCooking = true;
        foodCooked = false;
        currentCombination = GetMatchingCombination();

        if (currentCombination != null)
        {
            cookingTimer = 0f; // Start timer at 0

            if (cookingUITimer != null)
            {
                cookingUITimer.SetActive(true);
            }

            StartCoroutine(CookingCoroutine());
        }
        else
        {
            Debug.LogWarning("No matching combination found for the ingredients.");
            isCooking = false;
        }
    }

    IEnumerator CookingCoroutine()
    {
        while (true)
        {
            cookingTimer += Time.deltaTime;
            UpdateTimerUI();

            if (!foodCooked && cookingTimer >= currentCombination.cookingTime)
            {
                FinishCooking(false); // Successfully cooked food
                foodCooked = true; // Mark as cooked but continue tracking time
            }

            if (cookingTimer >= currentCombination.burnTime)
            {
                FinishCooking(true); // Burned food
                yield break;
            }

            yield return null;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.FloorToInt(cookingTimer).ToString();
        }
    }

    void FinishCooking(bool isBurned)
    {
        if (currentCombination != null && currentIngredients.Count > 0)
        {
            GameObject spawnPrefab = isBurned ? currentCombination.burnedFoodPrefab : currentCombination.resultPrefab;

            if (spawnPrefab != null)
            {
                Instantiate(spawnPrefab, cookingPosition, Quaternion.identity); // Spawn the resulting dish
            }

            if (isBurned || !foodCooked)
            {
                // Destroy ingredient objects only when cooking completes
                foreach (GameObject ingredient in currentIngredients)
                {
                    Destroy(ingredient);
                }

                currentIngredients.Clear(); // Reset the ingredients list
            }
        }

        if (isBurned && cookingUITimer != null)
        {
            cookingUITimer.SetActive(false);
        }

        if (isBurned)
        {
            isCooking = false;
            UpdateBurntFoodInFirebase(); // Update Firebase when food is burnt
        }
    }

    CookingCombination GetMatchingCombination()
    {
        foreach (CookingCombination combination in cookingCombinations)
        {
            if (AreIngredientsMatching(combination.ingredients, currentIngredients))
            {
                return combination;
            }
        }

        return null;
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

    // Update the burnt food count in Firebase
    private async void UpdateBurntFoodInFirebase()
    {
        if (authManager.User != null)
        {
            string userId = authManager.User.UserId;
            DatabaseReference userStatsRef = FirebaseDatabase.DefaultInstance.RootReference.Child("Players").Child(userId).Child("stat");

            try
            {
                // Get the current stats of the user
                DataSnapshot snapshot = await userStatsRef.GetValueAsync();
                int burntDishes = 0;

                if (snapshot.Exists && snapshot.HasChild("burnt_dishes"))
                {
                    // Safely try to get the burnt_dishes value
                    var burntDishesValue = snapshot.Child("burnt_dishes").Value;

                    if (burntDishesValue is long)
                    {
                        burntDishes = (int)(long)burntDishesValue; // Cast from long to int
                    }
                    else if (burntDishesValue is double)
                    {
                        burntDishes = (int)(double)burntDishesValue; // Cast from double to int
                    }
                    else if (burntDishesValue is int)
                    {
                        burntDishes = (int)burntDishesValue; // Direct cast if it's already an int
                    }
                    else
                    {
                        Debug.LogWarning("Unexpected type for 'burnt_dishes' in Firebase. Initializing to 0.");
                    }
                }

                // Increment the burnt dishes count
                burntDishes += 1;

                // Update the burnt dishes count in Firebase
                await userStatsRef.Child("burnt_dishes").SetValueAsync(burntDishes);
                Debug.Log("Burnt dishes count updated to " + burntDishes);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to update burnt dishes in Firebase: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("User not authenticated.");
        }
    }

}
