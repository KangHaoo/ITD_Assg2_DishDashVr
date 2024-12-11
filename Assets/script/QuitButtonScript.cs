using UnityEngine;

public class QuitButtonScript : MonoBehaviour
{
    // Function to quit the application
    public void QuitGame()
    {
#if UNITY_EDITOR
        // If running in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a built application
        Application.Quit();
#endif
    }
}
