# LEGO VR Build History System - Setup Guide

## Overview
Complete VR LEGO building system for Quest 3 with automatic build step logging and instruction manual generation.

## System Architecture

### Core Components

#### 1. **LegoBrick.cs** - Main Brick Component
- Manages snap points (studs/sockets)
- Handles VR grab/release events
- Automatically logs build steps when bricks snap together
- Integrates with `BrickIdentifier` for unique tracking
- Supports both grid-based and mesh-detected snap point generation

**Key Methods:**
- `GenerateSnapPoints()` - Creates stud/socket snap points
- `OnGrabbed()` / `OnReleased()` - VR interaction lifecycle
- `PerformSnap()` - Handles snapping logic and triggers build logging
- `GetConnectedBrickIDs()` - Returns all bricks this brick is connected to

#### 2. **LegoSnapPoint.cs** - Snap Point Component
- Represents individual studs (male) or sockets (female)
- Handles connection state tracking
- Provides visual feedback (yellow/red/blue/green markers in Play mode)
- Constants: `STUD_SPACING = 0.8f`, `SNAP_RADIUS = 0.1f`

#### 3. **BrickIdentifier.cs** - Unique Brick ID
- Assigns unique GUID to each brick instance
- Stores brick metadata (dimensions, color, name)
- Required for build history tracking

#### 4. **BuildStep.cs** - Single Build Step Data
- Serializable record of one assembly action
- Stores: brick ID, name, connected parent IDs, position, rotation, timestamp
- Supports "bridge scenario" (one brick connecting to multiple bricks)

#### 5. **BuildHistoryManager.cs** - Singleton History Manager
- Centralized storage of all build steps
- Chronological history tracking
- Export to JSON for AI consumption
- Generate AI-friendly prompts for instruction generation

#### 6. **BrickScanner.cs** - Connection Detection
- Uses LegoSnapPoint system to detect connected bricks
- Provides list of connected brick IDs
- Debug visualization in Scene view

#### 7. **BrickLibrary.cs** - Brick Spawning System
- Manages library of brick prefabs
- Spawns bricks with all components initialized
- Auto-configures BrickIdentifier, LegoBrick, BrickScanner
- Handles XR components (XRGrabInteractable)

#### 8. **BrickLibraryUI.cs** - Library Menu UI
- Displays available brick types
- Spawn buttons for each brick
- Positions UI in front of user
- Works with TextMeshPro

#### 9. **BuildHistoryUI.cs** - History Display UI
- Shows build history in real-time
- Generate AI prompt button
- Export JSON button
- Clear history functionality

## Setup Instructions

### Step 1: Initial Scene Setup

1. **Create BuildHistoryManager**
   - Create empty GameObject: `BuildHistoryManager`
   - Add component: `BuildHistoryManager.cs`
   - Check "Enable Debug Log" in Inspector

2. **Create BrickLibrary**
   - Create empty GameObject: `BrickLibrary`
   - Add component: `BrickLibrary.cs`
   - Assign Snap Point Prefab (see Step 2)

3. **Create LegoSnapManager**
   - Create empty GameObject: `LegoSnapManager`
   - Add component: `LegoSnapManager.cs` (if you have it)

### Step 2: Create Snap Point Prefab

**Option A: Using Menu Item (if CreateSnapPointPrefab.cs exists)**
- Menu: `LEGO > Create Snap Point Prefab`
- This creates `SnapPointPrefab.prefab` in Assets/Prefabs/

**Option B: Manual Creation**
1. Create empty GameObject: `SnapPointPrefab`
2. Add `LegoSnapPoint.cs` component
3. Add `SphereCollider` component:
   - Radius: `0.1`
   - Is Trigger: ✓ Checked
4. Add child GameObject: `Visual` with sphere mesh (scale 0.05)
5. Save as prefab

### Step 3: Setup Brick Prefabs

For each LEGO brick mesh:

1. **Import/Place Brick Model**
   - Import FBX or create brick mesh
   - Ensure mesh has proper collider

2. **Add Required Components** (or use BrickLibrary auto-init)
   - `BrickIdentifier` - will auto-generate
   - `LegoBrick` - assign snap point prefab
   - `BrickScanner` - optional but recommended
   - `Rigidbody` - auto-added if missing
   - `BoxCollider` - for physics
   - `XRGrabInteractable` - for VR grabbing
   - `LegoXRGrabbable` - bridges XR events to LegoBrick

3. **Configure LegoBrick**
   - Assign `Snap Point Prefab` from Step 2
   - Set `width` and `length` (in studs)
   - Set `height` (0.2m default)
   - Right-click component → "Generate Snap Points Now"

4. **Configure BrickIdentifier**
   - Set `studsLength` and `studsWidth`
   - Set `brickColor` and `brickName`

5. **Save as Prefab**
   - Drag to Prefabs folder
   - Each instance will get unique ID at runtime

### Step 4: Populate Brick Library

