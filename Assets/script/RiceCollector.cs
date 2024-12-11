using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceCollector : MonoBehaviour
{
    // The object to unhide inside the bowl
    public GameObject hiddenObject;

    // The tag for the rice object
    public string bowlTag = "DirtyRice";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object has the correct tag
        if (other.CompareTag(bowlTag))
        {
            // Unhide the hidden object
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
                Debug.Log("Hidden object revealed!");
            }
            else
            {
                Debug.LogWarning("Hidden object is not assigned in the inspector.");
            }
        }
    }
}
