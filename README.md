# ITB - Interactive LEGO Assembly System

A Unity XR application for tracking and assembling LEGO-style blocks with AI-powered instruction generation.

## Features

### 🧱 Block Tracking System
- **Real-time Position Tracking**: Monitors all LEGO blocks in the scene with sub-millimeter precision
- **Connection Detection**: Automatically detects when blocks snap together via FixedJoints
- **Assembly Groups**: Identifies connected assemblies and tracks their relationships
- **Debug Visualization**: Live on-screen display showing block positions and connection pairs

### 🤖 AI Instruction Generation
- **Gemini Integration**: Uses Google's Gemini 2.0 Flash model to generate assembly instructions
- **Context-Aware**: Analyzes block positions and connections to create step-by-step guides
- **Real-time Output**: Displays generated instructions in the VR environment

### 📊 Tracking Components

#### LegoSceneTracker
Central tracking system that:
- Scans for `LegoBrick` and `LegoSnapPerfect` components
- Records transform history with configurable snapshot intervals
- Builds connection graphs from FixedJoint components
- Exposes data via singleton pattern: `LegoSceneTracker.Instance`

**Key Properties:**
- `BrickCount` - Total tracked blocks
- `SnappedBrickCount` - Blocks in assemblies
- `ConnectedGroups` - List of all connected assemblies
- `TrackedStates` - Individual block states with position history

#### LegoTrackerDebugView
Visual debug panel that displays:
- Block names and positions (x, y, z coordinates)
- Connection pairs showing which blocks are snapped together
- Real-time updates every frame
- Console logging for debugging

#### LegoSnapPerfect
Snapping logic for blocks:
- Auto-detects stud and socket connection points
- Grid-based alignment (configurable grid size)
- Creates FixedJoint connections when blocks snap
- Audio feedback support

## Setup

### Prerequisites
- Unity 6.0 or later
- XR Interaction Toolkit
- TextMeshPro
- Google Gemini API key

### Configuration

#### 1. Scene Setup
Add the following GameObjects to your scene:

**MainCube** (Tracker):
```
MainCube
├── LegoSceneTracker (Script)
└── DebugInfoDisplayController
```

**InfoDisplay** (Debug UI):
```
InfoDisplay
├── DebugInfoDisplayController (Script)
├── LegoTrackerDebugView (Script)
└── Canvas
    └── Values (TextMeshProUGUI)
```

#### 2. Tracker Configuration
Select `MainCube` and configure `LegoSceneTracker`:
- ✅ Auto Capture Snapshots
- Snapshot Interval: `0.5` seconds
- Max History Entries: `32`
- Movement Threshold: `0.002` meters
- Angle Threshold: `0.5` degrees
- ✅ Log Snapshots (for debugging)

#### 3. Debug View Setup
On `InfoDisplay` → `LegoTrackerDebugView`:
- **Debug Display**: Assign `DebugInfoDisplayController` component
- **Tracker**: Leave empty (auto-finds `LegoSceneTracker.Instance`)

#### 4. AI API Setup
Create an `AI API` GameObject:
```
AI API
└── APICallInstruction (Script)
```

Configure `APICallInstruction`:
- **Api Endpoint**: `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent`
- **System Prompt**: (customizable instruction generation prompt)
- **Input Text Object**: Assign TextMeshProUGUI for input
- **Output Text Object**: Assign TextMeshProUGUI for output
- ✅ Auto Save Output

The API key is hardcoded: `AIzaSyBOs16qTMT0W93GbWT3dJqiDrZO1YjZ8XA`

#### 5. Block Setup
Add to each LEGO block GameObject:
- `LegoSnapPerfect` component
- `Rigidbody` component
- Child GameObjects named with "stud" and "socket" for connection points

Configure `LegoSnapPerfect`:
- Snap Distance: `0.1`
- Snap Speed: `10`
- Grid Size: `0.008` (LEGO standard)
- ✅ Use Grid Snapping

## Usage

