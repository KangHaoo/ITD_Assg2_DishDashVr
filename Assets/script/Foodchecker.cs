using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Assuming XR button uses Unity UI Button
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

        // Ensure XR button is assigned
        if (xrButton != null)
        {
            xrButton.onClick.AddListener(UpdateStats);
        }
        else
        {
            Debug.LogWarning("XR Button is not assigned!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoodDish"))
        {
            goodDishPresent = true;
        }
        else if (other.CompareTag("BadDish"))
        {
            badDishPresent = true;
        }

        CheckDishes();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GoodDish"))
        {
            goodDishPresent = false;
        }
        else if (other.CompareTag("BadDish"))
        {
            badDishPresent = false;
        }
    }

    private void CheckDishes()
    {
        if (goodDishPresent && badDishPresent)
        {
            Debug.Log("Both dishes are present. You can press the XR button.");
        }
    }

    public void UpdateStats()
    {
        if (goodDishPresent)
        {
            goodDishesMade++;
        }

        if (badDishPresent)
        {
            badDishesMade++;
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

        var updateTask = dbReference.Child("Players").Child(userId).Child("stat").UpdateChildrenAsync(statsData);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception != null)
        {
            Debug.LogWarning($"Failed to update stats: {updateTask.Exception}");
        }
        else
        {
            Debug.Log("Stats updated successfully in Firebase.");
        }
    }
}
