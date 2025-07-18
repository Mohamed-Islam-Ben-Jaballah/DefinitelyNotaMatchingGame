using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;


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
        [SerializeField] private RawImage ContentImage;

        [Tooltip("Assign a UI RawImage to render the card edge")]
        [SerializeField] private RawImage EdgeImage;

        [Tooltip("Assign a UI RawImage to render the card back")]
        [SerializeField] private RawImage BackImage;

        [Tooltip("flipping animation duration")]
        [SerializeField] private float flipDuration = 1;

        [Tooltip("Current State of card")]
        [SerializeField] private bool Up=false;






        private void Awake()
        {
            configurationHandler = GameManager.Instance.ConfigHandler;
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
            if (configurationHandler.Atlas == null || configurationHandler.AtlasColumns < 1 || configurationHandler.AtlasRows < 1 || ContentImage == null)
                return;

            // assign texture
            ContentImage.texture = configurationHandler.Atlas.texture;

            // size of each cell in UV space
            float cellW = 1f / configurationHandler.AtlasColumns;
            float cellH = 1f / configurationHandler.AtlasRows;

            // normalized offsets
            float uOffset = configurationHandler.HorizontalOffset * cellW;
            float vOffset = configurationHandler.VerticalOffset * cellH;

            // compute UV origin; invert Y for UV space
            float u = x * cellW + uOffset;
            float v = 1f - ((y + 1) * cellH) - vOffset;

            ContentImage.uvRect = new Rect(u, v, cellW, cellH);
        }

        public void Reveal()
        {
            if (Up) return;

            Up = true;

            // cancel any running flip tweens
            LeanTween.cancel(gameObject);

            // Phase 1: rotate to 90°
            LeanTween.rotateY(gameObject, 90f, flipDuration)
                     .setEase(LeanTweenType.easeOutSine)
                     .setOnComplete(OnHalfFlipComplete);

        }

        public void Hide()
        {
            if (!Up) return;

            Up = false;

            // cancel any running flip tweens
            LeanTween.cancel(gameObject);

            // Phase 1: rotate to 90°
            LeanTween.rotateY(gameObject, 90f, flipDuration)
                     .setEase(LeanTweenType.easeOutSine)
                     .setOnComplete(OnHalfFlipComplete);

        }

        private void OnHalfFlipComplete()
        {
            // swap artwork
            if (Up)
            {
                BackImage.gameObject.SetActive(false);
                EdgeImage.gameObject.SetActive(true);
            }
            else
            {
                EdgeImage.gameObject.SetActive(false);
                BackImage.gameObject.SetActive(true);
            }


            // Phase 2: rotate back to 0° (but showing the other side)
            LeanTween.rotateY(gameObject, 0f, flipDuration)
                     .setEase(LeanTweenType.easeInSine);
        }
    }
}