### Tracking Blocks
1. Add blocks to the scene with `LegoSnapPerfect` components
2. `LegoSceneTracker` automatically detects and tracks them
3. Snap blocks together - FixedJoints are created automatically
4. View real-time data in the debug panel or console

### Generating Instructions
1. Snap blocks together to create an assembly
2. The tracker captures position and connection data
3. Feed this data to `AI Input` TextMeshProUGUI
4. Call `CallAI()` method (via button or script)
5. Instructions appear in `OutputText`

### API Methods

**LegoSceneTracker:**
```csharp
// Manual refresh
LegoSceneTracker.Instance.RefreshTrackedBricks();

// Force snapshot
LegoSceneTracker.Instance.CaptureSnapshot("Manual");

// Rebuild connections
LegoSceneTracker.Instance.RecalculateConnections();

// Get state by component
LegoSceneTracker.Instance.TryGetState(brick, out var state);
```

**APICallInstruction:**
```csharp
// Call AI with current input
apiScript.CallAI();

// Retry last call
apiScript.TryAgain();

// Get last response
string response = apiScript.GetLastResponse();
```

## Debug Output Format

### Positions
```
MainCube (1): (0.000, 0.500, 0.000)
Cube_Red: (-0.250, 0.500, 0.000)
Cube_Blue: (0.250, 0.500, 0.000)
```

### Connections
```
--- 2 Connection(s) ---
Pair 1: MainCube (1) <-> Cube_Red (FixedJoint)
Pair 2: MainCube (1) <-> Cube_Blue (FixedJoint)
```

## Troubleshooting

### No blocks detected
- Ensure blocks have `LegoSnapPerfect` or `LegoBrick` components
- Check that `MainCube` has `LegoSceneTracker` component
- Enable "Log Snapshots" and check console

### No connections shown
- Verify FixedJoint components exist on snapped blocks
- Check that `connectedBody` references are set
- Enable debug logging in `LegoSceneTracker`
- Look for `[LegoSceneTracker]` logs showing joint detection

### API not working
- Check console for `[API DEBUG]` messages
- Verify API key is correct
- Ensure input text is not empty
- Check response code (200 = success, 401 = auth error)
- See "Failed:" messages in output text for specific errors

### Debug panel empty
- Ensure `DebugInfoDisplayController` is assigned
- Check that `Values` TextMeshProUGUI component is referenced
- Verify tracker is finding blocks (check `BrickCount` in Inspector)

## File Structure

```
ITB/
├── Assets/
│   ├── Scripts/
│   │   ├── LegoSceneTracker.cs       # Main tracking system
│   │   ├── LegoTrackerDebugView.cs   # Debug visualization
│   │   ├── SNAP.cs                   # LegoSnapPerfect snapping logic
│   │   ├── APICallInstruction.cs     # AI integration
│   │   └── GridCubeSnap.cs          # Alternative snapping (unused)
│   └── Scenes/
│       └── FinalScene.unity          # Main scene
├── README.md
└── LICENSE
```

## Packages & Dependencies

### Unity Packages
- **Unity 6.0** (6000.0.12f1) - Core engine
- **XR Interaction Toolkit** - VR/AR interaction system
- **TextMeshPro** - UI text rendering
- **Unity XR Hands** - Hand tracking support
- **Mixed Reality Template Assets** - Base MR setup

### External APIs
- **Google Gemini API 2.0 Flash** - AI instruction generation
  - Endpoint: `generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent`

### AI Tools Used
- **GitHub Copilot** - Code assistance and generation
- **Claude Sonnet 4.5** (via GitHub Copilot) - Architecture design, debugging, and documentation

### Assets
- Unity Standard Assets
- MR Template starter assets
- Custom LEGO-style block models

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## API Keys & Security

⚠️ **Note**: The Gemini API key is currently hardcoded for development. For production:
1. Move API key to environment variables
2. Use Unity's PlayerPrefs or secure storage
3. Implement key rotation
4. Consider backend proxy for API calls

## Acknowledgments

- Built with Unity XR Interaction Toolkit
- AI-powered by Google Gemini
- Development assisted by GitHub Copilot (Claude Sonnet 4.5)
