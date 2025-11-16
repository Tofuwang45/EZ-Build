using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects which bricks this brick is connected to using the LegoSnapPoint system.
/// Solves the "bridge scenario" where one brick connects to multiple bricks below.
/// </summary>
public class BrickScanner : MonoBehaviour
{
    [Header("Debug Visualization")]
    [Tooltip("Draw debug connection lines in Scene view")]
    public bool showDebugConnections = true;

    [Tooltip("Color of debug connection lines")]
    public Color debugConnectionColor = Color.green;

    private BrickIdentifier brickIdentifier;
    private LegoBrick legoBrick;

    private void Awake()
    {
        brickIdentifier = GetComponent<BrickIdentifier>();
        if (brickIdentifier == null)
        {
            Debug.LogError($"BrickScanner on {gameObject.name} requires a BrickIdentifier component!");
        }

        legoBrick = GetComponent<LegoBrick>();
        if (legoBrick == null)
        {
            Debug.LogError($"BrickScanner on {gameObject.name} requires a LegoBrick component!");
        }
    }

    /// <summary>
    /// Scan for all bricks that this brick is connected to using LegoSnapPoint data
    /// Returns a list of unique brick IDs
    /// </summary>
    public List<string> GetConnectedBricks()
    {
        if (legoBrick == null)
            return new List<string>();

        return legoBrick.GetConnectedBrickIDs();
    }

    /// <summary>
    /// Get connected brick names (for debugging)
    /// </summary>
    public List<string> GetConnectedBrickNames()
    {
        List<string> foundIDs = GetConnectedBricks();
        List<string> brickNames = new List<string>();

        foreach (string id in foundIDs)
        {
            // Find the brick with this ID in the scene
            BrickIdentifier[] allBricks = FindObjectsOfType<BrickIdentifier>();
            foreach (var brick in allBricks)
            {
                if (brick.uniqueID == id)
                {
                    brickNames.Add(brick.brickName);
                    break;
                }
            }
        }

        return brickNames;
    }



    private void OnDrawGizmosSelected()
    {
        if (!showDebugConnections || legoBrick == null) return;

        Gizmos.color = debugConnectionColor;

        // Draw lines to all connected bricks
        foreach (var socket in legoBrick.socketSnapPoints)
        {
            if (socket == null || !socket.isConnected || socket.connectedTo == null)
                continue;

            Gizmos.DrawLine(socket.transform.position, socket.connectedTo.transform.position);
            Gizmos.DrawSphere(socket.transform.position, 0.02f);
        }
    }

    /// <summary>
    /// Context menu helper to test scanning
    /// </summary>
    [ContextMenu("Test Scan")]
    public void TestScan()
    {
        List<string> connected = GetConnectedBrickNames();

        if (connected.Count == 0)
        {
            Debug.Log($"{brickIdentifier.brickName}: No connected bricks found");
        }
        else
        {
            Debug.Log($"{brickIdentifier.brickName} is connected to: {string.Join(", ", connected)}");
        }
    }
}
