using UnityEngine;
using TMPro;

namespace MatchingGame
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Timer : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("TextMeshProUGUI component used to display the time")]
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Settings")]
        [Tooltip("Start counting automatically on Enable")]
        [SerializeField] private bool startOnEnable = true;

        public float timeLeft = 0f;
        private bool running = false;

        public event System.Action OnTimeExpired;
        private float timeLimit = 0;

        private void Awake()
        {
            // Ensure we have a reference to the TMP component
            if (timeText == null)
                timeText = GetComponent<TextMeshProUGUI>();

            timeLimit = GameManager.Instance.ConfigHandler.CurrentTimeLimit();
        }
        private void OnEnable()
        {
            ResetTimer();
            if (startOnEnable)
                StartTimer();
        }

        private void Update()
        {
            if (!running) return;
            timeLeft -= Time.deltaTime;
            UpdateDisplay();
            if (timeLeft <= 0f)
            {
                running = false;
                OnTimeExpired.Invoke();
            }
        }

        /// <summary>
        /// Formats and updates the TMP text to show mm:ss:ms
        /// </summary>
        private void UpdateDisplay()
        {
            int minutes = (int)(timeLeft / 60f);
            int seconds = (int)(timeLeft % 60f);
            int milliseconds = (int)((timeLeft * 1000f) % 1000f);

            timeText.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliseconds/100);
        }

        /// <summary>Begin or resume the stopwatch.</summary>
        public void StartTimer() => running = true;

        /// <summary>Pause the stopwatch (retains elapsed time).</summary>
        public void StopTimer() => running = false;

        /// <summary>Reset elapsed time to zero and update display.</summary>
        public void ResetTimer()
        {
            timeLimit = GameManager.Instance.ConfigHandler.CurrentTimeLimit();
            timeLeft = timeLimit;
            UpdateDisplay();
        }

        /// <summary>Toggle running state: if running, stop; if stopped, start.</summary>
        public void ToggleTimer() => running = !running;
    }

}

