using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using PlatformerGame.Inventory;
using PlatformerGame.Player;
using PixeLadder.EasyTransition;

namespace PlatformerGame.Managers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }
        
        [Header("Transition Settings")]
        [SerializeField] private Animator transitionAnimator;
        [SerializeField] private SceneTransitioner sceneTransitioner;
        [SerializeField] private TransitionEffect transitionEffect;
        [SerializeField] private float transitionTime = 1f;
        
        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider loadingSlider;
        
        private static int currentSceneIndex = 1;
        private AsyncOperation loadingOperation;
        
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
            }
        }

        private void ShowTransitionEffect()
        {
            // The "next scene" is just this same scene, which we will reload.
            string sceneToLoad = SceneManager.GetActiveScene().name;
            TransitionEffect effectToUse = transitionEffect;

            // Call the SceneTransitioner to start the transition.
            SceneTransitioner.Instance.LoadScene(sceneToLoad, effectToUse);
        }
        
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
        
        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            // Save game state before transition
            SaveBeforeTransition();
            
            // Start transition out
            if (transitionAnimator != null)
                transitionAnimator.SetTrigger("Start");
            
            yield return new WaitForSeconds(transitionTime);
            
            // Show loading screen
            if (loadingScreen != null)
                loadingScreen.SetActive(true);
            
            // Load scene asynchronously
            loadingOperation = SceneManager.LoadSceneAsync(sceneName);
            loadingOperation.allowSceneActivation = false;
            
            // Update loading progress
            while (!loadingOperation.isDone)
            {
                float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
                
                if (loadingSlider != null)
                    loadingSlider.value = progress;
                
                if (loadingOperation.progress >= 0.9f)
                {
                    // Wait a moment for effect
                    yield return new WaitForSeconds(0.5f);
                    
                    // Hide loading screen
                    if (loadingScreen != null)
                        loadingScreen.SetActive(false);
                    
                    // Allow scene activation
                    loadingOperation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            // Transition in
            if (transitionAnimator != null)
                transitionAnimator.SetTrigger("End");
        }
        
        private void SaveBeforeTransition()
        {
            // Save current scene
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            
            // Save inventory
            InventoryManager.Instance?.SaveInventory();
            
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
                    // You'll need to add CurrentHealth property to PlayerHealth
                    // PlayerPrefs.SetInt("PlayerHealth", playerHealth.CurrentHealth);
                }
            }
            
            PlayerPrefs.Save();
        }
        
        public void LoadNextLevel()
        {
            if (transitionEffect != null)
            {
                ShowTransitionEffect();
            }
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                LoadSceneByIndex(nextSceneIndex);
            }
            else
            {
                LoadScene("MainMenu");
            }
        }
        
        public void LoadSceneByIndex(int sceneIndex)
        {
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            LoadScene(sceneName);
        }
        
        public void RestartCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // Call this when player dies
        public void GameOver()
        {
            StartCoroutine(GameOverRoutine());
        }
        
        private IEnumerator GameOverRoutine()
        {
            // gameOverScreen.SetActive(true);
            
            yield return new WaitForSeconds(3f);
            
            // Return to main menu or restart
            LoadScene("MainMenu");
        }
    }
}