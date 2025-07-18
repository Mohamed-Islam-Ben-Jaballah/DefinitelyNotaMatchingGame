using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MatchingGame
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField] private ConfigurationHandler configHandler;

        public ConfigurationHandler ConfigHandler => configHandler;

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
        }

        public void LaunchGame()
        {
            SceneManager.LoadSceneAsync("Matching");
        }

    }

}
