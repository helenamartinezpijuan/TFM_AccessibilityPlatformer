using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using PlatformerGame.Player;
using PlatformerGame.Inventory;

namespace PlatformerGame.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button startPanelButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button continuePanelButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject continuePanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        
        [Header("Scene Settings")]
        [SerializeField] private int firstLevelScene = 1;
        [SerializeField] private int savedScene;
        
        [Header("Transition Settings")]
        [SerializeField] private float transitionTime = 1f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource menuAudioSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip startGameSound;
        
        private bool hasSaveData = false;
    

        private void Start()
        {
            // Initialize UI buttons
            InitializeUI();
            
            // Load saved settings
            LoadSettings();
            
            // Set cursor visible
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        #region UI Initialization and Settings

        private void InitializeUI()
        {
            if (startPanelButton != null)
                startPanelButton.onClick.AddListener(OnStartPanel);

            if (startButton != null)
                startButton.onClick.AddListener(OnStartGame);

            if (continuePanelButton != null)
                continuePanelButton.onClick.AddListener(OnContinuePanel);
                
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueGame);
                // Check if save data exists
                hasSaveData = GameManager.Instance?.CheckForSaveData() ?? false;
                continueButton.interactable = hasSaveData;
            }
                
            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptions);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitGame);

            // Initialize start panel
            if (startPanel != null)
                startPanel.SetActive(false);

            // Initialize continue panel
            if (continuePanel != null)
                continuePanel.SetActive(false);

            // Initialize options panel
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }

        private void LoadSettings()
        {
            // Load volume
            if (volumeSlider != null)
            {
                volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }
            
            // Load fullscreen
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            }
        }

        private void PlayButtonSound(AudioClip clip)
        {
            if (menuAudioSource != null && clip != null)
            {
                menuAudioSource.PlayOneShot(clip);
            }
        }

        #endregion

        #region UI Button Handlers
        public void OnStartPanel()
        {
            PlayButtonSound(buttonClickSound);
            
            if (startPanel != null)
            {
                bool isActive = startPanel.activeSelf;
                startPanel.SetActive(!isActive);
                
                // Toggle other buttons
                if (continuePanelButton != null) continuePanelButton.interactable = isActive;
                if (optionsButton != null) optionsButton.interactable = isActive;
                if (quitButton != null) quitButton.interactable = isActive;
            }
        }
        
        public void OnStartGame()
        {
            PlayButtonSound(startGameSound);
            
            // Clear any existing save data for a fresh start
            GameManager.Instance?.ClearSaveData();
            
            // Start transition
            StartCoroutine(StartGameRoutine());
        }

        public void OnContinuePanel()
        {
            PlayButtonSound(buttonClickSound);
            
            if (continuePanel != null)
            {
                bool isActive = continuePanel.activeSelf;
                continuePanel.SetActive(!isActive);
                
                // Toggle other buttons
                if (startPanelButton != null) startPanelButton.interactable = isActive;
                if (optionsButton != null) optionsButton.interactable = isActive;
                if (quitButton != null) quitButton.interactable = isActive;
            }
        }
        
        public void OnContinueGame()
        {
            if (!hasSaveData) return;
            
            PlayButtonSound(startGameSound);
            
            StartCoroutine(ContinueGameRoutine());
        }

        public void OnOptions()
        {
            PlayButtonSound(buttonClickSound);
            
            if (optionsPanel != null)
            {
                bool isActive = optionsPanel.activeSelf;
                optionsPanel.SetActive(!isActive);
                
                // Toggle other buttons
                if (startPanelButton != null) startPanelButton.interactable = isActive;
                if (continuePanelButton != null) continuePanelButton.interactable = isActive;
                if (quitButton != null) quitButton.interactable = isActive;
            }
        }
        
        public void OnQuitGame()
        {
            PlayButtonSound(buttonClickSound);
            GameManager.Instance?.QuitGame();
        }
        #endregion

        #region Options Menu Handlers

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("MasterVolume", value);
            PlayerPrefs.Save();
        }
        
        private void OnFullscreenChanged(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        #endregion
        
        #region Game loop
        private IEnumerator StartGameRoutine()
        {
            // Wait for transition
            yield return new WaitForSeconds(transitionTime);
            GameManager.Instance?.NewGame();

            // Initialize new game state
            //InitializeNewGame();
            
            // Load the first level
            //SceneTransitionManager.Instance?.LoadScene(firstLevelScene);
        }
        
        /*private void InitializeNewGame()
        {
            // Reset player prefs for new game
            GameManager.Instance?.SetNewGamePrefs();
            SceneTransitionManager.Instance?.LoadScene(firstLevelScene);
            
            // Initialize inventory manager for new game
            InventoryManager.Instance?.InitializeNewGame();
        }*/
        
        private IEnumerator ContinueGameRoutine()
        {           
            yield return new WaitForSeconds(transitionTime);

            //GameManager.Instance?.ContinueGame();

            savedScene = PlayerPrefs.GetInt("LastScene", firstLevelScene);
            //SceneTransitionManager.Instance?.LoadScene(savedScene);

            GameManager.Instance?.ContinueGame();
        }

        #endregion
        
        #region Input System Callbacks
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
                // Handled by Unity's UI Navigation
        }
        
        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
                // Handled by Unity's UI Navigation
        }
        
        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            // Handle cancel based on current panel
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                // Close options panel
                OnOptions();
            }
            else if (startPanel != null && startPanel.activeSelf)
            {
                // Close start panel
                OnStartPanel();
            }
            else if (continuePanel != null && continuePanel.activeSelf)
            {
                // Close continue panel
                OnContinuePanel();
            }
        }
        
        #endregion

        private void OnDestroy()
        {
            // Clean up event listeners
            if (volumeSlider != null)
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
        }
    }
}