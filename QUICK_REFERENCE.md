# Quick Reference Guide - Unity MR Lego Project

## File Locations Quick Access

### Core Scripts (MRTemplateAssets)
```
/home/user/lego/ITB/Assets/MRTemplateAssets/Scripts/
├── HandSubsystemManager.cs           (Hand tracking control)
├── ARFeatureController.cs             (AR features: planes, passthrough, bounding boxes)
├── OcclusionManager.cs                (Occlusion rendering setup)
├── GoalManager.cs                     (Tutorial/onboarding flow)
├── SpawnedObjectsManager.cs           (Object persistence with anchors)
├── XRBlaster.cs                       (Grab interactable example)
├── GazeTooltips.cs                    (Dynamic tooltips)
├── SaveAndLoadAnchorDataToFile.cs    (Persistent storage)
└── [22 other support scripts]
```

### 3D Models
```
Lego Bricks:
/home/user/lego/ITB/Assets/LegoBricks/Legos.fbx    (1.5MB - main model)

Backup Formats:
/home/user/lego/ITB/Assets/ExtraFiles/
├── Legos.fbx
├── LEGO'S.3ds
├── LEGO'S.obj
└── LEGO'S.c4d
```

### UI Prefabs
```
/home/user/lego/ITB/Assets/MRTemplateAssets/Prefabs/UI/
├── CoachingUI.prefab                  (Tutorial interface)
├── HandMenuSetupVariant_MRTemplate.prefab  (Hand menu)
├── ListItem*.prefab                   (UI control variants)
├── TapVisualization.prefab            (Tap feedback)
├── Tooltip*.prefab                    (Tooltip variants)
└── [more UI components]
```

### Scenes
```
Main Scenes:
/home/user/lego/ITB/Assets/Scenes/SampleScene.unity    (Current main scene)
/home/user/lego/ITB/Assets/version1.unity              (Prototype with interactive bricks)

Reference Scenes:
/home/user/lego/ITB/Assets/Samples/XR Interaction Toolkit/3.2.1/
├── Starter Assets/DemoScene.unity     (VR demo)
├── Hands Interaction Demo/HandsDemoScene.unity
└── AR Starter Assets/ARDemoScene.unity
```

### Materials & Shaders
```
Interactable Materials:
/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/Primitive/
├── Interactables.mat (base)
├── Interactables_2.mat to 6.mat (color variants)
└── Interactables_*.physicMaterial

AR Materials:
/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/AR/

Shaders:
/home/user/lego/ITB/Assets/MRTemplateAssets/Shaders/
├── InteractablePrimitive.shadergraph
└── [other shaders]
```

### Interactive Prefabs
```
Base Interactables:
/home/user/lego/ITB/Assets/MRTemplateAssets/Prefabs/
├── TestObject.prefab
├── TotemFloatingTorus.prefab
├── TotemKinetic.prefab
├── TotemBlaster.prefab
└── MR Interaction Setup.prefab

XR Toolkit Samples:
/home/user/lego/ITB/Assets/Samples/XR Interaction Toolkit/3.2.1/
├── Starter Assets/DemoSceneAssets/Prefabs/
├── Hands Interaction Demo/HandsDemoSceneAssets/Prefabs/
└── AR Starter Assets/ARDemoSceneAssets/Prefabs/
```

---

## Key Classes & Their Relationships

### MR Foundation
```
ARFeatureController
  ├─ Controls ARCameraManager (passthrough)
  ├─ Controls ARPlaneManager (plane detection)
  ├─ Controls ARBoundingBoxManager
  └─ Controls OcclusionManager

HandSubsystemManager
  ├─ Manages XRHandSubsystem
  └─ Enables/disables hand tracking

SaveAndLoadAnchorDataToFile
  └─ Persists SerializableGuid -> prefab index mapping
```

### Object Management
```
SpawnedObjectsManager
  ├─ References ObjectSpawner
  ├─ Manages SpawnedObjectHelper struct array
  ├─ References ARAnchorManager
  └─ Integrates with SaveAndLoadAnchorDataToFile

ObjectSpawner (from XR Toolkit)
  ├─ Maintains objectPrefabs list
  ├─ Fires objectSpawned(GameObject) event
  └─ Supports randomization
```

