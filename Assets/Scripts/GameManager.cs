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


        [Header("Game Configurations")]
        [SerializeField] private ConfigurationHandler configHandler;
        public ConfigurationHandler ConfigHandler => configHandler;
       
        [SerializeField] private ScoringHandler scoringHandler;
        public ScoringHandler ScoringHandler => scoringHandler;

        [Header("UI fields")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject inGamePanel;
        [SerializeField] private GameObject inMenuPanel;

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
            inGamePanel.SetActive(true);    
            gameOverTriggered = false;
        }

        public void UpdateUI()
        {
            scoreText.text = scoringHandler.score.ToString();
            comboText.text = "X " + scoringHandler.combo.ToString();
        }

        public void GameOver(bool won)
        {
            if (gameOverTriggered) return;
            gameOverTriggered = true;

            
            comparisonInProgress = true; // Stop any further comparisons / input
            pendingCard = null;

            //block interactions
            foreach (GameObject cardGo in cardsGo)
            {
                if (cardGo && cardGo.activeSelf)
                {
                    cardGo.GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
            }

            // Save basic results
            PlayerPrefs.SetInt("MG_LastScore", scoringHandler.score);
            PlayerPrefs.SetInt("MG_LastCombo", scoringHandler.combo);
            PlayerPrefs.SetInt("MG_Won", won ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log(won ? "[GameOver] Player WON." : "[GameOver] Player LOST.");
        }

    }

}
