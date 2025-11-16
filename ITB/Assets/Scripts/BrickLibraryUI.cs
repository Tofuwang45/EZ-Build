using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI controller for the brick library menu.
/// Displays available bricks and allows spawning.
/// </summary>
public class BrickLibraryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Container for brick buttons")]
    public Transform buttonContainer;

    [Tooltip("Button prefab for each brick type")]
    public GameObject brickButtonPrefab;

    [Tooltip("Info text display")]
    public TextMeshProUGUI infoText;

    [Header("Settings")]
    [Tooltip("Show/hide library on start")]
    public bool showOnStart = true;

    [Tooltip("Distance from user to spawn UI")]
    public float uiDistance = 1.5f;

    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void Start()
    {
        if (!showOnStart)
        {
            gameObject.SetActive(false);
        }

        PopulateLibrary();
        PositionUIInFrontOfUser();
    }

    /// <summary>
    /// Create buttons for each brick in the library
    /// </summary>
    private void PopulateLibrary()
    {
        if (BrickLibrary.Instance == null)
        {
            Debug.LogError("BrickLibrary instance not found!");
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("Button container not assigned!");
            return;
        }

        // Clear existing buttons
        foreach (var btn in spawnedButtons)
        {
            if (btn != null)
                Destroy(btn);
        }
        spawnedButtons.Clear();

        // Create button for each brick type
        int count = BrickLibrary.Instance.GetBrickCount();
        for (int i = 0; i < count; i++)
        {
            var entry = BrickLibrary.Instance.GetBrickEntry(i);
            if (entry == null) continue;

            CreateBrickButton(i, entry);
        }

        UpdateInfoText($"{count} brick types available");
    }

    /// <summary>
    /// Create a button for a brick type
    /// </summary>
    private void CreateBrickButton(int index, BrickLibrary.BrickPrefabEntry entry)
    {
        GameObject buttonObj;

        if (brickButtonPrefab != null)
        {
            buttonObj = Instantiate(brickButtonPrefab, buttonContainer);
        }
        else
        {
            // Create basic button if no prefab
            buttonObj = new GameObject($"Btn_{entry.displayName}");
            buttonObj.transform.SetParent(buttonContainer);
            buttonObj.AddComponent<RectTransform>();
            buttonObj.AddComponent<Image>();
            buttonObj.AddComponent<Button>();
        }

        spawnedButtons.Add(buttonObj);

        // Setup button
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null)
        {
            int capturedIndex = index; // Capture for closure
            btn.onClick.AddListener(() => OnBrickButtonClicked(capturedIndex));
        }

        // Setup text
        TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = entry.displayName;
        }
        else
        {
            // Add text if not present
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = entry.displayName;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 24;
            
            RectTransform rt = textObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        // Setup icon
        if (entry.icon != null)
        {
            Image img = buttonObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = entry.icon;
            }
        }
    }

    /// <summary>
    /// Called when a brick button is clicked
    /// </summary>
    private void OnBrickButtonClicked(int brickIndex)
    {
        if (BrickLibrary.Instance == null)
            return;

        var entry = BrickLibrary.Instance.GetBrickEntry(brickIndex);
        if (entry != null)
        {
            UpdateInfoText($"Spawning {entry.displayName}...");
            GameObject brick = BrickLibrary.Instance.SpawnBrickInFrontOfUser(brickIndex);
            
            if (brick != null)
            {
                UpdateInfoText($"Spawned {entry.displayName}!");
            }
            else
            {
                UpdateInfoText($"Failed to spawn {entry.displayName}");
            }
        }
    }

    /// <summary>
    /// Update info text display
    /// </summary>
    private void UpdateInfoText(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
        }
        Debug.Log($"[BrickLibraryUI] {message}");
    }

    /// <summary>
    /// Position UI in front of user
    /// </summary>
    private void PositionUIInFrontOfUser()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // Position in front of camera
        Vector3 targetPos = mainCam.transform.position + mainCam.transform.forward * uiDistance;
        transform.position = targetPos;

        // Face the user
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }

    /// <summary>
    /// Toggle library visibility
    /// </summary>
    public void ToggleLibrary()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        
        if (gameObject.activeSelf)
        {
            PositionUIInFrontOfUser();
        }
    }

    /// <summary>
    /// Refresh the library display
    /// </summary>
    [ContextMenu("Refresh Library")]
    public void RefreshLibrary()
    {
        PopulateLibrary();
    }
}
