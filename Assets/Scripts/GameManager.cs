using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;


namespace MatchingGame
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public List<GameObject> cardsGo = new List<GameObject>();

        public Card pendingCard;

        public bool comparisonInProgress;

        public int unresolvedPairs;     
        private bool gameOverTriggered = false;

        [Header("Game Score")]
        [Tooltip("Current total score")]
        [SerializeField] public int score = 0;
        [Tooltip("Current Combo Multiplier")]
        [SerializeField] public int combo = 1;


        [Header("Game Configurations")]
        [SerializeField] private ConfigurationHandler configHandler;
        public ConfigurationHandler ConfigHandler => configHandler;
       
        [SerializeField] private ScoringConfiguration scoringHandler;
        public ScoringConfiguration ScoringHandler => scoringHandler;

        [Header("UI fields")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject inGamePanel;
        [SerializeField] private GameObject inMenuPanel;
        [SerializeField] private GameObject GameWonPanel;
        [SerializeField] private GameObject GameLostPanel;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject("GameManager");
                        instance = singletonObject.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        private void HandleTimeExpired()
        {
            if (!gameOverTriggered)
                GameOver(false); // loss
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            try
            {
                timerText.gameObject.GetComponent<Timer>().OnTimeExpired += HandleTimeExpired;
            }
            catch { }
        }

        public void LaunchGame()
        {
            inMenuPanel.SetActive(false);
            GameWonPanel.SetActive(false);
            GameLostPanel.SetActive(false);
            inGamePanel.SetActive(true);    
            gameOverTriggered = false;
        }

        public void UpdateUI()
        {
            scoreText.text = score.ToString();
            comboText.text = "X " + combo.ToString();
        }

        public void CorrectMatch()
        {
            combo *= scoringHandler.comboMultiplier;
            score += scoringHandler.baseScore * combo;
            AudioManager.instance.PlayMatch(0.6f + combo * 0.06f);
            PairMatched();
            UpdateUI();

        }

        public void IncorrectMatch()
        {
            AudioManager.instance.PlayMismatch(1.9f - combo * 0.09f);
            combo = 1;
            UpdateUI();
        }

        public void PairMatched()
        {
            if (gameOverTriggered) return;
            unresolvedPairs--;
            if (unresolvedPairs <= 0)
                GameOver(true);
        }

        public void GameOver(bool won)
        {
            if (gameOverTriggered) return;
            gameOverTriggered = true;

            
            comparisonInProgress = true; // Stop any further comparisons / input
            pendingCard = null;

            foreach (GameObject cardGo in cardsGo)
            {
                Destroy(cardGo);
            }
            cardsGo = new List<GameObject>();

            if (won)
            {
                configHandler.AdvanceDifficulty();
            }

            inGamePanel.SetActive(false);
            inMenuPanel.SetActive(false);
            GameWonPanel.SetActive(won);
            GameLostPanel.SetActive(!won);

            // Save basic results
            PlayerPrefs.SetInt("MG_LastScore", score);
            PlayerPrefs.SetInt("MG_LastCombo", combo);
            PlayerPrefs.SetInt("MG_Won", won ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log(won ? "[GameOver] Player WON." : "[GameOver] Player LOST.");


        }

    }

}
