using UnityEngine;
using UnityEngine.UI;

namespace MRTemplateAssets.Scripts
{
    /// <summary>
    /// Main controller for the Forearm Slate UI - simple static 3x3 grid
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class ForearmSlateUI : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [Tooltip("The parent transform containing the grid")]
        public Transform gridContainer;

        [Tooltip("Prefab for block buttons")]
        public GameObject blockButtonPrefab;

        [Header("Block Catalog")]
        [Tooltip("Catalog containing the first 9 blocks to display")]
        public BlockCatalogData blockCatalog;

        [Header("Grid Settings")]
        [Tooltip("Number of columns in the grid")]
        public int columns = 3;

        private Canvas canvas;
        private bool isInitialized = false;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            SetupCanvas();
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) return;

            // Setup grid with first 9 blocks from catalog
            if (blockCatalog != null && gridContainer != null && blockButtonPrefab != null)
            {
                SetupStaticGrid();
            }
            else
            {
                Debug.LogError("ForearmSlateUI: Missing references (blockCatalog, gridContainer, or blockButtonPrefab)");
            }

            isInitialized = true;
        }

        private void SetupCanvas()
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

        private void SetupStaticGrid()
        {
            // Setup grid layout
            var gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = columns;
            }

            // Create buttons for the first 9 blocks
            int blockCount = Mathf.Min(blockCatalog.allBlocks.Count, 9);
            for (int i = 0; i < blockCount; i++)
            {
                CreateBlockButton(blockCatalog.allBlocks[i]);
            }
        }

        private void CreateBlockButton(BlockData blockData)
        {
            if (blockData == null) return;

            GameObject buttonObj = Instantiate(blockButtonPrefab, gridContainer);
            BlockButton blockButton = buttonObj.GetComponent<BlockButton>();

            if (blockButton != null)
            {
                blockButton.Initialize(blockData);
            }
        }

        private void OnDestroy()
        {
            // Cleanup if needed
        }

        /// <summary>
        /// Show or hide the forearm slate
        /// </summary>
        public void SetSlateActive(bool active)
        {
            canvas.enabled = active;
        }

        /// <summary>
        /// Toggle the slate visibility
        /// </summary>
        public void ToggleSlate()
        {
            SetSlateActive(!canvas.enabled);
        }
    }
}