1. Select `BrickLibrary` GameObject
2. In Inspector, expand "Brick Prefabs" list
3. For each brick type:
   - Add new element
   - Set `Display Name` (e.g., "2x4 Red Brick")
   - Assign `Prefab` reference
   - Set `Dimensions` (e.g., X=2, Y=4)
   - Set `Color`
   - Optional: Assign icon sprite

### Step 5: Setup UI Canvas (Optional)

#### Build History UI:
1. Create Canvas (World Space or Screen Space Camera)
2. Add Panel: `HistoryPanel`
3. Add `BuildHistoryUI.cs` to panel
4. Create child TextMeshPro: `HistoryText`
5. Create child TextMeshPro: `AIPromptText`
6. Create buttons:
   - Refresh Button → link to `RefreshDisplay()`
   - Generate Prompt → link to `GenerateAIPrompt()`
   - Clear History → link to `ClearHistory()`
   - Export JSON → link to `ExportJSON()`

#### Brick Library UI:
1. Create Canvas (World Space for VR)
2. Add Panel: `LibraryPanel`
3. Add `BrickLibraryUI.cs` to panel
4. Create child ScrollView or Grid Layout for buttons
5. Create button prefab with TextMeshPro text
6. Assign references in Inspector

### Step 6: XR Interaction Setup (Quest 3)

1. **Install Packages** (if not already):
   - XR Interaction Toolkit
   - Meta XR SDK (Oculus Integration)

2. **Setup XR Origin**
   - Add XR Origin (Action-based or Device-based)
   - Add XR Interaction Manager

3. **Controller Setup**
   - Left/Right Hand Controllers with ray interactors
   - Or Hand Tracking with Direct/Ray Interactors

4. **Test Grabbing**
   - Play mode → grab brick with controller/hand
   - Should see OnGrabbed/OnReleased logs

### Step 7: Testing

1. **Test Spawning**
   - Select BrickLibrary → Context Menu: "Spawn Test Brick"
   - Should spawn brick in front of camera

2. **Test Snapping**
   - Spawn two bricks
   - Move one near the other
   - Release → should snap with sound/haptic
   - Check yellow gizmos turn green when connected

3. **Test Build History**
   - Snap several bricks together
   - Check Console for build step logs
   - Select BuildHistoryManager → Context Menu: "Print History"

4. **Test UI**
   - Open library UI → click brick buttons
   - Open history UI → see logged steps
   - Click "Generate AI Prompt" → check clipboard

## Configuration Reference

### LegoBrick Settings
```
width: 2               // studs wide
length: 4              // studs long
height: 0.2            // meters (VR scale)
snapPointPrefab: [SnapPointPrefab]
useDetectedStudPositions: false  // use mesh detection vs grid
```

### LegoSnapPoint Constants
```csharp
STUD_SPACING = 0.8f;   // meters between stud centers
SNAP_RADIUS = 0.1f;    // detection radius
Socket radius = 0.2f;  // 2x stud radius (set in LegoBrick)
```

### BuildHistoryManager Settings
```
maxHistorySize: 0      // 0 = unlimited
enableDebugLog: true   // console logging
```

## Workflow

### Build Session Flow:
1. User spawns brick from library → `BrickLibrary.SpawnBrick()`
2. Brick initialized with unique ID → `BrickIdentifier.Awake()`
3. Snap points generated → `LegoBrick.GenerateSnapPoints()`
4. User grabs brick → `LegoXRGrabbable` → `LegoBrick.OnGrabbed()`
5. User positions near other brick → studs/sockets detect proximity
6. User releases → `LegoBrick.OnReleased()` → `TrySnapToNearby()`
7. If snap successful → `PerformSnap()` → `LogBuildStep()`
8. BuildStep created with connected parent IDs → `BuildHistoryManager.AddBuildStep()`
9. Repeat steps 1-8 for entire build

### Export Flow:
1. Build complete
2. Click "Generate AI Prompt" → `BuildHistoryManager.GenerateAIPrompt()`
3. Or click "Export JSON" → `BuildHistoryManager.ExportToJSON()`
4. Send to AI API (GPT-4, Claude, etc.)
5. AI generates step-by-step instructions
6. Display instructions in-game or export PDF

## Advanced Features

### Mesh-Based Stud Detection
- Set `useDetectedStudPositions = true` on LegoBrick
- Scans mesh vertices to find stud positions
- Validates spacing and clusters nearby vertices
- Falls back to grid if detection fails

### Bridge Scenario Support
- One brick can connect to multiple bricks below
- `BuildStep.connectedParentIDs` stores all parent connections
- `GetConnectedBrickIDs()` returns list via socket connections

### Visual Feedback
- **Scene View**: Yellow wire spheres (gizmos) show snap radius
- **Play Mode**: 
  - Red crosses = studs (disconnected)
  - Blue crosses = sockets (disconnected)
  - Green crosses = connected points
  - Green lines = connections

