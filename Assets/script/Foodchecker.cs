using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For XR button
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class FoodChecker : MonoBehaviour
{
    public Button xrButton; // Assign the XR button in the Unity Inspector

    private bool goodDishPresent = false;
    private bool badDishPresent = false;

    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    private int goodDishesMade = 0;
    private int badDishesMade = 0;

    void Start()
    {
        // Initialize Firebase
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (auth.CurrentUser != null)
        {
            Debug.Log("Firebase initialized, user ID: " + auth.CurrentUser.UserId);
        }
        else
        {
            Debug.LogWarning("Firebase not initialized: User is null or not authenticated.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoodDish"))
        {
            goodDishPresent = true;
            Debug.Log("Good dish detected.");
        }
        else if (other.CompareTag("BadDish"))
        {
            badDishPresent = true;
            Debug.Log("Bad dish detected.");
        }

        CheckDishes();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GoodDish"))
        {
            goodDishPresent = false;
            Debug.Log("Good dish removed.");
        }
        else if (other.CompareTag("BadDish"))
        {
            badDishPresent = false;
            Debug.Log("Bad dish removed.");
        }
    }

    private void CheckDishes()
    {
        if (goodDishPresent || badDishPresent)
        {
            Debug.Log("Dish detected. You can trigger the UpdateStats function.");
        }
    }

    public void UpdateStats()
    {
        if (goodDishPresent)
        {
            goodDishesMade++;
            Debug.Log("Good dish count incremented.");
        }

        if (badDishPresent)
        {
            badDishesMade++;
            Debug.Log("Bad dish count incremented.");
        }

        StartCoroutine(UpdateStatsInFirebase());
    }

    private IEnumerator UpdateStatsInFirebase()
    {
        string userId = auth.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("User is not authenticated!");
            yield break;
        }

        var statsData = new Dictionary<string, object>
        {
            { "bad_dishes_made", badDishesMade },
            { "good_dishes_made", goodDishesMade }
        };

        Debug.Log("Attempting to update Firebase stats...");

        var updateTask = dbReference.Child("Players").Child(userId).Child("stat").UpdateChildrenAsync(statsData);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception != null)
        {
            Debug.LogError($"Failed to update stats: {updateTask.Exception}");
        }
        else
        {
            Debug.Log("Stats updated successfully in Firebase.");
        }
    }
}
