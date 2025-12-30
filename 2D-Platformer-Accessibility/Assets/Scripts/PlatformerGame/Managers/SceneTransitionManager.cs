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
        [SerializeField] private float transitionTime = 0.1f;
        
        //[Header("Loading Screen")]
        //[SerializeField] private GameObject loadingScreen;
        //[SerializeField] private Slider loadingSlider;
        
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
        
        public void LoadScene (int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            //StartCoroutine(LoadSceneRoutine(sceneIndex));
        }
        
        /*private IEnumerator LoadSceneRoutine(int sceneIndex)
        {
            // Save game state before transition
            SaveBeforeTransition();
            
            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(sceneIndex);
            
            // Show loading screen
            if (loadingScreen != null)
                loadingScreen.SetActive(true);
            
            // Load scene asynchronously
            loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
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
        }*/
        
        private void SaveBeforeTransition()
        {
            // Save current scene
            PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
            
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
                    PlayerPrefs.SetInt("PlayerHealth", playerHealth.CurrentHealth);
                }
            }
            
            PlayerPrefs.Save();
        }
        
        public void LoadNextLevel()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            
            LoadScene(nextSceneIndex);
        }
        
        /*public void LoadSceneByIndex(int sceneIndex)
        {
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name; 
            LoadScene(sceneName);
        }*/
        
        public void RestartCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void GameOver()
        {
            StartCoroutine(GameOverRoutine());
        }
        
        private IEnumerator GameOverRoutine()
        {
            // gameOverScreen.SetActive(true);
            
            yield return new WaitForSeconds(3f);
            
            // Return to main menu or restart
            LoadScene(0);
        }
    }
}