### Haptic Feedback
- Automatic controller vibration on snap (Quest 3)
- Uses reflection to avoid XR Toolkit dependency

## Troubleshooting

### Bricks Don't Snap
- Check snap point prefab is assigned
- Verify snap points generated (yellow spheres visible)
- Check SNAP_RADIUS isn't too small
- Ensure both bricks have LegoSnapPoint components
- Check colliders are triggers

### No Build History Logged
- Verify BuildHistoryManager exists in scene
- Check BrickIdentifier component exists on bricks
- Look for error logs in Console
- Ensure PerformSnap() is being called

### Snap Points Clustered at Center
- Check STUD_SPACING matches your mesh scale
- If bricks scaled 0.1x, use STUD_SPACING = 0.08f
- Verify width/length values match actual studs

### XR Grabbing Not Working
- Install XR Interaction Toolkit
- Ensure XRGrabInteractable component exists
- Check LegoXRGrabbable is present
- Verify XR Origin and controllers setup

### UI Not Visible
- Canvas in World Space for VR
- Check Canvas scale (0.001 recommended)
- Position UI in front of camera
- Verify TextMeshPro package installed

## Performance Tips

1. **Limit Snap Point Count**: Use appropriate brick dimensions
2. **Optimize Colliders**: Box colliders for bricks, sphere for snap points
3. **Disable Unused Gizmos**: Turn off OnDrawGizmos in builds
4. **Parent Connected Bricks**: Reduces physics calculations
5. **Pool Bricks**: Reuse destroyed bricks instead of instantiating

## API Quick Reference

### Spawn Brick
```csharp
BrickLibrary.Instance.SpawnBrickInFrontOfUser(0);
```

### Get Build History
```csharp
List<BuildStep> steps = BuildHistoryManager.Instance.GetAllSteps();
```

### Generate AI Prompt
```csharp
string prompt = BuildHistoryManager.Instance.GenerateAIPrompt();
```

### Export JSON
```csharp
string json = BuildHistoryManager.Instance.ExportToJSON();
```

### Manual Snap Point Generation
```csharp
GetComponent<LegoBrick>().GenerateSnapPoints();
```

## File Structure
```
Assets/
├── Scripts/
│   ├── LegoBrick.cs
│   ├── LegoSnapPoint.cs
│   ├── LegoSnapManager.cs
│   ├── LegoXRGrabbable.cs
│   ├── BrickIdentifier.cs
│   ├── BrickScanner.cs
│   ├── BrickLibrary.cs
│   ├── BrickLibraryUI.cs
│   ├── BuildStep.cs
│   ├── BuildHistoryManager.cs
│   └── BuildHistoryUI.cs
├── Prefabs/
│   ├── SnapPointPrefab.prefab
│   └── Bricks/
│       ├── 2x4_Red.prefab
│       ├── 2x2_Blue.prefab
│       └── ...
└── Scenes/
    └── MainScene.unity
```

## Next Steps for Hackathon

1. **Create More Brick Prefabs** - variety of sizes/colors
2. **Design Library UI** - scrollable grid with icons
3. **Implement "Ghost Preview"** - show where brick will snap
4. **Add Undo/Redo** - manipulate BuildHistory stack
5. **Instruction Replay** - animate build sequence
6. **AI Integration** - send JSON to GPT-4/Claude
7. **Export to PDF** - generate printable instructions
8. **Multi-user** - Photon/Netcode for shared building
9. **Hand Menu** - library attached to hand
10. **Plane Detection** - snap first brick to real desk

## Demo Script (2 Minutes)

1. **Show Library** (15s)
   - "Here's our virtual LEGO library"
   - Gesture to floating menu
   - Point at different brick types

2. **Spawn & Snap** (30s)
   - Pinch/grab to spawn brick
   - Place on desk (plane detection)
   - Spawn second brick
   - Show snap preview
   - Release → satisfying click + haptic

3. **Build Structure** (45s)
   - Quickly add 4-5 more bricks
   - Show bridge scenario
   - Mention "every action is logged"

4. **Generate Instructions** (30s)
   - Open history UI
   - Show chronological steps
   - Click "Generate Instructions"
   - Display AI-generated guide
   - "Perfect for training, prototyping, collaboration"

Total: Exactly 2 minutes with buffer

## Judges' Questions & Answers

**Q: How does this scale beyond LEGO?**
A: Same system works for industrial assembly, furniture, architecture, robotics training - any modular construction task.

**Q: What's the technical challenge?**
A: Spatial relationship tracking, multi-parent connections, real-time logging, and automatic instruction generation from interaction data.

**Q: Why Quest 3 specifically?**
A: Hand tracking for natural interaction, passthrough for mixed reality workspace, spatial anchors for persistence.

**Q: Can multiple people use this?**
A: Yes, easily extended with Photon/Netcode for collaborative building sessions.

---

## Credits
Built for Quest 3 Mixed Reality Hackathon
Uses Unity XR Interaction Toolkit + Meta XR SDK
