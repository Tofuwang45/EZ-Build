# Unity MR Lego Project - Comprehensive Codebase Analysis

## Project Overview
- **Framework**: XR Interaction Toolkit 3.2.1 with OpenXR and Meta/Android XR support
- **Target**: Mixed Reality (MR) platform supporting both Meta Quest and Android XR devices
- **Main Assets Directory**: `/home/user/lego/ITB/Assets`
- **Project Type**: Mobile AR/MR application for lego brick interaction

---

## 1. MR COMPONENTS & XR HAND TRACKING

### 1.1 Core MR Infrastructure Components

#### HandSubsystemManager.cs (74 lines)
**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Scripts/`
**Purpose**: Manages XR Hand Tracking subsystem lifecycle
**Key Features**:
- Enables/disables hand tracking via `EnableHandTracking()` and `DisableHandTracking()`
- Finds and manages running XRHandSubsystem instances
- Provides convenient wrapper for external hand tracking control

#### ARFeatureController.cs (260 lines)
**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Scripts/`
**Purpose**: Central controller for AR features (passthrough, planes, bounding boxes, occlusion)
**Key Features**:
- Controls ARCameraManager for passthrough visualization
- Manages ARPlaneManager for plane detection
- Manages ARBoundingBoxManager for object detection
- Controls OcclusionManager for occlusion rendering
- Provides events: onARPassthroughFeatureChanged, onARPlaneFeatureChanged, onARPlaneFeatureVisualizationChanged
- Methods: TogglePassthrough(), TogglePlanes(), TogglePlaneVisualization(), ToggleBoundingBoxes()

#### OcclusionManager.cs (220 lines)
**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Scripts/`
**Purpose**: Manages AR occlusion features (shader occlusion support)
**Key Features**:
- Handles Meta Occlusion and Android XR occlusion setup
- Configures Quest-specific and Android XR-specific settings
- Manages AROcclusionManager and ARShaderOcclusion
- SetupManager() method initializes occlusion rendering

#### SaveAndLoadAnchorDataToFile.cs (118 lines)
**Purpose**: Persistent anchor storage and loading
**Key Features**:
- Saves/loads anchor GUIDs to/from device storage
- Supports persistent object placement across sessions

### 1.2 Interaction & Visualization Components

#### XRBlaster.cs (83 lines)
**Purpose**: Interactive blaster object inheriting XRGrabInteractable
**Key Features**:
- Grab-based interaction with rotation logic
- Points toward interactor during grab
- Event listeners: selectEntered, selectExited

#### XRPokeFollowAffordanceFill.cs (234 lines)
**Purpose**: Visual affordance for poke interactions
**Key Features**:
- Provides visual feedback for poke gesture detection
- Fills based on pointer follow distance

#### GazeTooltips.cs (111 lines)
**Purpose**: Dynamic tooltip placement based on gaze and raycast
**Key Features**:
- Uses SphereCast to detect placeable surfaces
- Positions tooltips on detected planes
- Integrates with ARContactSpawnTrigger

#### DebugInfoDisplayController.cs (157 lines)
**Purpose**: Displays AR feature debug information
**Key Features**:
- Shows plane count, bounding box count
- Displays AR status on UI

#### PinchVizController.cs (696 lines)
**Purpose**: Visualizes hand pinch gestures
**Key Features**:
- Shows pinch point visualization
- Works with hand tracking data

### 1.3 Prefabs (MR Interaction Setup)

**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Prefabs/`

Key Prefabs:
- **MR Interaction Setup.prefab** (147KB) - Complete XR interaction setup
- **Left/Right Controller.prefab** - Controller models
- **Contact Spawn Trigger.prefab** - Spawning trigger on contact
- **Permissions Manager Variant.prefab** - Handles device permissions

---

## 2. LEGO BRICK IMPLEMENTATIONS & INTERACTABLE SYSTEMS

### 2.1 Lego Brick Models

**Location**: `/home/user/lego/ITB/Assets/LegoBricks/`

