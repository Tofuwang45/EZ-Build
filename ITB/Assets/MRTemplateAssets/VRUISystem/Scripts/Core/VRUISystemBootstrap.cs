using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MRTemplateAssets.Scripts
{
    /// <summary>
    /// Automatically bootstraps the VR UI system on scene start.
    /// Attach this to any GameObject in your scene, or run via Unity menu.
    /// </summary>
    public class VRUISystemBootstrap : MonoBehaviour
    {
        [Header("Auto-Setup Configuration")]
        [Tooltip("If true, will automatically setup the VR UI on scene start")]
        public bool autoSetupOnStart = true;

        [Tooltip("BlockCatalog to use (if null, will try to find one in Assets)")]
        public BlockCatalogData blockCatalog;

        [Header("Prefabs (Optional - will create from code if not assigned)")]
        public GameObject blockButtonPrefab;
        public GameObject statsPanelPrefab;

        [Header("Debug")]
        public bool showDebugLogs = true;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupVRUISystem();
            }
        }

        /// <summary>
        /// Call this from Unity Editor menu or button to setup the VR UI system
        /// </summary>
        [ContextMenu("Setup VR UI System")]
        public void SetupVRUISystem()
        {
            Log("=== VR UI System Bootstrap Starting ===");

            // Step 1: Find or create BlockCatalog
            if (blockCatalog == null)
            {
                blockCatalog = FindOrCreateBlockCatalog();
            }

            if (blockCatalog == null)
            {
                LogError("Failed to find or create BlockCatalog. Please assign one manually.");
                return;
            }

            // Step 2: Setup manager GameObjects (Singletons)
            SetupManagers();

            // Step 3: Find XR Rig and hand controllers
            Transform leftHand, rightHand;
            XRRayInteractor rightRayInteractor = null;
            NearFarInteractor rightNearFarInteractor = null;

            if (!FindXRRigComponents(out leftHand, out rightHand, out rightRayInteractor, out rightNearFarInteractor))
            {
                LogError("Could not find XR Rig components. Make sure you have an XR Origin in your scene.");
                return;
            }

            // Step 4: Create Forearm Slate UI
            GameObject forearmSlate = CreateForearmSlateUI(leftHand, rightRayInteractor, rightNearFarInteractor);

            if (forearmSlate != null)
            {
                Log("=== VR UI System Bootstrap Complete ===");
                Log("The Forearm Slate UI is now attached to your left hand!");
            }
            else
            {
                LogError("Failed to create Forearm Slate UI");
            }
        }

        private BlockCatalogData FindOrCreateBlockCatalog()
        {
            // Try to find existing BlockCatalog in Resources
            var catalogs = Resources.FindObjectsOfTypeAll<BlockCatalogData>();
            if (catalogs.Length > 0)
            {
                Log($"Found existing BlockCatalog: {catalogs[0].name}");
                return catalogs[0];
            }

#if UNITY_EDITOR
            // In editor, try to find in Assets folder
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BlockCatalogData");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                BlockCatalogData catalog = UnityEditor.AssetDatabase.LoadAssetAtPath<BlockCatalogData>(path);
                Log($"Found BlockCatalog at: {path}");
                return catalog;
            }

            // Create a new one
            Log("Creating new BlockCatalog with sample data...");
            BlockCatalogData newCatalog = ScriptableObject.CreateInstance<BlockCatalogData>();

            // Add some sample blocks
            newCatalog.allBlocks.Add(new BlockData
            {
                blockId = "brick_2x4",
                blockName = "2x4 Brick",
                category = BlockCategory.Bricks,
                prefabPath = "Prefabs/Blocks/2x4_Brick",
                thumbnailPath = "",
                defaultColor = Color.red
            });

            newCatalog.allBlocks.Add(new BlockData
            {
                blockId = "brick_2x2",
                blockName = "2x2 Brick",
                category = BlockCategory.Bricks,
                prefabPath = "Prefabs/Blocks/2x2_Brick",
                thumbnailPath = "",
                defaultColor = Color.blue
            });

            newCatalog.allBlocks.Add(new BlockData
            {
                blockId = "plate_1x2",
                blockName = "1x2 Plate",
                category = BlockCategory.Plates,
                prefabPath = "Prefabs/Blocks/1x2_Plate",
                thumbnailPath = "",
                defaultColor = Color.green
            });

            // Add default colors
            newCatalog.defaultColors.AddRange(new[]
            {
                Color.red, Color.blue, Color.green, Color.yellow,
                Color.white, Color.black, Color.gray, new Color(1f, 0.5f, 0f) // orange
            });

            // Create the asset
            string catalogPath = "Assets/MRTemplateAssets/VRUISystem/Data/BlockCatalog.asset";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(catalogPath));
            UnityEditor.AssetDatabase.CreateAsset(newCatalog, catalogPath);
            UnityEditor.AssetDatabase.SaveAssets();
            Log($"Created new BlockCatalog at: {catalogPath}");

            return newCatalog;
#else
            LogError("No BlockCatalog found and cannot create in runtime. Please create one in the Editor.");
            return null;
