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
        fileName = "DifficultyHandler",
        menuName = "MatchingGame/Difficulty Handler",
        order = 0)]
    public class DifficultyHandler : ScriptableObject
    {
        [Header("Default Starting Difficulty")]
        [Tooltip("Used on first run if no saved value exists")]
        [SerializeField]
        private Difficulty defaultDifficulty = Difficulty.Easy;

        [Header("Grid Size per Difficulty (size × size)")]
        [Min(2)][SerializeField] private int easySize = 2;
        [Min(3)][SerializeField] private int mediumSize = 3;
        [Min(4)][SerializeField] private int hardSize = 4;
        [Min(5)][SerializeField] private int impossibleSize = 5;

        [Header("Tiles Atlas")]
        [Tooltip("The atlas containing all tile images")]
        [SerializeField] private Sprite atlas;

        [Min(1)]
        [Tooltip("Number of columns in the atlas grid")]
        [SerializeField] private int atlasColumns = 5;

        [Min(1)]
        [Tooltip("Number of rows in the atlas grid")]
        [SerializeField] private int atlasRows = 5;

        [Header("Atlas Offsets (in pixels)")]
        [Tooltip("Horizontal offset between tiles in the atlas")]
        [SerializeField] private float horizontalOffset = 0f;

        [Tooltip("Vertical offset between tiles in the atlas")]
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
        /// Returns the grid size for a given difficulty.
        /// </summary>
        public int SizeFor(Difficulty d)
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

        /// <summary>
        /// The sprite atlas containing all tiles.
        /// </summary>
        public Sprite Atlas => atlas;

        public int AtlasColumns => atlasColumns;
        public int AtlasRows => atlasRows;
        public float HorizontalOffset => horizontalOffset;
        public float VerticalOffset => verticalOffset;

        private void OnEnable()
        {
            // Load saved difficulty if present, otherwise use the default
            if (PlayerPrefs.HasKey("difficulty") &&
                System.Enum.TryParse(PlayerPrefs.GetString("difficulty"), out Difficulty saved))
            {
                _difficulty = saved;
            }
            else
            {
                _difficulty = defaultDifficulty;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure minimum grid sizes
            easySize = Mathf.Max(2, easySize);
            mediumSize = Mathf.Max(3, mediumSize);
            hardSize = Mathf.Max(4, hardSize);
            impossibleSize = Mathf.Max(5, impossibleSize);

            atlasColumns = Mathf.Max(1, atlasColumns);
            atlasRows = Mathf.Max(1, atlasRows);

            // Calculate required mini-image count based on the largest grid
            int maxSize = Mathf.Max(easySize, mediumSize, hardSize, impossibleSize);
            int required = Mathf.CeilToInt((maxSize * maxSize) / 2f);
            int available = atlasColumns * atlasRows;

            if (available < required)
            {
                Debug.LogError(
                    $"Atlas grid ({atlasColumns}×{atlasRows}) must contain at least {required} mini-images " +
                    $"for the largest board ({maxSize}×{maxSize}), but only {available} slots are defined.",
                    this);
            }

            if (atlas == null)
            {
                Debug.LogError("Please assign an atlas sprite.", this);
            }

            // Mark asset dirty so Unity will re-serialize
            EditorUtility.SetDirty(this);
        }
#endif
    }
}