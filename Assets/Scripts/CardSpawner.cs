using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame
{
    [RequireComponent(typeof(RectTransform))]
    public class CardSpawner : MonoBehaviour
    {
        private ConfigurationHandler configHandler;
        private GridRandomPicker gridPicker;

        [Tooltip("Prefab of the card (with CardButton script attached)")]
        [SerializeField] private GameObject cardPrefab;

        private RectTransform panelRect;

        [Header("Layout Settings")]
        [Tooltip("Padding between cards in pixels (x: horizontal, y: vertical)")]
        [SerializeField] private Vector2 cardPadding = Vector2.zero;
        [Tooltip("Padding of the whole matching game play area")]
        [SerializeField] private Vector2 paddingOffset = Vector2.zero;


        private List<Vector2Int> atlasImages;
        private int _availableAtlasImages;

        public int AvailableImages => _availableAtlasImages;

        private void Awake()
        {
            panelRect = GetComponent<RectTransform>();
            configHandler = GameManager.Instance.ConfigHandler;
        }

        private void OnEnable()
        {
            InitAvailableImages();
            var gridSize = configHandler.CurrentDifficultySize();
            gridPicker = new GridRandomPicker(gridSize.x, gridSize.y);
            SpawnCards();
        }

        /// <summary>
        /// Initializes the available atlas images for use
        /// </summary>
        public void InitAvailableImages()
        {
            int cols = configHandler.AtlasColumns;
            int rows = configHandler.AtlasRows;
            atlasImages = new List<Vector2Int>(cols * rows);
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    atlasImages.Add(new Vector2Int(x, y));
            _availableAtlasImages = atlasImages.Count;
        }

        /// <summary>
        /// Returns a random unused atlas cell (x,y), or null if none remain.
        /// </summary>
        public Vector2Int? GetNextUnusedImage()
        {
            if (_availableAtlasImages == 0) return null;

            int idx = Random.Range(0, _availableAtlasImages);
            Vector2Int cell = atlasImages[idx];

            // Swap with last unused
            atlasImages[idx] = atlasImages[_availableAtlasImages - 1];
            atlasImages[_availableAtlasImages - 1] = cell;
            _availableAtlasImages--;

            return cell;
        }

        /// <summary>
        /// Spawns card instances, placing each image twice at random grid positions.
        /// </summary>
        private void SpawnCards()
        {
            var gridSize = configHandler.CurrentDifficultySize();
            int uniqueImagesCount = gridSize.x * gridSize.y / 2;

            // Calculate overall padding for the matching area
            var rtSample = cardPrefab.GetComponent<RectTransform>();
            float cellW = rtSample.rect.width + cardPadding.x;
            float cellH = rtSample.rect.height + cardPadding.y;
            float Hpadding = ((panelRect.rect.width - (cellW * gridSize.x)) * 0.5f) + paddingOffset.x;
            float Vpadding = ((panelRect.rect.height - (cellH * gridSize.y)) * 0.5f) + paddingOffset.y;

            // For each unique image, spawn two cards
            for (int i = 0; i < uniqueImagesCount; i++)
            {
                // Atlas coords
                if (!(GetNextUnusedImage() is Vector2Int atlasCoord))
                {
                    Debug.LogError("No more unused atlas cells! Check your atlas size and difficulty config.");
                    return;
                }

                // Spawn two cards for this atlas image
                for (int copy = 0; copy < 2; copy++)
                {
                    // Grid position
                    if (!(gridPicker.GetNext() is Vector2Int gridPos))
                    {
                        Debug.LogError("No more free grid slots! Check your grid size.");
                        return;
                    }

                    // Instantiate and slice
                    var cardGo = Instantiate(cardPrefab, panelRect);
                    cardGo.GetComponent<Card>().ApplySlice(atlasCoord);

                    // Position
                    var cardRt = cardGo.GetComponent<RectTransform>();
                    float posX = Hpadding + (cellW * gridPos.x) + cardPadding.x * 0.5f;
                    float posY = Vpadding + (cellH * (gridSize.y - 1 - gridPos.y)) + cardPadding.y * 0.5f;
                    // anchoredPosition uses bottom-left origin, so invert Y
                    posY = -posY;
                    cardRt.anchoredPosition = new Vector2(posX, posY);
                }
            }
        }
    }

    /// <summary>
    /// Helper: unique random picker over a cols×rows grid.
    /// </summary>
    public class GridRandomPicker
    {
        private readonly List<Vector2Int> _cells;
        private int _remaining;

        public GridRandomPicker(int columns, int rows)
        {
            _cells = new List<Vector2Int>(columns * rows);
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < columns; x++)
                    _cells.Add(new Vector2Int(x, y));
            _remaining = _cells.Count;
        }

        /// <summary>
        /// Returns a random unused cell (x,y), or null if exhausted.
        /// </summary>
        public Vector2Int? GetNext()
        {
            if (_remaining == 0) return null;
            int idx = Random.Range(0, _remaining);
            Vector2Int chosen = _cells[idx];
            // swap with last unused
            _cells[idx] = _cells[_remaining - 1];
            _cells[_remaining - 1] = chosen;
            _remaining--;
            return chosen;
        }

        public int Remaining => _remaining;
    }
}
