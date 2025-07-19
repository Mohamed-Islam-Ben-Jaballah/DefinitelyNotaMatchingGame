using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingGame
{
    [CreateAssetMenu(
        fileName = "ScoringHandler",
        menuName = "MatchingGame/Scoring Handler",
        order = 0)]
    public class ScoringHandler : ScriptableObject
    {
        [Header("Scoring System Configuration")]
        [Tooltip("Points rewarded on correct match")]
        [SerializeField] private int baseScore = 1;
        [Tooltip("Combo rewarded on consecutive correct match")]
        [SerializeField] private int comboMultiplier = 2;
        [Tooltip("Current total score")]
        [SerializeField] public int score = 0;
        [Tooltip("Current Combo Multiplier")]
        [SerializeField] public int combo = 1;




        public void CorrectMatch()
        {
            combo *= comboMultiplier;
            score += baseScore * combo;
            GameManager.Instance.UpdateUI();
        }

        public void IncorrectMatch()
        {
            combo = 1;
            GameManager.Instance.UpdateUI();
        }

        
    }
}