# VR UI System - Quick Start Guide

## 🚀 One-Click Setup

The VR UI System now includes an **automatic bootstrap** that sets everything up for you!

## Method 1: Unity Menu (Recommended)

1. Open your scene in Unity
2. Go to **Tools → VR UI System → Auto Setup VR UI**
3. Done! The VR UI is now attached to your left hand

## Method 2: Bootstrap Component

1. Create an empty GameObject in your scene
2. Add the `VRUISystemBootstrap` component to it
3. Check "Auto Setup On Start" (enabled by default)
4. Press Play

## What the Bootstrap Does Automatically

✅ **Finds your XR Rig**
- Locates XR Origin
- Finds left hand controller
- Detects right hand interactor (NearFarInteractor or XRRayInteractor)

✅ **Creates UI System**
- ForearmSlateCanvas (attached to left hand)
- Tab navigation panel
- 3×3 grid for block selection
- Bottom hotbar for recent blocks

✅ **Sets Up Managers**
- BlockUsageTracker (singleton)
- UndoSystem (singleton)
- EventSystem (if missing)

✅ **Creates Sample BlockCatalog** (if you don't have one)
- Sample blocks (2x4 Brick, 2x2 Brick, 1x2 Plate)
- Default color palette
- Saved at: `Assets/MRTemplateAssets/VRUISystem/Data/BlockCatalog.asset`

## Console Output

When the bootstrap runs, you'll see messages like:

```
[VRUIBootstrap] === VR UI System Bootstrap Starting ===
[VRUIBootstrap] Found XR Origin: MR Interaction Setup
[VRUIBootstrap] Found left hand: LeftHand Controller Stabilized
[VRUIBootstrap] Found NearFarInteractor: RightHand NearFarInteractor
[VRUIBootstrap] Created VR_UI_Managers with BlockUsageTracker and UndoSystem
[VRUIBootstrap] Created ForearmSlateCanvas with complete UI hierarchy
[VRUIBootstrap] === VR UI System Bootstrap Complete ===
[VRUIBootstrap] The Forearm Slate UI is now attached to your left hand!
```

## Troubleshooting

### "Could not find XR Rig components"

**Cause**: No XR Origin in your scene

**Solution**:
1. Make sure you're using the Unity MR Template
2. Verify "MR Interaction Setup" prefab is in your scene
3. If missing, drag it from: `Assets/MRTemplateAssets/Prefabs/MR Interaction Setup.prefab`

### "Failed to find or create BlockCatalog"

**Cause**: Runtime mode (build) and no catalog exists

**Solution**:
1. In Editor: Go to **Tools → VR UI System → Create Block Catalog**
2. Or manually create: **Assets → Create → VR Lego → Block Catalog Data**

### "UI appears but no blocks show up"

**Cause**: BlockCatalog has no blocks defined

**Solution**:
1. Select the BlockCatalog in Project window
2. Click "Add Block" in Inspector
3. Fill in block details (ID, name, category, prefab path)
4. Add at least one color to "Default Colors"

## Next Steps

### 1. Populate Your BlockCatalog

Select `Assets/MRTemplateAssets/VRUISystem/Data/BlockCatalog.asset` and:

- **Add more blocks**: Click "+" in the "All Blocks" list
- **Configure each block**:
  - `Block Id`: Unique identifier (e.g., "brick_2x4")
  - `Block Name`: Display name (e.g., "2x4 Brick")
  - `Category`: Choose from Bricks, Plates, Slopes, etc.
  - `Prefab Path`: Path to your Lego block prefab
  - `Thumbnail Path`: Path to icon (optional)
  - `Default Color`: Initial color for the block

- **Add colors**: Expand "Default Colors" and add Color values

### 2. Create Block Button Prefab (Optional)

For custom button styling:

1. Create: **GameObject → UI → Button**
2. Customize appearance
3. Add `BlockButton` component
4. Save as prefab
5. Assign to ForearmSlateUI → Grid Manager → Block Button Prefab

### 3. Test in VR

1. Put on your VR headset
2. Press Play
3. Look at your left forearm - you should see the UI!
4. Use your right hand to interact with the UI

## Default Controls

- **Select Block**: Point at block button and trigger/select
- **Place Block**: Point at surface, see ghost preview, trigger to place
- **Change Color**: Click color swatches on block buttons
- **Delete Mode**: (Requires configuration - see DETAILED_UNITY_SETUP.md)

## Advanced Configuration

### Manual Setup (Optional)

If you prefer manual control, see:
- **DETAILED_UNITY_SETUP.md** - Full step-by-step manual setup
- **IMPLEMENTATION_SUMMARY.md** - Technical architecture details

### Disable Auto-Setup

If you want to manually configure everything:

1. Find the `VRUISystemBootstrap` component
2. Uncheck "Auto Setup On Start"
3. You can still trigger setup via:
   - Right-click component → "Setup VR UI System"
   - Tools menu → Auto Setup VR UI

### Custom Positioning

Adjust the Forearm Slate position/rotation:

1. Select "ForearmSlateCanvas" in Hierarchy
2. Find `Forearm Slate UI` component
3. Adjust:
   - **Position Offset**: Move relative to left hand (X, Y, Z)
   - **Rotation Offset**: Rotate relative to left hand (Euler angles)
   - **Slate Scale**: Overall UI scale (default: 0.001)

**Recommended values**:
- Position: `(0.1, 0.05, 0.1)` - slightly forward and up from wrist
- Rotation: `(45, 0, 0)` - angled up for easy viewing
- Scale: `(0.001, 0.001, 0.001)` - scaled down to VR size

## Understanding the Setup

### GameObject Hierarchy Created

```
Scene Root
├── VR_UI_Managers
│   ├── BlockUsageTracker (Component)
│   └── UndoSystem (Component)
│
├── MR Interaction Setup (Your XR Rig)
│   ├── ... (existing structure)
│   └── LeftHand Controller Stabilized
│       └── ForearmSlateCanvas ← UI attaches here!
│           ├── Background
│           ├── TabsPanel
│           ├── GridPanel (3×3 grid)
│           └── RecentsPanel (hotbar)
│
└── EventSystem
```

### Components Wired

- **ForearmSlateUI**: Main controller
  - Auto-finds left hand controller
  - Auto-detects right hand interactor (NearFar or Ray)
  - Receives BlockCatalog reference

- **TabSystem**: Category switching
- **GridLayoutManager**: 3×3 block grid
- **RecentsManager**: Recently used blocks hotbar

### Auto-Detection Logic

The bootstrap intelligently finds your XR components:

1. **XR Origin**: Uses `FindFirstObjectByType<XROrigin>()`
2. **Left Hand**: Searches for Transform with "left" and "hand"/"controller" in name
3. **Right Hand Interactor**:
   - First tries `NearFarInteractor` (modern Unity 6 default)
   - Falls back to `XRRayInteractor` (legacy)
   - Searches for "right" in component name

## Comparison: Auto vs Manual Setup

| Step | Auto-Setup | Manual Setup |
|------|------------|--------------|
| **Time Required** | ~1 second | ~30-60 minutes |
| **Find XR Rig** | ✓ Automatic | Manual search |
| **Create Canvas** | ✓ Automatic | Manual creation |
| **UI Layout** | ✓ Pre-configured | Manual positioning |
| **Wire Components** | ✓ Automatic | Manual drag-and-drop |
| **Create Managers** | ✓ Automatic | Manual GameObjects |
| **BlockCatalog** | ✓ Creates sample | Manual creation |
| **Customization** | Limited | Full control |
| **Error-Prone** | No | Yes |

**Recommendation**: Use Auto-Setup to get started quickly, then customize as needed.

## FAQ

**Q: Can I run the bootstrap multiple times?**

A: Yes! The bootstrap is idempotent - it checks for existing components and updates references rather than creating duplicates.

**Q: Will this overwrite my existing setup?**

A: No. If a ForearmSlateUI already exists, it updates references but doesn't destroy your customizations.

**Q: My XR rig has custom hand controller names. Will it work?**

A: The bootstrap searches for any Transform containing "left/right" and "hand/controller". If your names are very different, you may need to manually assign the `leftHandController` field.

**Q: Can I customize the UI layout?**

A: Yes! After auto-setup, you can manually adjust the UI panels in the Hierarchy. The bootstrap just creates the initial structure.

**Q: Do I need the bootstrap component in my final build?**

A: The bootstrap can run in builds, but if you've already set up the UI, you can disable "Auto Setup On Start" or remove the component entirely.

**Q: What if I want a completely custom UI structure?**

A: Use the manual setup guide (DETAILED_UNITY_SETUP.md) instead. The bootstrap is for quick standard setups.

## Support

If you encounter issues:

1. **Check console** for `[VRUIBootstrap]` messages
2. **Verify XR rig** exists in scene (MR Interaction Setup)
3. **Check hand controllers** are named with "Left"/"Right" and "Hand"/"Controller"
4. **See detailed logs** by enabling "Show Debug Logs" on VRUISystemBootstrap component

For advanced troubleshooting, see:
- **UNITY_6_COMPATIBILITY.md** - API compatibility issues
- **NEARFAR_INTERACTOR_SUPPORT.md** - Interactor detection details
- **DETAILED_UNITY_SETUP.md** - Manual setup guide

---

**That's it!** You now have a fully functional VR UI system attached to your left forearm. Start adding blocks to your BlockCatalog and building in VR!
