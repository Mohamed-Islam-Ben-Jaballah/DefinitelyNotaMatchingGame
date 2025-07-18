using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame
{
    public class DifficultyButton : MonoBehaviour
    {
        [SerializeField] private ConfigurationHandler handler;
        [SerializeField] private Button button;
        [SerializeField] private Image[] skulls;  // assign 4 skull images in order

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(AdvanceDifficulty);
            UpdateSkulls();
        }

        private void AdvanceDifficulty()
        {
            var next = (Difficulty)(((int)handler.difficulty + 1) % System.Enum.GetValues(typeof(Difficulty)).Length);
            handler.difficulty = next;
            UpdateSkulls();
        }

        private void UpdateSkulls()
        {
            int idx = (int)handler.difficulty;
            for (int i = 0; i < skulls.Length; i++)
                skulls[i].gameObject.SetActive(i <= idx);
        }
    }
}
