using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Templates.MR;

/// <summary>
/// Bridges the <see cref="LegoSceneTracker"/> data into a floating debug panel driven by <see cref="DebugInfoDisplayController"/>.
/// Attach this alongside the controller to surface live cube counts and recent positions in-game.
/// </summary>
[DisallowMultipleComponent]
public class LegoTrackerDebugView : MonoBehaviour
{
    [Header("View References")]
    [SerializeField] private DebugInfoDisplayController debugDisplay;

    [Tooltip("Optional explicit tracker reference. Falls back to LegoSceneTracker.Instance if left empty.")]
    [SerializeField] private LegoSceneTracker tracker;

    private readonly List<LegoSceneTracker.TrackedBrickState> tempStates = new List<LegoSceneTracker.TrackedBrickState>();
    private readonly List<LegoSceneTracker.BrickConnectionInfo> tempConnections = new List<LegoSceneTracker.BrickConnectionInfo>();

    private void Awake()
    {
        if (debugDisplay == null)
            debugDisplay = GetComponent<DebugInfoDisplayController>();

        if (debugDisplay == null)
            debugDisplay = GetComponentInChildren<DebugInfoDisplayController>(true);

        if (tracker == null)
            tracker = LegoSceneTracker.Instance;

        if (debugDisplay == null)
            Debug.LogWarning("LegoTrackerDebugView could not find a DebugInfoDisplayController reference.", this);
    }

    private void Update()
    {
        if (tracker == null)
            tracker = LegoSceneTracker.Instance;

        if (debugDisplay == null || tracker == null)
            return;

        tracker.RefreshTrackedBricks();
        tracker.RecalculateConnections();

        int brickCount = tracker.BrickCount;
        debugDisplay.Show(brickCount > 0);

        if (brickCount == 0)
        {
            debugDisplay.RefreshDisplayInfo();
            return;
        }

        tempStates.Clear();
        foreach (var state in tracker.TrackedStates)
        {
            if (state == null || state.Transform == null)
                continue;
            tempStates.Add(state);
        }

        tempStates.Sort((a, b) => string.Compare(a.BrickName, b.BrickName, System.StringComparison.Ordinal));

        System.Text.StringBuilder consoleBuilder = null;
        if (Application.isPlaying)
            consoleBuilder = new System.Text.StringBuilder(tempStates.Count * 48);

        for (int i = 0; i < tempStates.Count; i++)
        {
            var state = tempStates[i];
            string label = state.BrickName ?? $"Cube {state.InstanceId}";
            string entry = FormatVector(state.LastPosition);
            string displayLine = $"{label}: {entry}";
            debugDisplay.AppendDebugEntry(label, entry);

            consoleBuilder?.AppendLine(displayLine);
        }

        tempConnections.Clear();
        var groups = tracker.ConnectedGroups;
        Debug.Log($"[LegoTrackerDebugView] Found {groups.Count} connected groups");
        
        for (int g = 0; g < groups.Count; g++)
        {
            var group = groups[g];
            if (group == null)
                continue;

            var connections = group.Connections;
            if (connections == null)
                continue;

            Debug.Log($"[LegoTrackerDebugView] Group {g+1} has {connections.Count} connections");
            
            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                if (connection != null)
                    tempConnections.Add(connection);
            }
        }

        if (tempConnections.Count > 0)
        {
            string separator = $"--- {tempConnections.Count} Connection(s) ---";
            debugDisplay.AppendDebugEntry("", separator);
            consoleBuilder?.AppendLine($"\n{separator}");
        }

        for (int i = 0; i < tempConnections.Count; i++)
        {
            var connection = tempConnections[i];
            string left = connection.A != null ? connection.A.BrickName : "?";
            string right = connection.B != null ? connection.B.BrickName : "?";
            string detail = connection.Detail;
            string descriptor = string.IsNullOrEmpty(detail) ? $"{left} <-> {right}" : $"{left} <-> {right} ({detail})";
            debugDisplay.AppendDebugEntry($"Pair {i + 1}", descriptor);

            consoleBuilder?.AppendLine($"Pair {i + 1}: {descriptor}");
        }

        debugDisplay.RefreshDisplayInfo();

        if (consoleBuilder != null && consoleBuilder.Length > 0)
            Debug.Log("[LegoTrackerDebugView]\n" + consoleBuilder.ToString().TrimEnd(), this);
    }

    private static string FormatVector(Vector3 value)
    {
        return $"({value.x:0.000}, {value.y:0.000}, {value.z:0.000})";
    }
}
