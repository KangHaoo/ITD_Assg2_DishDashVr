using UnityEngine;
using System;
using TMPro;
using Firebase.Database;
using System.Collections.Generic;
using System.Collections;

public class TimerPoke : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private bool isTimerRunning = false;
    private float timer = 0f;

    private AuthManager authManager;

    private void Awake()
    {
        authManager = FindObjectOfType<AuthManager>();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    // Triggered when the object is poked.
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            timer = 0f; // Reset timer when starting
            Debug.Log("Timer started at 0.");
        }
    }

    // Stops the timer (optional, you can call this when needed).
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log($"Timer stopped at: {timer} seconds."); // Show current timer value
        UpdateTotalPlayedTime();  // Update Firebase when timer stops
    }

    // Update the UI or handle the timer logic.
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"Time: {minutes:D2}:{seconds:D2}";
        }
    }


    private void UpdateTotalPlayedTime()
    {
        if (authManager.User != null)
        {
            string userId = authManager.User.UserId;
            Debug.Log($"User ID: {userId}");

            DatabaseReference userStatRef = authManager.dbReference.Child("Players").Child(userId).Child("stat");

            userStatRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"Failed to fetch user stats: {task.Exception}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                double currentTotalPlayedTime = 0.0;

                if (snapshot.Exists && snapshot.Child("total_played_time").Value != null)
                {
                    currentTotalPlayedTime = snapshot.Child("total_played_time").Value is long longValue
                        ? Convert.ToDouble(longValue)
                        : snapshot.Child("total_played_time").Value is double doubleValue
                            ? doubleValue
                            : 0.0;
                }

                Debug.Log($"Current Total Played Time: {currentTotalPlayedTime}");

                double newTotalPlayedTime = currentTotalPlayedTime + timer;
                Debug.Log($"New Total Played Time: {newTotalPlayedTime}");

                Dictionary<string, object> updatedStats = new Dictionary<string, object>
            {
                { "total_played_time", newTotalPlayedTime }
            };

                userStatRef.UpdateChildrenAsync(updatedStats).ContinueWith(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("Failed to update total_played_time: " + updateTask.Exception);
                    }
                    else if (updateTask.IsCanceled)
                    {
                        Debug.LogError("Update total_played_time task was canceled.");
                    }
                    else
                    {
                        Debug.Log("Total played time updated successfully!");
                    }
                });
            });
        }
        else
        {
            Debug.LogError("AuthManager User is null.");
        }
    }

}
