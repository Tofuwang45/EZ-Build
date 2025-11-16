using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single step in the LEGO build history.
/// Records brick placement, connections, and transform data.
/// </summary>
[System.Serializable]
public class BuildStep
{
    [Header("Step Identity")]
    public string stepID;
    public float timestamp;

    [Header("Brick Information")]
    public string brickID;
    public string brickName;
    public int studsWidth;
    public int studsLength;

    [Header("Connection Data")]
    public List<string> connectedParentIDs;

    [Header("Transform Data")]
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 worldPosition;

    /// <summary>
    /// Constructor for creating a new build step
    /// </summary>
    public BuildStep(string brickID, string brickName, int width, int length, List<string> connectedParents)
    {
        this.stepID = System.Guid.NewGuid().ToString();
        this.timestamp = Time.time;
        this.brickID = brickID;
        this.brickName = brickName;
        this.studsWidth = width;
        this.studsLength = length;
        this.connectedParentIDs = connectedParents ?? new List<string>();
    }

    /// <summary>
    /// Constructor with full transform data
    /// </summary>
    public BuildStep(string brickID, string brickName, int width, int length, List<string> connectedParents, 
                    Vector3 localPos, Quaternion localRot, Vector3 worldPos)
    {
        this.stepID = System.Guid.NewGuid().ToString();
        this.timestamp = Time.time;
        this.brickID = brickID;
        this.brickName = brickName;
        this.studsWidth = width;
        this.studsLength = length;
        this.connectedParentIDs = connectedParents ?? new List<string>();
        this.localPosition = localPos;
        this.localRotation = localRot;
        this.worldPosition = worldPos;
    }

    /// <summary>
    /// Get human-readable description for AI prompt generation
    /// </summary>
    public string GetDescription()
    {
        if (connectedParentIDs == null || connectedParentIDs.Count == 0)
        {
            return $"Step {timestamp:F1}s: Placed {brickName} as foundation brick";
        }
        else if (connectedParentIDs.Count == 1)
        {
            return $"Step {timestamp:F1}s: Attached {brickName} to 1 brick below";
        }
        else
        {
            return $"Step {timestamp:F1}s: Bridged {brickName} across {connectedParentIDs.Count} bricks";
        }
    }

    /// <summary>
    /// Convert to JSON string
    /// </summary>
    public string ToJSON()
    {
        return JsonUtility.ToJson(this, true);
    }
}
