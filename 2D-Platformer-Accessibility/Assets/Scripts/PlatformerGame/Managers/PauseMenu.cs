using UnityEngine;
using UnityEngine.UI;

namespace PlatformerGame.Managers
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Options")]
        [SerializeField] private GameObject optionsPanel;
        
        private bool isPaused = false;
        
        private void Start()
        {
            // Initialize buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(ResumeGame);
                
            if (optionsButton != null)
                optionsButton.onClick.AddListener(ShowOptions);
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
            
            // Hide menus initially
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
                
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }
        
        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(isPaused);
            
            Time.timeScale = isPaused ? 0f : 1f;
            
            // Show/hide cursor
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            
            Debug.Log($"Game {(isPaused ? "Paused" : "Resumed")}");
        }
        
        public void ResumeGame()
        {
            isPaused = false;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
                
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public void ShowOptions()
        {
            if (optionsPanel != null)
            {
                optionsPanel.SetActive(true);
            }
        }
        
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // Ensure time is running
            
            // Use the main menu manager if it exists
            MainMenuManager menuManager = FindObjectOfType<MainMenuManager>();
            if (menuManager != null)
            {
                menuManager.ReturnToMainMenu();
            }
            else
            {
                // Fallback
                SceneTransitionManager.Instance?.LoadScene("MainMenu");
            }
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        private void OnDestroy()
        {
            // Ensure time scale is reset when this object is destroyed
            Time.timeScale = 1f;
        }
    }
}