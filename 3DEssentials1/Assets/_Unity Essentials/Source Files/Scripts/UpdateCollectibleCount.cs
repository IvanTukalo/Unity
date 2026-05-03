using UnityEngine;
using TMPro;
using System; // Required for Type handling

public class UpdateCollectibleCount : MonoBehaviour
{
    private TextMeshProUGUI collectibleText; // Reference to the TextMeshProUGUI component
    private bool allCollected = false; // Flag to prevent repeated end-game calls
    private float endGameTimer = -1f; // Timer for delayed game end

    void Start()
    {
        collectibleText = GetComponent<TextMeshProUGUI>();
        if (collectibleText == null)
        {
            Debug.LogError("UpdateCollectibleCount script requires a TextMeshProUGUI component on the same GameObject.");
            return;
        }
        UpdateCollectibleDisplay(); // Initial update on start
    }

    void Update()
    {
        UpdateCollectibleDisplay();

        // Handle delayed game end
        if (endGameTimer > 0f)
        {
            endGameTimer -= Time.deltaTime;
            if (endGameTimer <= 0f)
            {
                EndGame();
            }
        }
    }

    private void UpdateCollectibleDisplay()
    {
        if (allCollected) return; // Don't update once all collected

        int totalCollectibles = 0;

        // Check and count objects of type Collectible
        Type collectibleType = Type.GetType("Collectible");
        if (collectibleType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(collectibleType, FindObjectsSortMode.None).Length;
        }

        // Optionally, check and count objects of type Collectible2D as well if needed
        Type collectible2DType = Type.GetType("Collectible2D");
        if (collectible2DType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(collectible2DType, FindObjectsSortMode.None).Length;
        }

        // Also count MyCollectible2D objects
        Type myCollectible2DType = Type.GetType("MyCollectible2D");
        if (myCollectible2DType != null)
        {
            totalCollectibles += UnityEngine.Object.FindObjectsByType(myCollectible2DType, FindObjectsSortMode.None).Length;
        }

        // Update the collectible count display
        collectibleText.text = $"Collectibles remaining: {totalCollectibles}";

        // Check if all collectibles have been collected
        if (totalCollectibles == 0)
        {
            allCollected = true;
            collectibleText.text = "All collectibles collected!";
            Debug.Log("All collectibles collected!");
            // End game after 3 seconds
            endGameTimer = 3f;
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over - All collectibles collected!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
