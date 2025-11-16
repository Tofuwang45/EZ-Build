using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a library of LEGO brick prefabs that can be spawned in the world.
/// Works with Quest 3 hand tracking and controllers.
/// </summary>
public class BrickLibrary : MonoBehaviour
{
    [System.Serializable]
    public class BrickPrefabEntry
    {
        public string displayName;
        public GameObject prefab;
        public Sprite icon;
        [Tooltip("Brick dimensions in studs (e.g., 2x4)")]
        public Vector2Int dimensions = new Vector2Int(2, 4);
        public Color color = Color.white;
    }

    [Header("Brick Library")]
    [Tooltip("List of available brick prefabs")]
    public List<BrickPrefabEntry> brickPrefabs = new List<BrickPrefabEntry>();

    [Header("Spawn Settings")]
    [Tooltip("Default spawn position offset from user")]
    public Vector3 spawnOffset = new Vector3(0, 0, 0.5f);

    [Tooltip("Parent transform for spawned bricks (for organization)")]
    public Transform brickContainer;

    [Header("Initialization")]
    [Tooltip("Snap point prefab to assign to spawned bricks")]
    public GameObject snapPointPrefab;

    private static BrickLibrary _instance;
    public static BrickLibrary Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BrickLibrary>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        // Create brick container if not assigned
        if (brickContainer == null)
        {
            GameObject container = new GameObject("BrickContainer");
            brickContainer = container.transform;
        }
    }

    /// <summary>
    /// Spawn a brick from the library by index
    /// </summary>
    public GameObject SpawnBrick(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (prefabIndex < 0 || prefabIndex >= brickPrefabs.Count)
        {
            Debug.LogError($"Invalid brick prefab index: {prefabIndex}");
            return null;
        }

        BrickPrefabEntry entry = brickPrefabs[prefabIndex];
        return SpawnBrick(entry, position, rotation);
    }

    /// <summary>
    /// Spawn a brick from a prefab entry
    /// </summary>
    public GameObject SpawnBrick(BrickPrefabEntry entry, Vector3 position, Quaternion rotation)
    {
        if (entry.prefab == null)
        {
            Debug.LogError("Brick prefab is null!");
            return null;
        }

        // Instantiate the brick
        GameObject brick = Instantiate(entry.prefab, position, rotation, brickContainer);
        
        // Initialize components
        InitializeBrick(brick, entry);

        Debug.Log($"Spawned brick: {entry.displayName} at {position}");
        return brick;
    }

    /// <summary>
    /// Spawn a brick in front of the main camera/user
    /// </summary>
    public GameObject SpawnBrickInFrontOfUser(int prefabIndex)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No main camera found!");
            return null;
        }

        Vector3 spawnPos = mainCam.transform.position + mainCam.transform.forward * spawnOffset.z;
        spawnPos += mainCam.transform.right * spawnOffset.x;
        spawnPos += mainCam.transform.up * spawnOffset.y;

        return SpawnBrick(prefabIndex, spawnPos, Quaternion.identity);
    }

    /// <summary>
    /// Initialize a spawned brick with all necessary components
    /// </summary>
    private void InitializeBrick(GameObject brick, BrickPrefabEntry entry)
    {
        // Ensure BrickIdentifier exists
        BrickIdentifier identifier = brick.GetComponent<BrickIdentifier>();
        if (identifier == null)
        {
            identifier = brick.AddComponent<BrickIdentifier>();
        }

        // Set brick properties
        identifier.studsLength = entry.dimensions.x;
        identifier.studsWidth = entry.dimensions.y;
        identifier.brickName = entry.displayName;
        identifier.brickColor = entry.color.ToString();
        identifier.RegenerateID(); // Ensure unique ID

        // Ensure LegoBrick exists
        LegoBrick legoBrick = brick.GetComponent<LegoBrick>();
        if (legoBrick == null)
        {
            legoBrick = brick.AddComponent<LegoBrick>();
        }

        // Assign snap point prefab if available
        if (snapPointPrefab != null && legoBrick.snapPointPrefab == null)
        {
            legoBrick.snapPointPrefab = snapPointPrefab;
        }

        // Ensure BrickScanner exists
        BrickScanner scanner = brick.GetComponent<BrickScanner>();
        if (scanner == null)
        {
            scanner = brick.AddComponent<BrickScanner>();
        }

        // Add XR interaction components if not present
        EnsureXRComponents(brick);

        // Generate snap points
        legoBrick.GenerateSnapPoints();
    }

    /// <summary>
    /// Ensure XR interaction components are present for Quest 3
    /// </summary>
    private void EnsureXRComponents(GameObject brick)
    {
        // Check for XRGrabInteractable using reflection (XR Toolkit may not be in project)
        try
        {
            var grabType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable, Unity.XR.Interaction.Toolkit");
            if (grabType != null && brick.GetComponent(grabType) == null)
            {
                brick.AddComponent(grabType);
            }

            // Add LegoXRGrabbable bridge if XR components exist
            if (brick.GetComponent<LegoXRGrabbable>() == null)
            {
                brick.AddComponent<LegoXRGrabbable>();
            }
        }
        catch
        {
            // XR Toolkit not available, skip
        }

        // Ensure Rigidbody
        Rigidbody rb = brick.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = brick.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;

        // Ensure Collider
        if (brick.GetComponent<Collider>() == null)
        {
            BoxCollider col = brick.AddComponent<BoxCollider>();
            // Try to auto-size based on mesh
            MeshFilter mf = brick.GetComponentInChildren<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                col.center = mf.sharedMesh.bounds.center;
                col.size = mf.sharedMesh.bounds.size;
            }
        }
    }

    /// <summary>
    /// Get number of available brick types
    /// </summary>
    public int GetBrickCount()
    {
        return brickPrefabs.Count;
    }

    /// <summary>
    /// Get brick entry by index
    /// </summary>
    public BrickPrefabEntry GetBrickEntry(int index)
    {
        if (index < 0 || index >= brickPrefabs.Count)
            return null;
        return brickPrefabs[index];
    }

    /// <summary>
    /// Context menu test spawner
    /// </summary>
    [ContextMenu("Spawn Test Brick")]
    public void SpawnTestBrick()
    {
        if (brickPrefabs.Count > 0)
        {
            SpawnBrickInFrontOfUser(0);
        }
        else
        {
            Debug.LogWarning("No brick prefabs configured in library!");
        }
    }
}
