using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingGame
{
    [CreateAssetMenu(
        fileName = "ScoringHandler",
        menuName = "MatchingGame/Scoring Handler",
        order = 0)]
    public class ScoringConfiguration : ScriptableObject
    {
        [Header("Scoring System Configuration")]
        [Tooltip("Points rewarded on correct match")]
        [SerializeField] public int baseScore = 1;
        [Tooltip("Combo rewarded on consecutive correct match")]
        [SerializeField] public int comboMultiplier = 2;              
    }
}