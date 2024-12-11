using UnityEngine;
using UnityEngine.UI; // Required for Button components.
using TMPro; // Required for TextMeshPro components.

public class TimerController : MonoBehaviour
{
    public TMP_Text timerText; // Assign the TextMeshPro Text UI element in the Inspector.
    public Button startButton; // Assign the Button UI element in the Inspector.
    public Button quitButton; // Assign the quit button in the Inspector.
    private float timer = 0f; // Timer value in seconds.
    private bool isRunning = false;
    private AuthManager authManager;

    void Start()
    {
        // Find the AuthManager instance.
        authManager = FindObjectOfType<AuthManager>();

        // Add listeners to buttons.
        startButton.onClick.AddListener(StartTimer);
        quitButton.onClick.AddListener(QuitGame);

        // Optionally, set the initial text.
        timerText.text = "Time: 0.0s";
    }

    void Update()
    {
        if (isRunning)
        {
            // Increment the timer and update the text.
            timer += Time.deltaTime;
            timerText.text = "Time: " + timer.ToString("F1") + "s";
        }
    }

    // Function to start the timer when the button is clicked.
    void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            timer = 0f; // Reset the timer.
        }
    }

    // Function to handle quitting the game and updating the total time played.
    void QuitGame()
    {
        if (authManager != null && authManager.User != null)
        {
            // Update the total_time_played for the current user.
            authManager.UpdateTotalTimePlayed((int)timer);
        }

        // You can also add other quit-related logic here, like saving game data, etc.
        Debug.Log("Game Quit, Time Played: " + timer);
        Application.Quit(); // Exits the application (works only in a build).
    }
}
