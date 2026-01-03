using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using PlatformerGame.Inventory;
using PlatformerGame.Player;

namespace PlatformerGame.Managers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }
        
        [Header("Transition Settings")]
        [SerializeField] private float transitionTime = 0.5f;
        [SerializeField] private Animator transitionAnimator;
        
        private bool isTransitioning = false;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            Debug.Log("SceneTransitionManager initialized");
        }
        
        public void LoadScene(int sceneIndex)
        {
            if (isTransitioning) return;
            
            StartCoroutine(LoadSceneRoutine(sceneIndex));
        }
        
        private IEnumerator LoadSceneRoutine(int sceneIndex)
        {
            if (isTransitioning) yield break;
            isTransitioning = true;
            
            Debug.Log($"Transitioning to scene {sceneIndex}");
            
            // Play transition animation
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("Start");
            }
            
            // Save game state before transition
            SaveBeforeTransition();
            
            // Wait for transition
            yield return new WaitForSeconds(transitionTime);
            
            // Load scene
            SceneManager.LoadScene(sceneIndex);
            
            // Wait for scene to load
            yield return new WaitForSeconds(0.1f);
            
            // End transition
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("End");
            }
            
            isTransitioning = false;
        }
        
        private void SaveBeforeTransition()
        {
            Debug.Log("Saving before scene transition");
            
            // Save current scene
            PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
            
            // Save inventory through GameManager
            GameManager.Instance?.SaveGameState();
            
            // Save player position if possible
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerPrefs.SetFloat("PlayerPositionX", player.transform.position.x);
                PlayerPrefs.SetFloat("PlayerPositionY", player.transform.position.y);
                
                // Save player health if exists
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    PlayerPrefs.SetInt("PlayerHealth", playerHealth.CurrentHealth);
                }
            }
            
            PlayerPrefs.Save();
        }
        
        public void LoadNextLevel()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            
            // Check if next scene exists
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No next level available, returning to main menu");
                LoadScene(0); // Main menu
            }
        }
        
        public void RestartCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void ReturnToMainMenu()
        {
            LoadScene(0);
        }
        
        // Quick load methods for common scenes
        //public void LoadMainMenu() => LoadScene(0);
        //public void LoadFirstLevel() => LoadScene(GameManager.Instance?.firstLevelScene ?? 1);
        
        // Transition with callback
        public void LoadSceneWithCallback(int sceneIndex, System.Action onComplete = null)
        {
            StartCoroutine(LoadSceneWithCallbackRoutine(sceneIndex, onComplete));
        }
        
        private IEnumerator LoadSceneWithCallbackRoutine(int sceneIndex, System.Action onComplete)
        {
            yield return StartCoroutine(LoadSceneRoutine(sceneIndex));
            onComplete?.Invoke();
        }
        
        // Check if scene exists
        public bool SceneExists(int sceneIndex)
        {
            return sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings;
        }
        
        // Get current scene info
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        public int GetCurrentSceneIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }
}