- **Legos.fbx** (1.5MB)
  - Main lego brick 3D model asset
  - Imported from Blender/Cinema 4D
  - Contains various brick pieces

- **ExtraFiles/**: Alternative formats
  - Legos.fbx, LEGO'S.3ds, LEGO'S.obj, LEGO'S.c4d
  - Backup formats for different 3D software

### 2.2 Interactive Bricks Setup (version1.unity)

**Scene**: `/home/user/lego/ITB/Assets/version1.unity` (1264 lines)

**Key Components in version1 Scene**:
- XR Origin (VR) with hand tracking setup
- Legos model instantiated as interactive object
- XRDirectInteractor configured for direct hand interaction
- Direct grab interaction enabled
- Physics-based interaction with grab support

**Recent Work** (commit 05c7aafe - "Made the bricks interactive"):
- Integrated Legos.fbx into scene
- Added XRDirectInteractor component
- Set up hand grab interaction
- Materials configured (FresnelHighlight, Interactable)

### 2.3 Object Spawning & Management

#### SpawnedObjectsManager.cs (433 lines)
**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Scripts/`
**Purpose**: Manages spawned objects with persistent anchor support
**Key Features**:
- Spawns objects with persistent anchors
- Tracks spawned objects with helper struct
- Save/Load/Delete anchor operations (async)
- UI Integration: object selector dropdown, destroy button
- Persistent GUID tracking: SpawnedObjectHelper struct
```csharp
struct SpawnedObjectHelper {
    public GameObject gameObject;
    public ARAnchor attachedAnchor;
    public int spawnObjectIdx;
    public bool isPersistent;
    public bool spawnWithAnchor;
    public SerializableGuid persistentGuid;
}
```

#### ObjectSpawner (From XR Interaction Toolkit Samples)
**Location**: `/home/user/lego/ITB/Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/Scripts/`
**Features**:
- Spawns objects at raycast/contact points
- Supports randomization
- Event: objectSpawned(GameObject)
- Properties: objectPrefabs list, spawnOptionIndex

### 2.4 Interactable Prefabs in Samples

**AR Demo Scene Assets**: `/home/user/lego/ITB/Assets/Samples/XR Interaction Toolkit/3.2.1/AR Starter Assets/ARDemoSceneAssets/Prefabs/`
- Arch.prefab, Cube.prefab, Cylinder.prefab, Pyramid.prefab, Torus.prefab, Wedge.prefab

**Hands Interaction Demo**: Includes poke interactions and grab examples

**Materials for Interactables**:
- Location: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/Primitive/`
- Interactables.mat (base material with 6 variants)
- Interactables.physicMaterial, Interactables_Bouncy.physicMaterial
- Shader: InteractablePrimitive.shadergraph

---

## 3. UI FEATURES (PREVIOUSLY MADE)

### 3.1 UI Prefabs Collection

**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Prefabs/UI/`

#### Coaching & Onboarding UI
- **CoachingUI.prefab** (273KB) - Comprehensive coaching interface
  - Tutorial steps display
  - Skip/Continue buttons
  - Learn modal dialogs

#### Hand Menu System
- **HandMenuSetupVariant_MRTemplate.prefab** (248KB)
  - Hand-anchored UI menu system
  - For hand interaction menu display

#### Interactive Controls
- **ListItemButton.prefab** - Clickable list items (25KB)
- **ListItemToggle.prefab** - Toggle controls (21KB)
- **ListItemBooleanToggle.prefab** - Boolean toggle (23KB)
- **ListItemSlider.prefab** - Slider controls (29KB)
- **ListItemDropdown.prefab** - Dropdown selector (10KB)

#### Visual Feedback
- **TapVisualization.prefab** (125KB) - Visual feedback for taps
- **Tooltip.prefab** (18KB) - World-space tooltips
- **TooltipWorldspace.prefab** (9KB) - Alternative tooltip style
- **IndexTipPrefab.prefab** (12KB) - Index finger tip visual
- **Toggle_MRTemplate.prefab** (23KB) - MR-styled toggle

#### Advanced UI Components
- **SpatialPanelScroll.prefab** (49KB) - Scrollable spatial panel
- **Dropdown.prefab** (42KB) - Dropdown menu

### 3.2 Coaching & Goal System

#### GoalManager.cs (472 lines)
**Purpose**: Manages onboarding goals and coaching flow
**Key Goals**:
```csharp
public enum OnboardingGoals {
    Empty,          // Welcome/End steps
    FindSurfaces,   // Find AR planes
    TapSurface,     // Tap to spawn object
}
```

**Features**:
- Queue-based goal progression
- Step-by-step tutorial
- Video player integration
- Feature controller integration (planes, passthrough)
- Learn modal system
- Auto-progression on object spawn

**Methods**:
- `CompleteGoal()` - Move to next goal
- `ForceEndAllGoals()` - Skip to main experience
- `ResetCoaching()` - Restart tutorial

**UI Integration**:
- StepButtonTextField - Text for button labels
- SkipButton - Skip current step
- LearnButton - Open learning modal
- LearnModal - Additional learning information
- TapTooltip - Visual hint for interactions

### 3.3 Video Player UI

**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Videos/`

#### VideoTimeScrubControl.cs (191 lines)
**Purpose**: Video playback with timeline scrubbing
**Features**:
- Timeline slider control
- Play/Pause functionality
- Time display UI
- Smooth seeking

#### VideoPlayerRenderTexture.cs (57 lines)
**Purpose**: Render video to texture for 3D display

### 3.4 Visualization Controllers

#### BooleanToggleVisualsController.cs (90 lines)
- Toggles visual elements based on boolean state

#### FadeMaterial.cs (1674 lines)
**Purpose**: Fades material transparency
**Features**: Coroutine-based fade animations

#### FadePlaneMaterial.cs (110 lines)
**Purpose**: Fade AR plane materials specifically

---

## 4. HISTORY FEATURES (PREVIOUSLY MADE)

### 4.1 Persistent Anchor System (History of Placed Objects)

The project uses AR Anchors as the primary history mechanism:

#### SaveAndLoadAnchorDataToFile.cs
**Purpose**: Maintains history of spawned objects
**Features**:
- Saves anchor GUIDs and associated object types to device storage
- Dictionary: `SavedAnchorsData<SerializableGuid, int>`
  - Key: Unique anchor GUID
  - Value: Spawn object index (which prefab was spawned)

**Methods**:
- `SaveAnchorIdAsync(guid, spawnIndex)` - Record placement
- `EraseAnchorIdAsync(guid)` - Remove from history
- `LoadAnchorIdAsync()` - Restore saved placements

#### SpawnedObjectsManager History Operations
**UI Methods Available**:
- `LoadAnchors()` - Restore all previously placed objects
- `SaveAnchors()` - Persist current objects
- `DeleteAnchors()` - Clear history
- `OnDestroyObjectsButtonClicked()` - Delete single object

**History Display**:
- m_AnchorText: TextMeshProUGUI that displays:
  - Currently saved objects
  - Successfully loaded objects
  - Load/Save/Delete status messages
  - Object names and GUIDs

**Data Structure**:
```csharp
struct SpawnedObjectHelper {
    public SerializableGuid persistentGuid;  // Unique ID for history
    public int spawnObjectIdx;               // Which prefab type
    public bool isPersistent;                // Mark for persistence
}
```

### 4.2 Debug Info for History

#### DebugInfoDisplayController.cs (157 lines)
**Purpose**: Display AR tracking information
**Features**: Shows counts of detected planes and objects (useful for debugging placement history)

---

## 5. EXISTING SCENE FILES & STRUCTURE

### 5.1 Main Scenes

| Scene | Location | Status | Purpose |
|-------|----------|--------|---------|
| **SampleScene** | `/home/user/lego/ITB/Assets/Scenes/SampleScene.unity` | Current | Main project scene (recently updated) |
| **version1** | `/home/user/lego/ITB/Assets/version1.unity` | Prototype | Interactive bricks prototype with XR setup |
| **_Recovery/0** | `/home/user/lego/ITB/Assets/_Recovery/0.unity` | Backup | Recovery scene from package updates |

### 5.2 XR Toolkit Sample Scenes (Reference)

- **DemoScene.unity** - VR Starter Assets demo with full interactables
- **HandsDemoScene.unity** - Hand tracking demo with poke/grab interactions
- **ARDemoScene.unity** - AR-focused demo with plane detection
- **HandVisualizer.unity** - Hand joint visualization

### 5.3 Scene Infrastructure

**XROrigin (VR) - Main XR Setup in version1.unity**:
```
XROrigin (VR)
├── Main Camera
├── DirectionalLight
├── XRInteractionManager
├── Hand Tracking (Left/Right)
├── Input Actions Handler
├── Controllers (Left/Right)
└── Contact Spawn Triggers
```

---

## 6. PREFABS & GAME OBJECTS - LEGO BRICKS

### 6.1 Lego Brick Related Prefabs

**Direct Lego Support**:
- Legos.fbx → Auto-generates prefab on import
  - Can be instantiated in scenes
  - Supports physics rigidbody
  - Works with XRGrabInteractable

### 6.2 Interactive Base Prefabs (Used for Bricks)

These prefabs can be modified to work with lego bricks:

**From MRTemplateAssets**:
- **TestObject.prefab** (16KB) - Generic interactable test object
- **SittingCylinder.prefab** (18KB) - Cylindrical interactable
- **TotemFloatingTorus.prefab** (78KB) - Complex interactable with visual feedback
- **TotemKinetic.prefab** (44KB) - Physics-based interactable
- **TotemBlaster.prefab** (49KB) - Blaster variant with projectiles

**From XR Interaction Toolkit Samples - Hands Demo**:
- **Cube.prefab** (physics, interactable)
- **Cylinder.prefab** (physics, interactable)
- **Disc.prefab** (flat interactable)
- **Arrow.prefab** (directional object)
- **AudioAffordance.prefab** (with audio feedback)

**Socket/Connector Prefabs**:
- **SimpleSocket.prefab** - Socket for object attachment
- **SimpleSocketShape.prefab** - Shape-specific socket

### 6.3 XRGrabInteractable Configuration for Bricks

Base class for all grabable lego bricks:
```
Component: XRGrabInteractable
├── Interaction Layers: Default (all layers)
├── Interaction Manager: XRInteractionManager reference
├── Grab Settings:
│   ├── Movement Type: Kinematic
│   ├── Throw on Detach: Enabled
│   ├── Throw Velocity Scale: 1.5
│   └── Throw Angular Velocity Scale: 1.0
├── Rigidbody: Configured for physics
│   ├── Mass: 0.1 (light brick)
│   ├── Constraints: Freeze Z rotation (optional)
│   └── Collision Detection: Continuous
└── Collider: Box/Mesh collider (matches brick shape)
```

### 6.4 Interactable Materials

**Location**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/Primitive/`

Materials suitable for lego bricks:
- **Interactables.mat** - Base material (white/light)
- **Interactables_2.mat** - Color variant
- **Interactables_3.mat** - Color variant
- **Interactables_4.mat** - Color variant
- **Interactables_5.mat** - Color variant
- **Interactables_6.mat** - Color variant
- **Interactables_Bouncy.physicMaterial** - Bouncy physics variant

**Shader**: `InteractablePrimitive.shadergraph`
- Features: PBR material, highlight support, color variations

---

## 7. KEY INPUT ACTIONS & CONTROLS

**Input Actions File**: `/home/user/lego/ITB/Assets/MRTemplateAssets/MRTemplateInputActions.inputactions`

Standard XR Interaction Toolkit mappings:
- **Select** - Grab/Select action
- **Select Value** - Analog grab value
- **Activate** - Secondary action
- **Activate Value** - Analog activate value
- **UI Submit/Cancel** - Menu interactions

---

## 8. MATERIAL & RENDERING SETUP

### 8.1 Rendering Pipeline
- **Universal Render Pipeline (URP)** - Lightweight rendering
- **Settings**: `/home/user/lego/ITB/Assets/Settings/Project Configuration/UniversalRenderPipelineGlobalSettings.asset`

### 8.2 AR-Specific Materials

**AR Plane Materials**:
- Location: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/AR/`
- Supports plane visualization and fading

### 8.3 Controllers & UI Materials
- **Controller Materials**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/Controller/`
- **UI Materials**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/UI/`
- **Pointer Materials**: `/home/user/lego/ITB/Assets/MRTemplateAssets/Materials/Pointer/`

---

## 9. PROJECT DEPENDENCIES & PACKAGES

### Core XR Packages
1. **XR Interaction Toolkit** (v3.2.1)
2. **XR Hands** (v1.7.0)
3. **OpenXR Plugin** - Cross-platform XR support
4. **Meta XR Features** - Meta Quest specific features
5. **Android XR** - Android XR platform support

### Supporting Packages
- **TextMeshPro** - Text rendering
- **UI Toolkit** - UI system
- **ARFoundation** - AR feature abstraction
- **Universal Render Pipeline** - Rendering

---

## 10. GIT HISTORY & MIGRATION STATUS

### Recent Commits
1. **a4cddacf** - "Update necessary packages" (Nov 15)
2. **d3e9dcbc** - "Updated XR Hands" (Nov 14)
3. **05c7aafe** - "Made the bricks interactive" (Nov 14) ← Lego integration
4. **7d749cb9** - "update sample scene" (Nov 14)
5. **7c50b08b** - "Start migrating legos" (Nov 14) ← Initial lego import

### Active Branch
- `claude/create-mr-scene-01NRLvBe7WtfAEEZajqvGV1t`
- Based on: `a4cddacf` (latest updates)

---

## 11. RECOMMENDED STRUCTURE FOR MR SCENE

Based on the existing components, here's how to structure a complete MR lego scene:

```
MR Lego Scene (New)
├── XR Interaction Setup
│   ├── XRInteractionManager
│   ├── XROrigin (MR)
│   ├── Hands Tracking (L/R)
│   └── Controllers (L/R)
├── AR Feature Controllers
│   ├── ARFeatureController
│   ├── OcclusionManager
│   ├── ARPlaneManager
│   └── ARBoundingBoxManager
├── Lego Bricks Container
│   ├── LegoBrick_01 (XRGrabInteractable)
│   ├── LegoBrick_02 (XRGrabInteractable)
│   └── ... (more bricks)
├── Object Management
│   ├── SpawnedObjectsManager
│   └── ObjectSpawner
├── UI System
│   ├── CoachingUI (if tutorial needed)
│   ├── Hand Menu
│   ├── Debug Info Display
│   └── Tooltips
├── Visualization
│   ├── AR Planes (visual)
│   ├── Pinch Visualization
│   └── Affordance Feedback
└── Lighting & Environment
    ├── Directional Light
    └── AR Passthrough (if needed)
```

---

## 12. NEXT STEPS FOR COMPLETE MR SCENE

To create a complete working MR scene with lego bricks:

1. **Brick Setup**:
   - Import Legos.fbx as individual brick prefabs (separate models)
   - Add XRGrabInteractable to each brick type
   - Configure physics (mass, colliders, constraints)
   - Apply interactable materials

2. **Hand Interaction**:
   - Enable XRDirectInteractor and PokeInteractor in XR Rig
   - Configure hand tracking from HandVisualizer sample
   - Set up hand affordances (pinch visualization)

3. **Object Management**:
   - Integrate SpawnedObjectsManager for persistence
   - Configure ObjectSpawner for brick spawning
   - Connect to UI for object selection

4. **UI Integration**:
   - Add HandMenuSetupVariant for brick selection menu
   - Include CoachingUI for user guidance
   - Add debug info display for troubleshooting

5. **AR Features**:
   - Enable plane detection for placement
   - Configure passthrough
   - Set up occlusion for realistic rendering

6. **Persistence**:
   - Enable SaveAndLoadAnchorDataToFile for saving placements
   - Test across app sessions

