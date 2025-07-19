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

        private float elapsedTime = 0f;
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
            elapsedTime -= Time.deltaTime;
            UpdateDisplay();
            if (elapsedTime <= 0f)
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
            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            int milliseconds = (int)((elapsedTime * 1000f) % 1000f);

            timeText.text = string.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, milliseconds/100);
        }

        /// <summary>Begin or resume the stopwatch.</summary>
        public void StartTimer() => running = true;

        /// <summary>Pause the stopwatch (retains elapsed time).</summary>
        public void StopTimer() => running = false;

        /// <summary>Reset elapsed time to zero and update display.</summary>
        public void ResetTimer()
        {
            elapsedTime = timeLimit;
            UpdateDisplay();
        }

        /// <summary>Toggle running state: if running, stop; if stopped, start.</summary>
        public void ToggleTimer() => running = !running;
    }

}