### User Experience
```
GoalManager
  ├─ Manages Queue<Goal> for tutorial progression
  ├─ Integrates with ARFeatureController
  ├─ Integrates with ObjectSpawner (listens to objectSpawned)
  ├─ Controls CoachingUI
  └─ Controls VideoPlayer

GazeTooltips
  ├─ Works with ZoneScale
  ├─ Uses Physics.SphereCast for plane detection
  └─ Positions tooltip on detected surfaces
```

### Interaction
```
XRGrabInteractable (base for bricks)
  ├─ Handles select events (grab/release)
  ├─ Physics-based movement
  └─ Throw velocity support

XRDirectInteractor
  └─ Provides direct hand interaction

PokeInteractor
  └─ Provides poke gesture interaction
```

---

## Component Integration Diagram

```
Scene Setup
├── XROrigin (with cameras, hands tracking)
├── XRInteractionManager
│   ├─ XRDirectInteractor (left hand)
│   ├─ XRDirectInteractor (right hand)
│   ├─ PokeInteractor (left)
│   └─ PokeInteractor (right)
│
├── AR System
│   ├─ ARFeatureController
│   │   ├─ ARCameraManager
│   │   ├─ ARPlaneManager
│   │   ├─ ARBoundingBoxManager
│   │   └─ OcclusionManager
│   │       └─ AROcclusionManager / ARShaderOcclusion
│   └─ ARAnchorManager (for persistence)
│
├── Object Management
│   ├─ ObjectSpawner
│   │   └─ Fires: objectSpawned(GameObject)
│   └─ SpawnedObjectsManager
│       ├─ Listens: objectSpawned
│       └─ SaveAndLoadAnchorDataToFile
│
├── UI System
│   ├─ GoalManager (listens to objectSpawned)
│   ├─ DebugInfoDisplayController
│   ├─ GazeTooltips
│   └─ Hand Menu (from XR Toolkit)
│
└── Lego Bricks (GameObject)
    ├─ XRGrabInteractable
    ├─ Rigidbody
    └─ Collider (BoxCollider or MeshCollider)
```

---

## Configuration Checklist for New MR Scene

### 1. XR Setup
- [ ] Drag XROrigin (AR or VR rig) into scene
- [ ] Ensure XRInteractionManager is present
- [ ] Configure hand tracking (if using hands)
- [ ] Set up controllers (if using controllers)

### 2. AR Features
- [ ] Add ARFeatureController
- [ ] Configure ARCameraManager reference
- [ ] Configure ARPlaneManager reference
- [ ] Configure ARBoundingBoxManager reference
- [ ] Add OcclusionManager if on Meta Quest
- [ ] Ensure ARAnchorManager exists in scene

### 3. Lego Bricks
- [ ] Instantiate or place Legos.fbx (or individual bricks)
- [ ] Add XRGrabInteractable component
- [ ] Configure Rigidbody (mass, drag, constraints)
- [ ] Add Collider (BoxCollider recommended)
- [ ] Assign interactable material
- [ ] Reference XRInteractionManager in interactable

### 4. Object Management
- [ ] Add ObjectSpawner to scene
- [ ] Configure objectPrefabs list with brick prefabs
- [ ] Add SpawnedObjectsManager
- [ ] Configure UI element references (dropdown, destroy button)
- [ ] Ensure ARAnchorManager is referenced
- [ ] Configure anchor text display

### 5. UI & Feedback
- [ ] Add CoachingUI prefab (if onboarding needed)
- [ ] Add HandMenu prefab (if hand-based menu needed)
- [ ] Add DebugInfoDisplayController (for development)
- [ ] Add GazeTooltips (for placement hints)
- [ ] Configure all UI references

### 6. Persistence
- [ ] Ensure SaveAndLoadAnchorDataToFile code is integrated
- [ ] Test Save/Load/Delete functionality
- [ ] Verify GUID storage on device

### 7. Testing
- [ ] Test hand tracking on device
- [ ] Test brick grabbing
- [ ] Test AR plane detection
- [ ] Test object persistence
- [ ] Test UI interactions

---

## Important Class Methods

