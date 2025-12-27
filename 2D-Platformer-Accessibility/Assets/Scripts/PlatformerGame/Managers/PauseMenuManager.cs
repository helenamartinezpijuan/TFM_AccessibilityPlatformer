using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlatformerGame.Managers
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Pause Menu Prefab")]
        [SerializeField] private GameObject pauseMenuPrefab;

        [Header("Input System")]
        [SerializeField] private PlayerInput playerInput;
        
        private bool isPaused = false;
        private GameObject currentPauseMenu;

        private void Start()
        {
            // Make sure we have a player input reference
            if (playerInput == null)
            {
                playerInput = FindObjectOfType<PlayerInput>();
            }
        }

        #region Input System Callbacks
        
        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            TogglePause();
        }
        
        #endregion
        
        #region Pause Logic
        
        public void TogglePause()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                ShowPauseMenu();
            }
            else
            {
                HidePauseMenu();
            }
        }
        
        private void ShowPauseMenu()
        {
            Time.timeScale = 0f;
            
            // Instantiate pause menu if it doesn't exist
            if (currentPauseMenu == null && pauseMenuPrefab != null)
            {
                currentPauseMenu = Instantiate(pauseMenuPrefab);
            }
            else if (currentPauseMenu != null)
            {
                currentPauseMenu.SetActive(true);
            }

            // Switch input to UI
            /*if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("UI");
            }
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;*/
        }
        
        private void HidePauseMenu()
        {
            Time.timeScale = 1f;
            
            if (currentPauseMenu != null)
            {
                currentPauseMenu.SetActive(false);
            }

            // Switch input back to player
            /*if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("Player");
            }
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;*/
        }
        
        public void ResumeGame()
        {
            isPaused = false;
            HidePauseMenu();
        }

        public void RestartLevel()
        {
            
        }
        
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // Ensure game is unpaused
            
            // Destroy pause menu if it exists
            if (currentPauseMenu != null)
            {
                Destroy(currentPauseMenu);
            }
            
            // Load main menu
            GameManager.Instance?.ReturnToMainMenu();
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        #endregion
    }
}