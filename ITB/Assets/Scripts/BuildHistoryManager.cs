using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton manager that tracks the complete LEGO build history.
/// Automatically receives build steps from LegoBrick when pieces snap together.
/// </summary>
public class BuildHistoryManager : MonoBehaviour
{
    private static BuildHistoryManager _instance;
    public static BuildHistoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BuildHistoryManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("BuildHistoryManager");
                    _instance = go.AddComponent<BuildHistoryManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Build History")]
    [SerializeField]
    private List<BuildStep> buildHistory = new List<BuildStep>();

    [Header("Settings")]
    [Tooltip("Maximum steps to store (0 = unlimited)")]
    public int maxHistorySize = 0;

    [Tooltip("Enable console logging")]
    public bool enableDebugLog = true;

    [Tooltip("Auto-save interval in seconds (0 = disabled)")]
    public float autoSaveInterval = 30f;

    private float lastSaveTime;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        lastSaveTime = Time.time;
    }

    private void Update()
    {
        // Auto-save if enabled
        if (autoSaveInterval > 0 && Time.time - lastSaveTime >= autoSaveInterval)
        {
            AutoSave();
            lastSaveTime = Time.time;
        }
    }

    /// <summary>
    /// Add a new build step to history (called by LegoBrick.LogBuildStep)
    /// </summary>
    public void AddBuildStep(BuildStep step)
    {
        if (step == null)
        {
            Debug.LogWarning("[BuildHistory] Cannot add null step");
            return;
        }

        buildHistory.Add(step);

        if (enableDebugLog)
        {
            string parentInfo = step.connectedParentIDs.Count > 0 
                ? $" → Connected to {step.connectedParentIDs.Count} brick(s)" 
                : " → Foundation brick";
            
            Debug.Log($"<color=cyan>📝 [BuildHistory] Step {buildHistory.Count}: {step.brickName}{parentInfo}</color>");
        }

        // Enforce max size
        if (maxHistorySize > 0 && buildHistory.Count > maxHistorySize)
        {
            buildHistory.RemoveAt(0);
        }
    }

    /// <summary>
    /// Remove steps associated with a brick ID (for undo/deletion)
    /// </summary>
    public void RemoveBuildStep(string brickID)
    {
        int removed = buildHistory.RemoveAll(step => step.brickID == brickID);
        
        if (enableDebugLog && removed > 0)
        {
            Debug.Log($"[BuildHistory] Removed {removed} step(s) for brick {brickID}");
        }
    }

    /// <summary>
    /// Find a step by brick ID
    /// </summary>
    public BuildStep FindStepByBrickID(string brickID)
    {
        return buildHistory.FirstOrDefault(step => step.brickID == brickID);
    }

    /// <summary>
    /// Get all build steps (read-only copy)
    /// </summary>
    public List<BuildStep> GetAllSteps()
    {
        return new List<BuildStep>(buildHistory);
    }

    /// <summary>
    /// Get total step count
    /// </summary>
    public int GetStepCount()
    {
        return buildHistory.Count;
    }

    /// <summary>
    /// Clear entire history
    /// </summary>
    public void ClearHistory()
    {
        buildHistory.Clear();
        if (enableDebugLog)
        {
            Debug.Log("[BuildHistory] History cleared");
        }
    }

    /// <summary>
    /// Generate summary text
    /// </summary>
    public string GenerateSummary()
    {
        if (buildHistory.Count == 0)
        {
            return "No build steps recorded yet.";
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== LEGO Build History ({buildHistory.Count} steps) ===");
        sb.AppendLine();

        for (int i = 0; i < buildHistory.Count; i++)
        {
            BuildStep step = buildHistory[i];
            sb.AppendLine($"{i + 1}. {step.GetDescription()}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Export to JSON for AI API consumption
    /// </summary>
    public string ExportToJSON()
    {
        BuildHistoryData data = new BuildHistoryData
        {
            totalSteps = buildHistory.Count,
            buildTimeSeconds = buildHistory.Count > 0 ? buildHistory[buildHistory.Count - 1].timestamp : 0f,
            steps = buildHistory
        };

        return JsonUtility.ToJson(data, true);
    }

    /// <summary>
    /// Generate AI prompt for instruction generation
    /// </summary>
    public string GenerateAIPrompt()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine("# LEGO Assembly Instructions Generator");
        sb.AppendLine();
        sb.AppendLine("Generate clear, step-by-step assembly instructions for the following LEGO construction:");
        sb.AppendLine();
        sb.AppendLine("## Build Sequence:");
        sb.AppendLine();

        for (int i = 0; i < buildHistory.Count; i++)
        {
            BuildStep step = buildHistory[i];
            sb.AppendLine($"**Step {i + 1}** ({step.timestamp:F1}s)");
            sb.AppendLine($"- Brick: {step.brickName} ({step.studsWidth}x{step.studsLength})");
            
            if (step.connectedParentIDs.Count == 0)
            {
                sb.AppendLine($"- Action: Place as foundation/base");
            }
            else if (step.connectedParentIDs.Count == 1)
            {
                sb.AppendLine($"- Action: Attach on top of previous brick");
            }
            else
            {
                sb.AppendLine($"- Action: Bridge across {step.connectedParentIDs.Count} bricks");
            }
            
            sb.AppendLine();
        }

        sb.AppendLine("## Output Format:");
        sb.AppendLine("Please provide:");
        sb.AppendLine("1. Numbered assembly steps with clear descriptions");
        sb.AppendLine("2. Tips for proper alignment");
        sb.AppendLine("3. Visual cues or landmarks for each step");

        return sb.ToString();
    }

    /// <summary>
    /// Auto-save to PlayerPrefs
    /// </summary>
    private void AutoSave()
    {
        string json = ExportToJSON();
        PlayerPrefs.SetString("LEGO_BuildHistory", json);
        PlayerPrefs.Save();
        
        if (enableDebugLog)
        {
            Debug.Log($"[BuildHistory] Auto-saved {buildHistory.Count} steps");
        }
    }

    /// <summary>
    /// Load from PlayerPrefs
    /// </summary>
    public void LoadHistory()
    {
        if (PlayerPrefs.HasKey("LEGO_BuildHistory"))
        {
            string json = PlayerPrefs.GetString("LEGO_BuildHistory");
            BuildHistoryData data = JsonUtility.FromJson<BuildHistoryData>(json);
            
            if (data != null && data.steps != null)
            {
                buildHistory = data.steps;
                Debug.Log($"[BuildHistory] Loaded {buildHistory.Count} steps");
            }
        }
    }

    /// <summary>
    /// Context menu helpers
    /// </summary>
    [ContextMenu("Print History")]
    public void PrintHistory()
    {
        Debug.Log(GenerateSummary());
    }

    [ContextMenu("Print AI Prompt")]
    public void PrintAIPrompt()
    {
        Debug.Log(GenerateAIPrompt());
    }

    [ContextMenu("Export JSON")]
    public void PrintJSON()
    {
        string json = ExportToJSON();
        Debug.Log(json);
        GUIUtility.systemCopyBuffer = json;
        Debug.Log("JSON copied to clipboard!");
    }
}

/// <summary>
/// Wrapper for JSON serialization
/// </summary>
[System.Serializable]
public class BuildHistoryData
{
    public int totalSteps;
    public float buildTimeSeconds;
    public List<BuildStep> steps;
}
