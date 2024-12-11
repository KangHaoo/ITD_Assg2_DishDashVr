using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference dbReference;

    // Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    // Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }



    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth and Database");
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }



    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to login task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail: message = "Missing Email"; break;
                case AuthError.MissingPassword: message = "Missing Password"; break;
                case AuthError.WrongPassword: message = "Wrong Password"; break;
                case AuthError.InvalidEmail: message = "Invalid Email"; break;
                case AuthError.UserNotFound: message = "Account does not exist"; break;
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail: message = "Missing Email"; break;
                    case AuthError.MissingPassword: message = "Missing Password"; break;
                    case AuthError.WeakPassword: message = "Weak Password"; break;
                    case AuthError.EmailAlreadyInUse: message = "Email Already In Use"; break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(() => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        UserStats newUserStats = new UserStats();
                        StartCoroutine(SaveUserData(_username, User.UserId, _password, newUserStats));
                        warningRegisterText.text = "";
                        UIManager.instance.LoginScreen();
                    }
                }
            }
        }
    }

    private IEnumerator SaveUserData(string username, string uid, string password, UserStats userStats)
    {
        var userInformation = new Dictionary<string, object>
        {
            { "username", username },
            { "uid", uid },
            { "password", password },
            { "time_created", ServerValue.Timestamp }
        };

        var infoTask = dbReference.Child("Players").Child(username).SetValueAsync(userInformation);
        yield return new WaitUntil(() => infoTask.IsCompleted);

        if (infoTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save user information: {infoTask.Exception}");
        }
        else
        {
            Debug.Log("User information saved successfully.");
        }

        var statsTask = dbReference.Child("Stats").Child(username).SetValueAsync(userStats.ToDictionary());
        yield return new WaitUntil(() => statsTask.IsCompleted);

        if (statsTask.Exception != null)
        {
            Debug.LogWarning($"Failed to save user stats: {statsTask.Exception}");
        }
        else
        {
            Debug.Log("User stats saved successfully.");
        }
    }

    public void UpdateTotalTimePlayed(int timePlayed)
    {
        if (User != null)
        {
            // Retrieve current user stats from the database
            DatabaseReference userStatsRef = dbReference.Child("Stats").Child(User.DisplayName);

            userStatsRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    // Check if stats exist, if not create a new one
                    if (snapshot.Exists)
                    {
                        var currentStats = snapshot.Value as Dictionary<string, object>;
                        int currentTotalTime = (int)currentStats["total_time_played"];

                        // Update total time played
                        currentTotalTime += timePlayed;

                        // Save the updated stats back to Firebase
                        userStatsRef.Child("total_time_played").SetValueAsync(currentTotalTime);
                        Debug.Log("Updated total_time_played: " + currentTotalTime);
                    }
                    else
                    {
                        // If stats don't exist, create new stats object
                        UserStats newStats = new UserStats(totalTimePlayed: timePlayed);
                        userStatsRef.SetValueAsync(newStats.ToDictionary());
                        Debug.Log("Created new stats with total_time_played: " + timePlayed);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to fetch user stats: " + task.Exception);
                }
            });
        }
    }
}


[System.Serializable]
public class UserStats
{
    public int totalTimePlayed;
    public int totalFoodMade;
    public string favoriteFoodMade;
    public int highestScore;

    public UserStats(int totalTimePlayed = 0, int totalFoodMade = 0, string favoriteFoodMade = "None", int highestScore = 0)
    {
        this.totalTimePlayed = totalTimePlayed;
        this.totalFoodMade = totalFoodMade;
        this.favoriteFoodMade = favoriteFoodMade;
        this.highestScore = highestScore;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "total_time_played", totalTimePlayed },
            { "total_food_made", totalFoodMade },
            { "favorite_food_made", favoriteFoodMade },
            { "highest_score", highestScore }
        };
    }
}
