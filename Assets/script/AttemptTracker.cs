/// <author>Kang Hao</author>
/// <date>2024-12-9</date>
/// <summary>
/// This script help to manager the total attemptTracked within the firebase
/// </summary>


using System.Collections;
using UnityEngine;
using Firebase.Database;

public class AttemptTracker : MonoBehaviour
{
    private AuthManager authManager; // Reference to AuthManager

    void Start()
    {
        // Get the AuthManager instance in the scene
        authManager = FindObjectOfType<AuthManager>();

        if (authManager == null)
        {
            Debug.LogError("AuthManager not found in the scene.");
        }
    }

    public void OnButtonPressed()
    {
        if (authManager != null && authManager.User != null)
        {
            string uid = authManager.User.UserId;

            if (!string.IsNullOrEmpty(uid))
            {
                Debug.Log($"Button pressed by user ID: {uid}");
                UpdateTotalAttempts(uid);
            }
            else
            {
                Debug.LogWarning("Current user does not have a valid UID.");
            }
        }
        else
        {
            Debug.LogWarning("No user is currently logged in or AuthManager is not initialized.");
        }
    }

    private void UpdateTotalAttempts(string uid)
    {
        // Access the database reference from AuthManager
        DatabaseReference dbReference = authManager.dbReference;
        DatabaseReference userStatRef = dbReference.Child("Players").Child(uid).Child("stat").Child("total_attempts_made");

        // Log the database path
        Debug.Log($"Updating total_attempts_made at path: Players/{uid}/stat/total_attempts_made");

        // Increment total attempts atomically
        userStatRef.RunTransaction(mutableData =>
        {
            if (mutableData.Value == null)
            {
                Debug.Log("total_attempts_made not found. Initializing to 1.");
                mutableData.Value = 1; // Initialize if it doesn't exist
            }
            else
            {
                Debug.Log($"Current total_attempts_made: {mutableData.Value}");
                mutableData.Value = System.Convert.ToInt64(mutableData.Value) + 1; // Increment
            }
            return TransactionResult.Success(mutableData);
        }).ContinueWith(task =>
        {
            if (task.IsCompleted && task.Exception == null)
            {
                Debug.Log("Total attempts successfully updated.");
            }
            else
            {
                Debug.LogWarning("Failed to update total attempts: " + task.Exception);
            }
        });
    }
}