### GoalManager
```csharp
public void CompleteGoal()              // Progress to next goal
public void ForceEndAllGoals()          // Skip to main experience
public void ResetCoaching()             // Restart tutorial
public void TooglePlayer(bool visibility) // Control video player
```

### SpawnedObjectsManager
```csharp
public void OnDestroyObjectsButtonClicked()  // Delete all objects
public async void DeleteAnchors()             // Clear persistence
public async void LoadAnchors()               // Restore objects
public async void SaveAnchors()               // Save objects
```

### ARFeatureController
```csharp
public void TogglePassthrough(bool enable)
public void TogglePlanes(bool enable)
public void TogglePlaneVisualization(bool enable)
public void ToggleBoundingBoxes(bool enable)
public bool HasBoundingBoxes()
```

### HandSubsystemManager
```csharp
public void EnableHandTracking()   // Start hand tracking
public void DisableHandTracking()  // Stop hand tracking
```

---

## Data Structures

### SpawnedObjectHelper (Tracking Individual Objects)
```csharp
public struct SpawnedObjectHelper {
    public GameObject gameObject;           // The spawned object
    public ARAnchor attachedAnchor;        // Its world anchor
    public int spawnObjectIdx;             // Which prefab (index)
    public bool isPersistent;              // Should save?
    public bool spawnWithAnchor;           // Has anchor?
    public SerializableGuid persistentGuid; // Unique ID for storage
}
```

### Goal (Tutorial Progression)
```csharp
public struct Goal {
    public GoalManager.OnboardingGoals CurrentGoal;
    public bool Completed;
}

public enum OnboardingGoals {
    Empty,           // Transitions
    FindSurfaces,    // Step 1: Detect planes
    TapSurface,      // Step 2: Spawn object
}
```

---

## Event Flow Examples

### When User Grabs a Lego Brick
```
1. Hand position detected by XRDirectInteractor
2. SphereCast finds XRGrabInteractable on brick
3. selectEntered event fired
4. XRGrabInteractable.StartGrab() called
5. Brick follows hand movement (XRGrabInteractable handles physics)
6. On release: selectExited event, Rigidbody respects throw velocity
```

### When User Spawns an Object
```
1. User taps plane (via ObjectSpawner or contact trigger)
2. ObjectSpawner creates new object from prefab
3. objectSpawned(GameObject) event fired
4. SpawnedObjectsManager receives event
5. Creates ARAnchor at spawn position
6. Adds SpawnedObjectHelper to tracking list
7. GoalManager updates progress if needed
```

### When App Restarts
```
1. SpawnedObjectsManager.Start() called
2. SaveAndLoadAnchorDataToFile initializes
3. If loadSavedAnchorsOnStart = true:
4. Loop through SavedAnchorsData GUIDs
5. ARAnchorManager.TryLoadAnchorAsync(guid)
6. For each loaded anchor, CreateObjectForLoadedAnchor
7. Spawned objects reappear in same locations
```

---

## Performance Tips

1. **Brick Count**: Keep individual brick count under 20 for 60fps
2. **Physics**: Use Continuous collision for grabbed objects only
3. **Occlusion**: Enable only on supported devices (use OcclusionManager)
4. **Plane Detection**: Disable plane visualization after initial setup
5. **Hand Tracking**: Use only when needed (HandSubsystemManager)

---

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Bricks not grabbable | Check XRGrabInteractable component and Collider |
| No hand tracking | Verify HandSubsystemManager is in scene and enabled |
| Objects don't persist | Check SaveAndLoadAnchorDataToFile integration |
| Planes not visible | Check ARFeatureController.TogglePlaneVisualization(true) |
| UI not responding | Verify XRInteractionManager reference in all interactors |
| Occlusion not working | Ensure OcclusionManager is in scene (Quest only) |

---

## Useful Testing Commands (Editor)

```csharp
// In GoalManager (Editor debug):
// Press Space to complete current goal
if (Keyboard.current.spaceKey.wasPressedThisFrame) {
    CompleteGoal();
}

// Check anchor count:
Debug.Log($"Saved anchors: {m_SaveAndLoadAnchorIdsToFile.SavedAnchorsData.Count}");

// Check hand subsystem:
HandSubsystemManager handMgr = FindAnyObjectByType<HandSubsystemManager>();
handMgr.EnableHandTracking();  // Manual enable
```