#endif
        }

        private void SetupManagers()
        {
            // Check if managers already exist
            if (FindFirstObjectByType<BlockUsageTracker>() != null)
            {
                Log("BlockUsageTracker already exists in scene");
            }
            else
            {
                GameObject managers = new GameObject("VR_UI_Managers");
                managers.AddComponent<BlockUsageTracker>();
                managers.AddComponent<UndoSystem>();
                Log("Created VR_UI_Managers with BlockUsageTracker and UndoSystem");
            }
        }

        private bool FindXRRigComponents(out Transform leftHand, out Transform rightHand,
            out XRRayInteractor rightRay, out NearFarInteractor rightNearFar)
        {
            leftHand = null;
            rightHand = null;
            rightRay = null;
            rightNearFar = null;

            // Find XR Origin
            var xrOrigin = FindFirstObjectByType<XROrigin>();
            if (xrOrigin == null)
            {
                LogError("No XR Origin found in scene!");
                return false;
            }

            Log($"Found XR Origin: {xrOrigin.name}");

            // Search for hand controllers
            var allTransforms = xrOrigin.GetComponentsInChildren<Transform>(true);
            foreach (var t in allTransforms)
            {
                string name = t.name.ToLower();

                // Find left hand
                if (leftHand == null && name.Contains("left") && (name.Contains("hand") || name.Contains("controller")))
                {
                    leftHand = t;
                    Log($"Found left hand: {t.name}");
                }

                // Find right hand
                if (rightHand == null && name.Contains("right") && (name.Contains("hand") || name.Contains("controller")))
                {
                    rightHand = t;
                    Log($"Found right hand: {t.name}");
                }
            }

            // Find right hand interactors
            var nearFarInteractors = FindObjectsByType<NearFarInteractor>(FindObjectsSortMode.None);
            foreach (var interactor in nearFarInteractors)
            {
                if (interactor.name.ToLower().Contains("right"))
                {
                    rightNearFar = interactor;
                    Log($"Found NearFarInteractor: {interactor.name}");
                    break;
                }
            }

            if (rightNearFar == null)
            {
                var rayInteractors = FindObjectsByType<XRRayInteractor>(FindObjectsSortMode.None);
                foreach (var interactor in rayInteractors)
                {
                    if (interactor.name.ToLower().Contains("right"))
                    {
                        rightRay = interactor;
                        Log($"Found XRRayInteractor: {interactor.name}");
                        break;
                    }
                }
            }

            bool success = leftHand != null && (rightRay != null || rightNearFar != null);

            if (!success)
            {
                LogError($"Missing components - Left: {leftHand != null}, Right Interactor: {(rightRay != null || rightNearFar != null)}");
            }

            return success;
        }

        private GameObject CreateForearmSlateUI(Transform leftHand, XRRayInteractor rightRay, NearFarInteractor rightNearFar)
        {
            // Check if already exists
            var existing = FindFirstObjectByType<ForearmSlateUI>();
            if (existing != null)
            {
                Log("ForearmSlateUI already exists, updating references...");
                existing.leftHandController = leftHand;
                existing.rightHandRayInteractor = rightRay;
                existing.rightHandNearFarInteractor = rightNearFar;
                existing.blockCatalog = blockCatalog;
                return existing.gameObject;
            }

            // Create canvas GameObject
            GameObject canvasObj = new GameObject("ForearmSlateCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(200f, 150f);
            canvasRect.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            // Add CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100f;

            // Add GraphicRaycaster for UI interaction
            canvasObj.AddComponent<GraphicRaycaster>();

            // Add TrackedDeviceGraphicRaycaster for XR interaction
            var trackedRaycaster = canvasObj.AddComponent<TrackedDeviceGraphicRaycaster>();

            // Ensure EventSystem exists
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Log("Created EventSystem");
            }

            // Create UI structure
            CreateUIHierarchy(canvasObj, out GameObject tabsPanel, out GameObject gridPanel, out GameObject recentsPanel);

            // Add ForearmSlateUI component
            ForearmSlateUI slateUI = canvasObj.AddComponent<ForearmSlateUI>();
            slateUI.leftHandController = leftHand;
            slateUI.rightHandRayInteractor = rightRay;
            slateUI.rightHandNearFarInteractor = rightNearFar;
            slateUI.blockCatalog = blockCatalog;
            slateUI.positionOffset = new Vector3(0.1f, 0.05f, 0.1f);
            slateUI.rotationOffset = new Vector3(45f, 0f, 0f);
            slateUI.slateScale = new Vector3(0.001f, 0.001f, 0.001f);

            // Setup TabSystem
            TabSystem tabSystem = tabsPanel.AddComponent<TabSystem>();
            slateUI.tabSystem = tabSystem;

            // Setup GridLayoutManager
            GridLayoutManager gridManager = gridPanel.AddComponent<GridLayoutManager>();
            gridManager.gridContainer = gridPanel.transform;
            gridManager.rows = 3;
            gridManager.columns = 3;
            slateUI.gridManager = gridManager;

            // Add GridLayoutGroup to grid panel
            GridLayoutGroup gridLayout = gridPanel.AddComponent<GridLayoutGroup>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 3;
            gridLayout.cellSize = new Vector2(60f, 60f);
            gridLayout.spacing = new Vector2(5f, 5f);

            // Setup RecentsManager
            RecentsManager recentsManager = recentsPanel.AddComponent<RecentsManager>();
            recentsManager.recentsContainer = recentsPanel.transform;
            recentsManager.maxRecents = 6;
            slateUI.recentsManager = recentsManager;

            // Add HorizontalLayoutGroup to recents panel
            HorizontalLayoutGroup recentsLayout = recentsPanel.AddComponent<HorizontalLayoutGroup>();
            recentsLayout.spacing = 5f;
            recentsLayout.childControlWidth = false;
            recentsLayout.childControlHeight = false;
            recentsLayout.childForceExpandWidth = false;
            recentsLayout.childForceExpandHeight = false;

            Log("Created ForearmSlateCanvas with complete UI hierarchy");
            Log("Note: You'll need to create block button prefabs for the UI to display blocks");

            return canvasObj;
        }

        private void CreateUIHierarchy(GameObject canvas, out GameObject tabsPanel, out GameObject gridPanel, out GameObject recentsPanel)
        {
            // Main background panel
            GameObject background = CreateUIPanel("Background", canvas.transform);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Tabs Panel (left side)
            tabsPanel = CreateUIPanel("TabsPanel", background.transform);
            RectTransform tabsRect = tabsPanel.GetComponent<RectTransform>();
            tabsRect.anchorMin = new Vector2(0f, 0.2f);
            tabsRect.anchorMax = new Vector2(0.15f, 0.95f);
            tabsRect.offsetMin = new Vector2(5f, 0f);
            tabsRect.offsetMax = new Vector2(-5f, 0f);

            VerticalLayoutGroup tabsLayout = tabsPanel.AddComponent<VerticalLayoutGroup>();
            tabsLayout.spacing = 5f;
            tabsLayout.childControlWidth = true;
            tabsLayout.childControlHeight = false;
            tabsLayout.childForceExpandWidth = true;
            tabsLayout.childForceExpandHeight = false;

            // Grid Panel (center)
            gridPanel = CreateUIPanel("GridPanel", background.transform);
            RectTransform gridRect = gridPanel.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.15f, 0.2f);
            gridRect.anchorMax = new Vector2(0.95f, 0.95f);
            gridRect.offsetMin = new Vector2(5f, 0f);
            gridRect.offsetMax = new Vector2(-5f, 0f);

            // Recents Panel (bottom)
            recentsPanel = CreateUIPanel("RecentsPanel", background.transform);
            RectTransform recentsRect = recentsPanel.GetComponent<RectTransform>();
            recentsRect.anchorMin = new Vector2(0.15f, 0.05f);
            recentsRect.anchorMax = new Vector2(0.95f, 0.18f);
            recentsRect.offsetMin = new Vector2(5f, 0f);
            recentsRect.offsetMax = new Vector2(-5f, 0f);

            Log("Created UI hierarchy: Background, Tabs, Grid, and Recents panels");
        }

        private GameObject CreateUIPanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            return panel;
        }

        private void Log(string message)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[VRUIBootstrap] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[VRUIBootstrap] {message}");
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor menu item to setup VR UI from Unity menu
    /// </summary>
    public static class VRUISystemBootstrapMenu
    {
        [UnityEditor.MenuItem("Tools/VR UI System/Auto Setup VR UI")]
        public static void SetupVRUI()
        {
            // Find or create bootstrap component
            VRUISystemBootstrap bootstrap = GameObject.FindFirstObjectByType<VRUISystemBootstrap>();

            if (bootstrap == null)
            {
                GameObject bootstrapObj = new GameObject("VRUISystemBootstrap");
                bootstrap = bootstrapObj.AddComponent<VRUISystemBootstrap>();
                Debug.Log("[VRUIBootstrap] Created VRUISystemBootstrap GameObject");
            }

            bootstrap.SetupVRUISystem();
        }

        [UnityEditor.MenuItem("Tools/VR UI System/Create Block Catalog")]
        public static void CreateBlockCatalog()
        {
            // Check if one already exists
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BlockCatalogData");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.Log($"[VRUIBootstrap] BlockCatalog already exists at: {path}");
                UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<BlockCatalogData>(path);
                return;
            }

            // Create new catalog
            BlockCatalogData catalog = ScriptableObject.CreateInstance<BlockCatalogData>();

            // Add default colors
            catalog.defaultColors.AddRange(new[]
            {
                Color.red, Color.blue, Color.green, Color.yellow,
                Color.white, Color.black, Color.gray, new Color(1f, 0.5f, 0f)
            });

            string catalogPath = "Assets/MRTemplateAssets/VRUISystem/Data/BlockCatalog.asset";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(catalogPath));
            UnityEditor.AssetDatabase.CreateAsset(catalog, catalogPath);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log($"[VRUIBootstrap] Created BlockCatalog at: {catalogPath}");
            UnityEditor.Selection.activeObject = catalog;
        }
    }
#endif
}
