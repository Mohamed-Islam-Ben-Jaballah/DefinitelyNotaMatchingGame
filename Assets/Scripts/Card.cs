using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MatchingGame
{
    [RequireComponent(typeof(Button))]
    public class Card : MonoBehaviour
    {
        private ConfigurationHandler configurationHandler;

        [Header("Grid Coordinates (0-based)")]
        [Tooltip("X index in the atlas grid")]
        [SerializeField] private int coordX = 0;

        [Tooltip("Y index in the atlas grid")]
        [SerializeField] private int coordY = 0;

        [Tooltip("Assign a UI RawImage to render the atlas slice")]
        [SerializeField] private RawImage rawImage;

        private void Awake()
        {
            configurationHandler = GameManager.Instance.ConfigHandler;

            if (rawImage == null)
                rawImage = GetComponentInChildren<RawImage>();
            
        }

        private void OnEnable()
        {
            ApplySlice(coordX, coordY);
        }

        /// <summary>
        /// Sets the UV rect on the RawImage to show only the (x,y) cell of the atlas.
        /// </summary>
        /// 
        public void ApplySlice(Vector2Int v)
        {
            ApplySlice(v.x, v.y);
        }
        public void ApplySlice(int x, int y)
        {
            if (configurationHandler.Atlas == null || configurationHandler.AtlasColumns < 1 || configurationHandler.AtlasRows < 1 || rawImage == null)
                return;

            // assign texture
            rawImage.texture = configurationHandler.Atlas.texture;

            // size of each cell in UV space
            float cellW = 1f / configurationHandler.AtlasColumns;
            float cellH = 1f / configurationHandler.AtlasRows;

            // normalized offsets
            float uOffset = configurationHandler.HorizontalOffset * cellW;
            float vOffset = configurationHandler.VerticalOffset * cellH;

            // compute UV origin; invert Y for UV space
            float u = x * cellW + uOffset;
            float v = 1f - ((y + 1) * cellH) - vOffset;

            rawImage.uvRect = new Rect(u, v, cellW, cellH);
        }
    }
}

