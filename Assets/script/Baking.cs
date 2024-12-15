/// <author>Kang Hao</author>
/// <date>2024-12-11</date>
/// <summary>
/// this script manager the baking function for the cooking game
/// </summary>


using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BakingScript : MonoBehaviour
{
    [Header("References")]
    public string requiredTag = "Bakeable"; // The required tag to allow baking
    public TextMeshProUGUI timerText; // TextMeshPro for displaying the timer
    public Button bakeButton; // Button to trigger baking
    public GameObject cookedPrefab; // Prefab to spawn once baked

    [Header("Baking Settings")]
    public float bakingTime = 5f; // Time required to bake in seconds

    private GameObject itemToBake;
    private float timer;
    private bool isBaking;

    void Start()
    {
        // Ensure the button is initially disabled
        bakeButton.interactable = false;
        bakeButton.onClick.AddListener(StartBaking);
        timerText.text = ""; // Clear initial text
    }

    void Update()
    {
        // Handle the baking timer
        if (isBaking)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Max(timer, 0).ToString("F2") + "s";

            if (timer <= 0)
            {
                FinishBaking();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the required tag
        if (other.CompareTag(requiredTag))
        {
            itemToBake = other.gameObject;
            bakeButton.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset if the object leaves the area
        if (other.gameObject == itemToBake)
        {
            itemToBake = null;
            bakeButton.interactable = false;
            timerText.text = "";
        }
    }

    public void StartBaking()
    {
        if (itemToBake != null && !isBaking)
        {
            isBaking = true;
            timer = bakingTime;
            bakeButton.interactable = false; // Disable button during baking
        }
    }

    private void FinishBaking()
    {
        isBaking = false;
        timerText.text = "Done!";

        // Spawn the cooked item and destroy the original
        Instantiate(cookedPrefab, itemToBake.transform.position, Quaternion.identity);
        Destroy(itemToBake);

        // Reset button
        bakeButton.interactable = false;
        itemToBake = null;
    }
}
