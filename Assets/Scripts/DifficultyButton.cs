using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame
{
    public class DifficultyButton : MonoBehaviour
    {
        [SerializeField] private ConfigurationHandler handler;
        [SerializeField] private Image[] skulls;  // assign 4 skull images in order

        private void Awake()
        {
            UpdateDifficulty();
        }

        public void UpdateDifficulty()
        {           
            int DifficultyIdx = (int)handler.AdvanceDifficulty();
            for (int i = 0; i < skulls.Length; i++)
                skulls[i].gameObject.SetActive(i <= DifficultyIdx);
        }
    }
}
