using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AGDDPlatformer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Players")]
        public PlayerController[] players;

        [Header("Objects")]
        public KinematicObject[] kinematicObjectobjects;

        [Header("Level")]
        public PlayerGoal[] playerGoals;
        public bool timeStopped;
        public bool isGameComplete;
        public string firstLevel;
        public string nextLevel;

        [Header("Level Transition")]
        public GameObject startScreen;
        public GameObject endScreen;
        public GameObject gameOverScreen;
        public float startScreenTime = 1.0f;
        public float endScreenDelay = 1.0f;
        public float endScreenTime = 1.0f;

        [Header("Audio")]
        public AudioSource source;
        public AudioClip winSound;

        [Header("Checkpoint")]
        public Vector2 checkPointPosition;

        void Awake()
        {
            instance = this;

            if (playerGoals.Length == 0)
            {
                playerGoals = FindObjectsOfType<PlayerGoal>();
            }
        }

        void Start()
        {
            timeStopped = true;

            endScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            startScreen.SetActive(false);

            timeStopped = false;
        }

        void Update()
        {
            if (isGameComplete)
            {
                if (Input.GetButtonDown("Reset"))
                {
                    ResetGame();
                }
            }

            if (timeStopped)
                return;

            /* --- Check Player Goals --- */

            bool allGoalsSatisfied = true;
            foreach (PlayerGoal playerGoal in playerGoals)
            {
                if (!playerGoal.isSatisfied)
                {
                    allGoalsSatisfied = false;
                    break;
                }
            }

            if (allGoalsSatisfied)
            {
                source.PlayOneShot(winSound);
                StartCoroutine(LevelCompleted());
            }

            if (Input.GetButtonDown("Reset"))
            {
                ResetLevel();
            }
        }

        IEnumerator LevelCompleted()
        {
            timeStopped = true;

            yield return new WaitForSeconds(endScreenDelay);

            endScreen.SetActive(true);

            yield return new WaitForSeconds(endScreenTime);

            if (!string.IsNullOrEmpty(nextLevel))
            {
                SceneManager.LoadScene(nextLevel);
            }
            else
            {
                isGameComplete = true;
                gameOverScreen.SetActive(true);
            }
        }

        void ResetGame()
        {
            SceneManager.LoadScene(firstLevel);
            
        }

        public void ResetLevel()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            foreach (PlayerController player in players)
            {
                player.ResetPlayer();
            }
        }

        public void SetCheckpointPosition(Vector3 newPos)
        {
            checkPointPosition = newPos;


            foreach (var player in players)
            {
                player.Die();
            }
            StartCoroutine(ResetLevelAfterDelay(1f));
        }

        private IEnumerator ResetLevelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
