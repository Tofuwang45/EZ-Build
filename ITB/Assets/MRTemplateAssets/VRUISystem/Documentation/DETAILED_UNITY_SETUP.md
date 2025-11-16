# VR Forearm Slate UI - Detailed Unity Editor Setup Guide

**For Unity 6000.2.12f1 with XR Interaction Toolkit 3.2.1**

This guide provides step-by-step instructions with exact values for building the VR UI in Unity Editor.

## Table of Contents
1. [Understanding Canvas Scaling](#understanding-canvas-scaling)
2. [Creating the Block Catalog](#step-1-create-block-catalog)
3. [Building the Forearm Slate Canvas](#step-2-build-forearm-slate-canvas)
4. [Creating the Tab Bar](#step-3-create-tab-bar)
5. [Building the Grid Container](#step-4-build-grid-container)
6. [Creating Block Button Prefab](#step-5-create-block-button-prefab)
7. [Creating Recents Bar](#step-6-create-recents-bar)
8. [Building Stats Panel](#step-7-build-stats-panel)
9. [Adding Delete Mode Toggle](#step-8-add-delete-mode)
10. [Wiring Everything Together](#step-9-wire-everything-together)

---

## Understanding Canvas Scaling

### Important Concept: World Space UI

The Forearm Slate uses **World Space** rendering, which means:
- The canvas exists in 3D world space (not screen overlay)
- Scale of **0.001** converts Unity units to realistic sizes
- A RectTransform width of **200** = 20cm in VR (200 × 0.001 = 0.2m)
- Think of the canvas as a virtual tablet in your hand

### Scaling Formula
```
Real VR Size = RectTransform Size × Canvas Scale
Example: 200 width × 0.001 scale = 0.2m (20cm) actual size
```

**Key Rule**: Design in large numbers (200-600 range), then scale down with Canvas scale.

---

## Step 1: Create Block Catalog

### 1.1 Create the ScriptableObject

1. In Project window, navigate to: `Assets/MRTemplateAssets/VRUISystem/Data/`
2. Right-click in the folder
3. Select: **Create → VR Lego → Block Catalog Data**
4. Name it: `BlockCatalog`

### 1.2 Configure Block Catalog

Select `BlockCatalog` in Project window. In Inspector:

**Default Colors Setup:**
1. Expand **Default Colors** list
2. Set Size: `6`
3. Configure colors:
   - Element 0: Red (R:1, G:0, B:0, A:1)
   - Element 1: Blue (R:0, G:0.3, B:1, A:1)
   - Element 2: Yellow (R:1, G:0.92, B:0.016, A:1)
   - Element 3: Green (R:0, G:1, B:0, A:1)
   - Element 4: White (R:1, G:1, B:1, A:1)
   - Element 5: Dark Gray (R:0.2, G:0.2, B:0.2, A:1)

**Add Your First Block:**
1. Expand **All Blocks** list
2. Click **+** to add Element 0
3. Configure:
   ```
   Block Name: "2x4 Brick"
   Block ID: "brick_2x4"
   Prefab: [Drag your Lego brick prefab here]
   Icon: [Assign a sprite - create one if needed]
   Category: Bricks
   Default Scale: X:1, Y:1, Z:1
   Available Colors: Size: 6 (copy from Default Colors above)
   ```

**Note**: You'll need to create icons for your blocks. Quick method:
- Take a screenshot of each block in Unity
- Import as sprite
- Use as icon

---

## Step 2: Build Forearm Slate Canvas

### 2.1 Create Main Canvas GameObject

1. In Hierarchy, create: **GameObject → UI → Canvas**
2. Rename to: `ForearmSlateCanvas`
3. **DO NOT** attach to hand yet - we'll do that at the end

### 2.2 Configure Canvas Component

Select `ForearmSlateCanvas`, in Inspector:

**Canvas Component:**
```yaml
Render Mode: World Space
Event Camera: [Leave None for now - will auto-assign]
Sorting Layer: Default
Order in Layer: 0
```

**Rect Transform:**
```yaml
Pos X: 0, Y: 0, Z: 0
Width: 200
Height: 150
Scale: X: 0.001, Y: 0.001, Z: 0.001  # CRITICAL!
```

**Visual Result**: The canvas will appear as a tiny rectangle in Scene view (20cm × 15cm in VR).

### 2.3 Add Canvas Scaler

1. With `ForearmSlateCanvas` selected, click **Add Component**
2. Search: `Canvas Scaler`
3. Configure:
   ```yaml
   UI Scale Mode: Constant Physical Size
   Physical Unit: Centimeters
   Fallback Screen DPI: 96
   Default Sprite DPI: 96
   Reference Pixels Per Unit: 100
   ```

### 2.4 Add Graphic Raycaster

1. Click **Add Component**
2. Search: `Tracked Device Graphic Raycaster` (from XR Interaction Toolkit)
3. Configure:
   ```yaml
   Ignore Reversed Graphics: True
   Blocking Objects: None
   Blocking Mask: Everything
   ```

**Why this component?** Allows VR ray interactors to interact with the UI.

### 2.5 Add ForearmSlateUI Script

1. Click **Add Component**
2. Search: `Forearm Slate UI`
3. **Leave all fields empty for now** - we'll wire them later

---

## Step 3: Create Tab Bar

### 3.1 Create Tab Bar Panel

1. Right-click `ForearmSlateCanvas` → **UI → Panel**
2. Rename to: `TabBar`

**Configure TabBar RectTransform:**
```yaml
Anchor Preset: Top Stretch (hold Alt+Shift, click top-middle preset)
Pos X: 0, Y: -15
Pos Z: 0
Left: 10, Top: 10, Right: 10
Height: 30
```

**Visual Result**: A bar at the top of the canvas, 30 units tall.

### 3.2 Configure TabBar Visual

**Image Component** (on TabBar):
```yaml
Color: R:0.15, G:0.15, B:0.15, A:0.9 (dark semi-transparent)
Material: None
Raycast Target: ✓ (checked)
```

### 3.3 Add Horizontal Layout Group

1. Select `TabBar`
2. Add Component: `Horizontal Layout Group`
3. Configure:
   ```yaml
   Padding: Left:5, Right:5, Top:5, Bottom:5
   Spacing: 5
   Child Alignment: Middle Center
   Child Controls Size:
     - Width: ✓ (checked)
     - Height: ✓ (checked)
   Child Force Expand:
     - Width: ✓ (checked)
     - Height: ✗ (unchecked)
   ```

**What this does**: Automatically arranges tab buttons horizontally with spacing.

### 3.4 Create Tab Buttons

**Create First Tab (Bricks):**

1. Right-click `TabBar` → **UI → Button - TextMeshPro**
2. Rename to: `BricksTab`

**Configure BricksTab Button:**
```yaml
Image Component:
  Color: R:0.3, G:0.3, B:0.3, A:1 (gray)

Button Component:
  Interactable: ✓
  Transition: Color Tint
  Normal Color: R:0.3, G:0.3, B:0.3, A:1
  Highlighted Color: R:0.4, G:0.4, B:0.4, A:1
  Pressed Color: R:0.2, G:0.2, B:0.2, A:1
  Selected Color: R:0.2, G:0.6, B:1, A:1 (blue - for active tab)
  Disabled Color: R:0.2, G:0.2, B:0.2, A:0.5
```

**Configure BricksTab Text** (child object):
```yaml
Text: "Bricks"
Font Size: 14
Color: White
Alignment: Center + Middle
Auto Size: ✗
Wrapping: ✗
```

**Create Additional Tabs:**

Duplicate `BricksTab` 2 more times (Ctrl+D):
- `PlatesTab` - Change text to "Plates"
- `SlopesTab` - Change text to "Slopes"

**Layout Group will auto-arrange** them horizontally!

### 3.5 Add TabSystem Script

1. Select `TabBar`
2. Add Component: `Tab System`
3. Configure Tabs array:
   ```yaml
   Tabs: Size: 3

   Element 0:
     Category: Bricks
     Button: [Drag BricksTab here]
     Label: [Drag BricksTab/Text (TMP) here]
     Background: [Drag BricksTab/Image here]

   Element 1:
     Category: Plates
     Button: [Drag PlatesTab here]
     Label: [Drag PlatesTab/Text (TMP) here]
     Background: [Drag PlatesTab/Image here]

   Element 2:
     Category: Slopes
     Button: [Drag SlopesTab here]
     Label: [Drag SlopesTab/Text (TMP) here]
     Background: [Drag SlopesTab/Image here]
   ```

**Visual Settings:**
```yaml
Active Tab Color: R:0.2, G:0.6, B:1, A:1 (blue)
Inactive Tab Color: R:0.3, G:0.3, B:0.3, A:1 (gray)
```

---

## Step 4: Build Grid Container

### 4.1 Create Grid Panel

1. Right-click `ForearmSlateCanvas` → **UI → Panel**
2. Rename to: `GridContainer`

**Configure GridContainer RectTransform:**
```yaml
Anchor Preset: Stretch Both (Alt+Shift + bottom-right preset)
Left: 10, Right: 10, Top: 50, Bottom: 10
Pos Z: 0
```

**Visual Result**: Fills most of canvas below tab bar.

### 4.2 Configure Grid Visual

**Image Component:**
```yaml
Color: R:0.1, G:0.1, B:0.1, A:0.8 (darker, semi-transparent)
Raycast Target: ✓
```

### 4.3 Add Grid Layout Group

1. Select `GridContainer`
2. Add Component: `Grid Layout Group`
3. Configure:
   ```yaml
   Padding: All: 10
   Cell Size: X:50, Y:50  # This is the button size
   Spacing: X:5, Y:5
   Start Corner: Upper Left
   Start Axis: Horizontal
   Child Alignment: Upper Left
   Constraint: Fixed Column Count
   Constraint Count: 3  # 3 columns = 3x3 grid
   ```

**Important**: Cell Size 50×50 means each button is 5cm × 5cm in VR (50 × 0.001 scale).

### 4.4 Add GridLayoutManager Script

1. Select `GridContainer`
2. Add Component: `Grid Layout Manager`
3. Configure:
   ```yaml
   Grid Container: [Drag GridContainer (self) here]
   Block Button Prefab: [We'll create this next]
   Rows: 3
   Columns: 3
   ```

---

## Step 5: Create Block Button Prefab

This is the most detailed part! We'll create a button template.

### 5.1 Create Button Structure (Temporary)

**Create in a temporary location** (not inside GridContainer yet):

1. Right-click Hierarchy (not under ForearmSlateCanvas) → **UI → Button - TextMeshPro**
2. Rename to: `BlockButtonPrefab`

**Result**: Creates a standalone button in Scene root.

### 5.2 Configure Button Base

Select `BlockButtonPrefab`:

**Rect Transform:**
```yaml
Width: 50
Height: 50
```

**Image Component:**
```yaml
Color: R:0.2, G:0.2, B:0.2, A:1
Material: None
```

**Button Component:**
```yaml
Interactable: ✓
Transition: Color Tint
Normal Color: R:0.2, G:0.2, B:0.2, A:1
Highlighted Color: R:0.3, G:0.5, B:0.8, A:1 (blue highlight)
Pressed Color: R:0.15, G:0.4, B:0.7, A:1
```

### 5.3 Create Icon Image

1. Right-click `BlockButtonPrefab` → **UI → Image**
2. Rename to: `Icon`

**Configure Icon:**
```yaml
Rect Transform:
  Anchor: Center
  Pos: X:0, Y:5, Z:0
  Width: 40
  Height: 40

Image Component:
  Source Image: [Leave None - will be set per block]
  Color: White
  Preserve Aspect: ✓
  Raycast Target: ✗ (unchecked - parent handles clicks)
```

### 5.4 Create Name Label

1. Right-click `BlockButtonPrefab` → **UI → Text - TextMeshPro**
2. Rename to: `NameLabel`

**Configure NameLabel:**
```yaml
Rect Transform:
  Anchor: Bottom Stretch
  Pos Y: 5
  Left: 2, Right: 2, Bottom: 2
  Height: 12

TextMeshPro Component:
  Text: "Block Name"
  Font Size: 8
  Color: White
  Alignment: Center + Bottom
  Overflow: Truncate
  Wrapping: ✗
  Auto Size: ✗
  Raycast Target: ✗
```

### 5.5 Create Color Dots Container

1. Right-click `BlockButtonPrefab` → **Create Empty**
2. Rename to: `ColorDotsContainer`

**Configure ColorDotsContainer:**
```yaml
Rect Transform:
  Anchor: Top Stretch
  Pos Y: -5
  Left: 2, Right: 2, Top: 2
  Height: 8
```

3. Add Component: `Horizontal Layout Group`
   ```yaml
   Padding: 0
   Spacing: 2
   Child Alignment: Middle Center
   Child Controls Size:
     Width: ✓
     Height: ✓
   Child Force Expand:
     Width: ✓
     Height: ✗
   ```

### 5.6 Create Color Dot Prefab

**Create a single color dot** (child of ColorDotsContainer):

1. Right-click `ColorDotsContainer` → **UI → Button - TextMeshPro**
2. Rename to: `ColorDotPrefab`

**Configure ColorDotPrefab:**
```yaml
Rect Transform:
  Width: 8
  Height: 8

Image Component:
  Color: R:1, G:0, B:0 (Red - will change per color)

Button Component:
  Interactable: ✓
  Transition: Color Tint
  Normal Color: Full brightness
  Highlighted Color: Slightly brighter
  Pressed Color: Slightly darker
```

**Delete the Text child** - we don't need text on color dots.

**Now move ColorDotPrefab to Project window** to save as prefab:
1. Create folder: `Assets/MRTemplateAssets/VRUISystem/Prefabs/UI/`
2. Drag `ColorDotPrefab` from Hierarchy to this folder
3. Delete `ColorDotPrefab` from Hierarchy (we only need it as prefab)

### 5.7 Add BlockButton Script

1. Select `BlockButtonPrefab` (in Hierarchy)
2. Add Component: `Block Button`
3. Configure:
   ```yaml
   Icon Image: [Drag Icon image here]
   Name Label: [Drag NameLabel here]
   Button: [Drag BlockButtonPrefab (self) Button component]
   Color Dots Container: [Drag ColorDotsContainer]
   Color Dot Prefab: [Drag ColorDotPrefab from Project]
   Ghost Material: [We'll create this separately - see below]
   Ghost Distance: 0.3
   ```

### 5.8 Create Ghost Material (Optional)

**To create a semi-transparent ghost material:**

1. Create: **Assets → Create → Material**
2. Name: `GhostMaterial`
3. Configure:
   ```yaml
   Shader: Universal Render Pipeline/Lit (or Standard)
   Rendering Mode: Transparent
   Base Color: R:1, G:1, B:1, A:0.5 (50% transparent)
   ```
4. Save to: `Assets/MRTemplateAssets/VRUISystem/Materials/`
5. Drag to BlockButton's **Ghost Material** field

### 5.9 Save BlockButton as Prefab

1. Create folder if needed: `Assets/MRTemplateAssets/VRUISystem/Prefabs/UI/`
2. Drag `BlockButtonPrefab` from Hierarchy → Prefabs folder
3. **Delete `BlockButtonPrefab` from Hierarchy** (we only need the prefab)

**Now assign this prefab** to GridLayoutManager's `Block Button Prefab` field!

---

## Step 6: Create Recents Bar

### 6.1 Create Recents Panel

1. Right-click `ForearmSlateCanvas` → **UI → Panel**
2. Rename to: `RecentsBar`

**Configure RecentsBar RectTransform:**
```yaml
Anchor Preset: Bottom Stretch
Left: 10, Right: 10, Bottom: 45
Height: 35
Pos Z: 0
```

**Image Component:**
```yaml
Color: R:0.18, G:0.18, B:0.18, A:0.85
```

### 6.2 Add Horizontal Layout

1. Select `RecentsBar`
2. Add Component: `Horizontal Layout Group`
3. Configure:
   ```yaml
   Padding: 5 (all sides)
   Spacing: 3
   Child Alignment: Middle Left
   Child Controls Size: Width ✓, Height ✓
   Child Force Expand: Width ✗, Height ✗
   ```

### 6.3 Add RecentsManager Script

1. Select `RecentsBar`
2. Add Component: `Recents Manager`
3. Configure:
   ```yaml
   Recents Container: [Drag RecentsBar (self)]
   Recent Block Button Prefab: [Drag BlockButtonPrefab from Project]
   Max Recents: 6
   Ray Interactor: [Leave empty - will wire later]
   ```

---

## Step 7: Build Stats Panel

This panel floats separately, not attached to the main canvas.

### 7.1 Create Stats Panel Canvas

1. In Hierarchy root → **GameObject → UI → Canvas**
2. Rename to: `StatsPanelCanvas`

**Configure Canvas:**
```yaml
Render Mode: World Space
Width: 120
Height: 80
Scale: 0.001, 0.001, 0.001
```

### 7.2 Add Components to StatsPanelCanvas

1. Add: `Canvas Scaler` (same settings as before)
2. Add: `Tracked Device Graphic Raycaster`
3. Add: `Stats Panel` script

### 7.3 Create Background Panel

1. Right-click `StatsPanelCanvas` → **UI → Panel**
2. Rename to: `Background`

**Configure:**
```yaml
Anchor: Stretch Both
Margins: 0 (all)
Color: R:0.1, G:0.1, B:0.1, A:0.9 (dark, mostly opaque)
```

### 7.4 Create Total Count Text

1. Right-click `StatsPanelCanvas` → **UI → Text - TextMeshPro**
2. Rename to: `TotalCountText`

**Configure:**
```yaml
Rect Transform:
  Anchor: Top Stretch
  Left: 5, Right: 5, Top: 5
  Height: 15

TextMeshPro:
  Text: "Total: 0 blocks"
  Font Size: 10
  Color: White
  Alignment: Center + Middle
  Font Style: Bold
```

### 7.5 Create Top Blocks Container

1. Right-click `StatsPanelCanvas` → **UI → Panel**
2. Rename to: `TopBlocksContainer`

**Configure:**
```yaml
Anchor: Stretch Both
Left: 5, Right: 5, Top: 25, Bottom: 20
Color: Transparent (A:0)
```

3. Add Component: `Vertical Layout Group`
   ```yaml
   Padding: 0
   Spacing: 2
   Child Alignment: Upper Left
   Child Controls Size: Width ✓, Height ✗
   Child Force Expand: Width ✓, Height ✗
   ```

### 7.6 Create Full List Container (Scroll View)

1. Right-click `StatsPanelCanvas` → **UI → Scroll View**
2. Rename to: `FullListContainer`
3. Set initially inactive (uncheck at top of Inspector)

**Configure:**
```yaml
Rect Transform:
  Anchor: Stretch Both
  Left: 5, Right: 5, Top: 25, Bottom: 20

Scroll Rect:
  Horizontal: ✗
  Vertical: ✓
  Movement Type: Clamped
```

**Find the Content child** and add:
- Component: `Vertical Layout Group` (same settings as TopBlocksContainer)

### 7.7 Create Stat Line Prefab

Create a template for each stat line:

1. Right-click in Project → **Create → Folder**: `Prefabs/UI/StatsPanelPrefabs/`
2. In Hierarchy (temporary): **GameObject → UI → Text - TextMeshPro**
3. Rename to: `StatLinePrefab`

**Configure:**
```yaml
Width: 110
Height: 12

Text: "5x Red 2x4 Brick"
Font Size: 8
Color: White
Alignment: Left + Middle
```

4. Drag to Prefabs folder, then delete from Hierarchy

### 7.8 Create Expand Button

1. Right-click `StatsPanelCanvas` → **UI → Button - TextMeshPro**
2. Rename to: `ExpandButton`

**Configure:**
```yaml
Anchor: Bottom Stretch
Left: 5, Right: 5, Bottom: 5
Height: 12

Text: "⏷ Expand"
Font Size: 8
```

### 7.9 Wire Stats Panel Script

Select `StatsPanelCanvas`, find `Stats Panel` component:

```yaml
Total Count Text: [Drag TotalCountText]
Top Blocks Container: [Drag TopBlocksContainer]
Full List Container: [Drag FullListContainer/Viewport/Content]
Stat Line Prefab: [Drag StatLinePrefab from Project]
Expand Button: [Drag ExpandButton]
Expand Icon: [Optional - create expand/collapse icons]
Panel Position: X:0.3, Y:0.1, Z:0.5
Use Lazy Follow: ✓
```

### 7.10 Save as Prefab

1. Drag `StatsPanelCanvas` to Project: `Prefabs/`
2. Name: `StatsPanelPrefab`
3. Delete `StatsPanelCanvas` from scene (we'll instantiate via script)

---

## Step 8: Add Delete Mode

### 8.1 Create Delete Toggle

1. Right-click `TabBar` → **UI → Toggle**
2. Rename to: `DeleteModeToggle`

**Configure:**
```yaml
Width: 60
Height: 25

Background:
  Color: R:0.3, G:0.3, B:0.3

Checkmark:
  Color: R:1, G:0.3, B:0.3 (Red when active)

Label Text: "Delete"
  Font Size: 8
```

**The Horizontal Layout Group** on TabBar will add this to the end of tabs.

### 8.2 Add DeleteMode Script

1. Select `ForearmSlateCanvas` (main canvas)
2. Add Component: `Delete Mode`
3. Configure:
   ```yaml
   Delete Mode Toggle: [Drag DeleteModeToggle]
   Ray Interactor: [Leave empty - wire later]
   Delete Ray Color: R:1, G:0, B:0 (Red)
   Normal Ray Color: R:0.2, G:0.6, B:1 (Blue)
   Ray Line Renderer: [Leave empty unless you have visible rays]
   Deletable Layer Mask: Everything
   Max Delete Distance: 10
   ```

### 8.3 Add Undo Button (Optional)

Create similar to Delete toggle:
1. Button next to Delete toggle
2. Label: "Undo"
3. OnClick → Connect to UndoSystem script later

---

## Step 9: Wire Everything Together

Now we connect all the pieces!

### 9.1 Create Manager GameObjects

1. In Hierarchy root → **Create Empty**
2. Rename to: `VR_UI_Managers`

**Add to VR_UI_Managers:**
1. Add Component: `Block Usage Tracker`
2. Add Component: `Undo System`
   - Max Undo History: 20

### 9.2 Attach Canvas to Left Hand

**Find your Left Hand Controller:**
1. Expand: `XR Origin → Camera Offset`
2. Find: `LeftHand Controller` (or similar - check your XR rig)
3. Drag `ForearmSlateCanvas` onto it (make it a child)

**Set Transform:**
```yaml
Position: X:0.1, Y:0.05, Z:0.1
Rotation: X:45, Y:0, Z:0
Scale: 0.001, 0.001, 0.001 (should already be set)
```

**Adjust position** in Play mode to find comfortable spot on forearm!

### 9.3 Wire ForearmSlateUI Script

Select `ForearmSlateCanvas`, find `Forearm Slate UI` component:

```yaml
Hand Attachment:
  Left Hand Controller: [Drag LeftHand Controller transform]

Positioning:
  Position Offset: 0.1, 0.05, 0.1
  Rotation Offset: 45, 0, 0
  Slate Scale: 0.001, 0.001, 0.001

References:
  Block Catalog: [Drag BlockCatalog from Project]
  Tab System: [Drag TabBar]
  Grid Manager: [Drag GridContainer]
  Recents Manager: [Drag RecentsBar]
  Stats Panel Prefab: [Drag StatsPanelPrefab from Project]

Interaction:
  Right Hand Ray Interactor: [Drag RightHand Controller's XRRayInteractor]
```

### 9.4 Wire Tab System

Select `TabBar`, ensure TabSystem has:
- ✓ Block Catalog assigned
- ✓ All 3 tabs configured (Element 0-2)

### 9.5 Wire Grid Manager

Select `GridContainer`, ensure GridLayoutManager has:
- ✓ Grid Container (self)
- ✓ Block Button Prefab
- ✓ Rows: 3, Columns: 3

### 9.6 Wire Recents Manager

Select `RecentsBar`, ensure RecentsManager has:
- ✓ Recents Container (self)
- ✓ Recent Block Button Prefab
- ✓ Ray Interactor: [Drag RightHand XRRayInteractor]

### 9.7 Wire Delete Mode

Select `ForearmSlateCanvas`, find DeleteMode:
- ✓ Delete Mode Toggle assigned
- ✓ Ray Interactor: [Drag RightHand XRRayInteractor]

---

## Step 10: Testing

### 10.1 Test in Editor (Play Mode)

1. Add at least 1 block to BlockCatalog with:
   - Valid prefab
   - Icon
   - Category set

2. Press Play

3. Check Console for errors

4. **If UI doesn't show:**
   - Check ForearmSlateCanvas is child of LeftHand Controller
   - Check Canvas scale is 0.001
   - Check all script references are assigned

### 10.2 Test in VR

1. Build and deploy to headset
2. Check positioning on left forearm
3. Adjust `Position Offset` and `Rotation Offset` for comfort
4. Test ray interaction with right hand

---

## Common Issues & Solutions

### UI Too Small/Big
- Check Canvas scale (should be 0.001)
- Adjust RectTransform sizes, not scale

### Can't Click Buttons
- Check: Tracked Device Graphic Raycaster on canvas
- Check: Right Hand Ray Interactor assigned
- Check: Event Camera on Canvas (may need manual assignment)

### Tabs Don't Switch
- Check: TabSystem has Block Catalog assigned
- Check: OnTabChanged event is wired to GridManager

### Blocks Don't Spawn
- Check: Block Catalog has blocks configured
- Check: Block prefabs are assigned
- Check: GridLayoutManager has catalog reference

### Ghost Preview Doesn't Show
- Check: Ghost Material assigned
- Check: Block prefabs have Renderers
- Check: XRRayInteractor is working

---

## Summary Checklist

- [ ] BlockCatalog created with at least 1 block
- [ ] ForearmSlateCanvas created with scale 0.001
- [ ] TabBar with 3 tabs and TabSystem script
- [ ] GridContainer with GridLayoutManager script
- [ ] BlockButtonPrefab created and saved
- [ ] RecentsBar with RecentsManager script
- [ ] StatsPanelPrefab created
- [ ] DeleteModeToggle created
- [ ] VR_UI_Managers with BlockUsageTracker and UndoSystem
- [ ] ForearmSlateCanvas attached to LeftHand Controller
- [ ] All script references wired in Inspector
- [ ] Tested in Play mode
- [ ] Tested in VR headset

---

**Need Help?**
- Check console for errors
- Verify all references are assigned (no "None" in Inspector)
- Compare your hierarchy to the structure in README.md

