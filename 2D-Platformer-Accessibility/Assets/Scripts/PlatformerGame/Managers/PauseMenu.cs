using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace PlatformerGame.Managers
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButton;
        
        [Header("Options")]
        [SerializeField] private GameObject optionsPanel;

        private PauseMenuManager pauseMenuManager;
        
        private bool isPaused = false;
        
        private void Start()
        {
            pauseMenuManager = FindObjectOfType<PauseMenuManager>();

            // Initialize buttons
            //InitializeUI();
        }

        private void InitializeUI()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(ResumeGame);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartLevel);
                
            if (optionsButton != null)
                optionsButton.onClick.AddListener(ShowOptions);
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
            // Always show pause menu panel initially
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            
            // Hide options initially
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }
        
        /*public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(isPaused);
            
            Time.timeScale = isPaused ? 0f : 1f;
            
            // Show/hide cursor
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            
            Debug.Log($"Game {(isPaused ? "Paused" : "Resumed")}");
        }*/

        #region UI Button Handlers
        
        public void ResumeGame()
        {
            pauseMenuManager.ResumeGame();
            /*isPaused = false;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
                
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;*/
        }

        public void RestartLevel()
        {
            pauseMenuManager.RestartLevel();
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
            pauseMenuManager.ReturnToMainMenu();
            /*Time.timeScale = 1f; // Ensure time is running
            
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
            }*/
        }
        
        public void QuitGame()
        {
            pauseMenuManager.QuitGame();
            /*#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif*/
        }

        #endregion

        #region Input System Callbacks
        
        // Called by PlayerInput component
        /*public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            TogglePause();
        }
        
        public void OnUICancel(InputAction.CallbackContext context)
        {
            if (!context.performed || !isPaused) return;
            
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                ShowOptions(); // Toggle options off
            }
            else
            {
                ResumeGame(); // Resume if on main pause menu
            }
        }*/
        
        #endregion
        
        private void OnDestroy()
        {
            // Ensure time scale is reset when this object is destroyed
            Time.timeScale = 1f;
        }
    }
}