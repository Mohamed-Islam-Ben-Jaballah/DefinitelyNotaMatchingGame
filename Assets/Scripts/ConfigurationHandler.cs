using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchingGame
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    [CreateAssetMenu(
        fileName = "ConfigurationHandler",
        menuName = "MatchingGame/Configuration Handler",
        order = 0)]
    public class ConfigurationHandler : ScriptableObject
    {
        [Header("Default Starting Difficulty")]
        [Tooltip("Used on first run if no saved value exists")]
        [SerializeField] private Difficulty defaultDifficulty = Difficulty.Easy;

        [Header("Grid Size per Difficulty (columns × rows)")]
        [Tooltip("Define the grid width (columns) and height (rows) separately")]
        [SerializeField] private Vector2Int easySize = new Vector2Int(3, 3);
        [SerializeField] private Vector2Int mediumSize = new Vector2Int(4, 3);
        [SerializeField] private Vector2Int hardSize = new Vector2Int(5, 4);
        [SerializeField] private Vector2Int impossibleSize = new Vector2Int(6, 5);

        [Header("Time per Difficulty (in seconds)")]
        [SerializeField] private float easyTime = 30;
        [SerializeField] private float mediumTime = 80;
        [SerializeField] private float hardTime = 150;
        [SerializeField] private float impossibleTime = 280;

        [Header("Tiles Atlas")]
        [Tooltip("The atlas containing all tile images")]
        [SerializeField] private Sprite atlas;

        [Min(1)]
        [Tooltip("Number of columns in the atlas grid")]
        [SerializeField] private int atlasColumns = 5;
        [Min(1)]
        [Tooltip("Number of rows in the atlas grid")]
        [SerializeField] private int atlasRows = 5;

        [Header("Atlas Offsets (in cell units)")]
        [Tooltip("Horizontal offset between tiles in the atlas (in cells)")]
        [SerializeField] private float horizontalOffset = 0f;
        [Tooltip("Vertical offset between tiles in the atlas (in cells)")]
        [SerializeField] private float verticalOffset = 0f;

        private Difficulty _difficulty;

        /// <summary>
        /// Current difficulty (loads from PlayerPrefs on enable, or uses defaultDifficulty).
        /// </summary>
        public Difficulty difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                PlayerPrefs.SetString("difficulty", value.ToString());
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Returns the grid size (columns, rows) for the given difficulty.
        /// </summary>
        public Vector2Int SizeFor(Difficulty d)
        {
            switch (d)
            {
                case Difficulty.Easy: return easySize;
                case Difficulty.Medium: return mediumSize;
                case Difficulty.Hard: return hardSize;
                case Difficulty.Impossible: return impossibleSize;
                default: return easySize;
            }
        }
        public Vector2Int CurrentDifficultySize() => SizeFor(difficulty);

        public Difficulty AdvanceDifficulty()
        {
            return difficulty = (Difficulty)(((int)difficulty + 1) % System.Enum.GetValues(typeof(Difficulty)).Length);
        }

        /// <summary>
        /// Returns the Time Limit (seconds) for the given difficulty.
        /// </summary>
        public float TimeFor(Difficulty d)
        {
            switch (d)
            {
                case Difficulty.Easy: return easyTime;
                case Difficulty.Medium: return mediumTime;
                case Difficulty.Hard: return hardTime;
                case Difficulty.Impossible: return impossibleTime;
                default: return impossibleTime;
            }
        }
        public float CurrentTimeLimit() => TimeFor(difficulty);


        public Sprite Atlas => atlas;
        public int AtlasColumns => atlasColumns;
        public int AtlasRows => atlasRows;
        public float HorizontalOffset => horizontalOffset;
        public float VerticalOffset => verticalOffset;

        private void OnEnable()
        {
            _difficulty = defaultDifficulty;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Clamp grid sizes per difficulty
            easySize.x = Mathf.Max(2, easySize.x);
            easySize.y = Mathf.Max(2, easySize.y);
            mediumSize.x = Mathf.Max(2, mediumSize.x);
            mediumSize.y = Mathf.Max(3, mediumSize.y);
            hardSize.x = Mathf.Max(4, hardSize.x);
            hardSize.y = Mathf.Max(4, hardSize.y);
            impossibleSize.x = Mathf.Max(5, impossibleSize.x);
            impossibleSize.y = Mathf.Max(5, impossibleSize.y);

            atlasColumns = Mathf.Max(1, atlasColumns);
            atlasRows = Mathf.Max(1, atlasRows);

            // Determine required mini-image slots: based on largest grid
            int maxCols = Mathf.Max(easySize.x, mediumSize.x, hardSize.x, impossibleSize.x);
            int maxRows = Mathf.Max(easySize.y, mediumSize.y, hardSize.y, impossibleSize.y);
            int required = Mathf.CeilToInt((maxCols * maxRows) / 2f);
            int available = atlasColumns * atlasRows;

            if (available < required)
            {
                Debug.LogError(
                    $"Atlas grid ({atlasColumns}×{atlasRows}) must contain at least {required} mini-images " +
                    $"for the largest board ({maxCols}×{maxRows}), but only {available} slots are defined.",
                    this);
            }

            // Ensure matching panel is not overloaded
            if ((easySize.y > 6) || (mediumSize.y > 6) || (hardSize.y > 6) || (impossibleSize.y > 6))
            {
                Debug.LogError("Y size must not exceed 6 cells");
            }

            if ((easySize.x > 10) || (mediumSize.x > 10) || (hardSize.x > 10) || (impossibleSize.x > 10))
            {
                Debug.LogError("X size must not exceed 10 cells");
            }

            // Ensure total cell count is even for each difficulty
            if ((easySize.x * easySize.y) % 2 != 0)
                Debug.LogError($"Easy grid ({easySize.x}×{easySize.y}) must have an even number of cells.", this);
            if ((mediumSize.x * mediumSize.y) % 2 != 0)
                Debug.LogError($"Medium grid ({mediumSize.x}×{mediumSize.y}) must have an even number of cells.", this);
            if ((hardSize.x * hardSize.y) % 2 != 0)
                Debug.LogError($"Hard grid ({hardSize.x}×{hardSize.y}) must have an even number of cells.", this);
            if ((impossibleSize.x * impossibleSize.y) % 2 != 0)
                Debug.LogError($"Impossible grid ({impossibleSize.x}×{impossibleSize.y}) must have an even number of cells.", this);

            if (atlas == null)
                Debug.LogError("Please assign an atlas sprite.", this);

            EditorUtility.SetDirty(this);
        }
#endif
    }
}
