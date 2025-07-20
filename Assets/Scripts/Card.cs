using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame
{
    [RequireComponent(typeof(Button))]
    public class Card : MonoBehaviour
    {
        // ---------- STATIC GLOBAL RESOLUTION STATE ----------
        /// <summary>Ordered list of all face-up, *unresolved* cards (reveal order).</summary>
        public static readonly List<Card> UnresolvedFaceUp = new List<Card>(8);

        /// <summary>Try to resolve as many ready pairs as possible (first two finished face-up flips).</summary>
        private static void TryResolvePairs()
        {
            int i = 0;
            // Process pairs while at least two and both not flipping and face-up
            while (UnresolvedFaceUp.Count - i >= 2)
            {
                Card first = UnresolvedFaceUp[i];
                Card second = UnresolvedFaceUp[i + 1];

                // If either still flipping, we must wait; earlier pair blocks later pairs
                if (first.IsFlipping || second.IsFlipping) break;
                if (!first.Up || !second.Up) break; // safety

                ResolvePair(first, second);
                // After removal, continue from same index (0) since list shifts
            }
        }

        /// <summary>Resolves the earliest pair (already guaranteed to be ready).</summary>
        private static void ResolvePair(Card a, Card b)
        {
            // Remove them from the unresolved list first (index 0 twice)
            UnresolvedFaceUp.RemoveAt(0);
            UnresolvedFaceUp.RemoveAt(0);

            bool match = a.Matches(b);

            if (match)
            {
                a.ResolveAsMatch();
                b.ResolveAsMatch();
                GameManager.Instance.CorrectMatch();
            }
            else
            {
                a.ResolveAsMismatchAndHide();
                b.ResolveAsMismatchAndHide();
                GameManager.Instance.IncorrectMatch();
            }

            // After mismatch hides complete, new selections can still be added concurrently;
            // any already-completed later pairs will get processed by their own OnFlipComplete calls.
        }

        // ---------- INSTANCE FIELDS ----------
        private ConfigurationHandler configurationHandler;

        [Header("Atlas Coords")]
        [SerializeField] public int coordX;
        [SerializeField] public int coordY;

        [Header("Visuals")]
        [SerializeField] private RawImage ContentImage;
        [SerializeField] private RawImage EdgeImage;
        [SerializeField] private RawImage BackImage;

        [Header("Flip Settings")]
        [SerializeField] private float flipDuration = 0.25f;
        [SerializeField] private float succefulMatchAnimationRatio = 0.33f;
        // State flags
        public bool Up { get; private set; }
        public bool IsFlipping { get; private set; }
        private bool resolved;          // permanently resolved (matched) – not reselectable
        private bool targetFaceUp;      // target orientation of current flip

        private void Awake()
        {
            configurationHandler = GameManager.Instance.ConfigHandler;
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            LeanTween.cancel(gameObject);
            IsFlipping = false;
        }

        private void Init()
        {
            LeanTween.cancel(gameObject);
            transform.localEulerAngles = Vector3.zero;
            EdgeImage.gameObject.SetActive(false);
            BackImage.gameObject.SetActive(true);
            Up = false;
            IsFlipping = false;
            resolved = false;
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

            //save atlas coordinates on card
            coordX = x;
            coordY = y;

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

        public void OnCardPressed()
        {
            if (resolved) return;
            if (Up || IsFlipping) return;  // already face-up or mid flip

            // Mark logical state first
            Up = true;

            // Add to unresolved reveal order
            UnresolvedFaceUp.Add(this);

            // Start flip-up animation
            StartFlip(true);

            //Play SFX
            AudioManager.instance.PlayFlip();
        }

        public bool Matches(Card other)
            => other.coordX == coordX && other.coordY == coordY;

        private void ResolveAsMatch()
        {
            if (resolved) return;
            resolved = true;
            LeanTween.cancel(gameObject);

            Vector3 original = transform.localScale;
            Vector3 peak = original * (1 + succefulMatchAnimationRatio);

            LeanTween.scale(gameObject, peak, flipDuration*(1- succefulMatchAnimationRatio))
                     .setEase(LeanTweenType.easeOutBack)
                     .setOnComplete(() =>
                     {
                         LeanTween.scale(gameObject, original, flipDuration * succefulMatchAnimationRatio)
                                  .setEase(LeanTweenType.easeInQuad)
                                  .setOnComplete(() =>
                                  {
                                      gameObject.SetActive(false);
                                  });
                     });
        }

        private void ResolveAsMismatchAndHide()
        {
            StartFlip(false);
        }

        // ---------- FLIP ANIMATION ----------
        private void StartFlip(bool toFaceUp)
        {
            targetFaceUp = toFaceUp;

            if (IsFlipping) LeanTween.cancel(gameObject);
            IsFlipping = true;

            float half = flipDuration * 0.5f;

            // First half 0 -> 90
            LeanTween.rotateY(gameObject, 90f, half)
                     .setEase(LeanTweenType.easeOutSine)
                     .setOnComplete(() =>
                     {
                         // Midpoint face swap
                         if (targetFaceUp)
                         {
                             BackImage.gameObject.SetActive(false);
                             EdgeImage.gameObject.SetActive(true);
                         }
                         else
                         {
                             EdgeImage.gameObject.SetActive(false);
                             BackImage.gameObject.SetActive(true);
                         }

                         // Second half 90 -> 0
                         LeanTween.rotateY(gameObject, 0f, half)
                                  .setEase(LeanTweenType.easeInSine)
                                  .setOnComplete(OnFlipComplete);
                     });
        }

        private void OnFlipComplete()
        {
            IsFlipping = false;
            Up = targetFaceUp;

            // If we flipped face-down (after mismatch) we are simply reusable; nothing to resolve
            if (!Up) return;

            // If we flipped face-up, attempt to resolve pairs (this may resolve multiple if ready)
            TryResolvePairs();
        }
    }
}
