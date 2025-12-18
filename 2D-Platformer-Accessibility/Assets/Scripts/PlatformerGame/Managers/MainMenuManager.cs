using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using PixeLadder.EasyTransition;
using PixeLadder.EasyTransition.Effects;

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
        [SerializeField] private string firstLevelScene = "Tutorial";
        [SerializeField] private string saveSceneName = "SavedScene";
        
        [Header("Transition Settings")]
        [SerializeField] private SceneTransitioner sceneTransitioner;
        [SerializeField] private TransitionEffect transitionEffect;
        [SerializeField] private float transitionTime = 1f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource menuAudioSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip startGameSound;
        
        private bool hasSaveData = false;
        private static int currentSceneIndex = 1;
    

        private void Start()
        {
            // Initialize UI buttons
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
                hasSaveData = CheckForSaveData();
                continueButton.interactable = hasSaveData;
            }
                
            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptions);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitGame);
            
            // Initialize options panel
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            
            // Load saved settings
            LoadSettings();
            
            // Set cursor visible
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            Debug.Log("Main Menu initialized");
        }

        private bool CheckForSaveData()
        {
            // Check for player prefs or save file
            return PlayerPrefs.HasKey("LastScene") || 
                   PlayerPrefs.HasKey("PlayerPositionX") ||
                   PlayerPrefs.GetInt("HasSaveData", 0) == 1;
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

        public void OnStartPanel()
        {
            PlayButtonSound(buttonClickSound);
            
            if (startPanel != null)
            {
                bool isActive = startPanel.activeSelf;
                startPanel.SetActive(!isActive);
                
                // Toggle other buttons
                //if (startButton != null) startButton.interactable = isActive;
                //if (continueButton != null) continueButton.interactable = isActive && hasSaveData;
                if (continuePanelButton != null) continuePanelButton.interactable = isActive;
                if (optionsButton != null) optionsButton.interactable = isActive;
                if (quitButton != null) quitButton.interactable = isActive;
            }
        }
        
        public void OnStartGame()
        {
            PlayButtonSound(startGameSound);
            
            // Clear any existing save data for a fresh start
            ClearSaveData();
            
            // Start transition
            StartCoroutine(StartGameRoutine());
        }
        
        private IEnumerator StartGameRoutine()
        {
            // Wait for transition
            yield return new WaitForSeconds(transitionTime);
            
            // Load the first level
            //SceneManager.LoadScene(firstLevelScene);
            sceneTransitioner.LoadScene(firstLevelScene, transitionEffect);
            
            // Initialize new game state
            InitializeNewGame();
        }
        
        private void InitializeNewGame()
        {
            // Reset player prefs for new game
            PlayerPrefs.SetInt("IsNewGame", 1);
            PlayerPrefs.SetString("LastScene", firstLevelScene);
            PlayerPrefs.Save();
            
            // Initialize inventory manager for new game
            InventoryManager.Instance?.InitializeNewGame();
        }

        public void OnContinuePanel()
        {
            PlayButtonSound(buttonClickSound);
            
            if (continuePanel != null)
            {
                bool isActive = continuePanel.activeSelf;
                continuePanel.SetActive(!isActive);
                
                // Toggle other buttons
                //if (startButton != null) startButton.interactable = isActive;
                //if (continueButton != null) continueButton.interactable = isActive && hasSaveData;
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
        
        private IEnumerator ContinueGameRoutine()
        {           
            yield return new WaitForSeconds(transitionTime);
            
            // Load the saved scene
            string savedScene = PlayerPrefs.GetString("LastScene", firstLevelScene);
            //SceneManager.LoadScene(savedScene);
            sceneTransitioner.LoadScene(savedScene, transitionEffect);
            
            // Mark as continuing, not new game
            PlayerPrefs.SetInt("IsNewGame", 0);
            PlayerPrefs.Save();
        }

        
        
        public void OnOptions()
        {
            PlayButtonSound(buttonClickSound);
            
            if (optionsPanel != null)
            {
                bool isActive = optionsPanel.activeSelf;
                optionsPanel.SetActive(!isActive);
                
                // Toggle other buttons
                //if (startButton != null) startButton.interactable = isActive;
                //if (continueButton != null) continueButton.interactable = isActive && hasSaveData;
                if (startPanelButton != null) startPanelButton.interactable = isActive;
                if (continuePanelButton != null) continuePanelButton.interactable = isActive;
                if (quitButton != null) quitButton.interactable = isActive;
            }
        }
        
        public void OnQuitGame()
        {
            PlayButtonSound(buttonClickSound);
            
            #if UNITY_EDITOR
            Debug.Log("Quit game (would quit in build)");
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
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
        
        private void PlayButtonSound(AudioClip clip)
        {
            if (menuAudioSource != null && clip != null)
            {
                menuAudioSource.PlayOneShot(clip);
            }
        }
        
        private void ClearSaveData()
        {
            PlayerPrefs.DeleteKey("PlayerPositionX");
            PlayerPrefs.DeleteKey("PlayerPositionY");
            PlayerPrefs.DeleteKey("PlayerHealth");
            PlayerPrefs.DeleteKey("LastScene");
            PlayerPrefs.SetInt("HasSaveData", 0);
            PlayerPrefs.Save();
            
            // Clear inventory save
            InventoryManager.Instance?.ClearSavedInventory();
        }
        
        // For pause menu access
        public void ReturnToMainMenu()
        {
            StartCoroutine(ReturnToMenuRoutine());
        }
        
        private IEnumerator ReturnToMenuRoutine()
        {
            // Save game before returning
            SaveGameState();
            
            yield return new WaitForSeconds(transitionTime);
            
            // Load main menu scene
            //SceneManager.LoadScene("MainMenu");
            sceneTransitioner.LoadScene("MainMenu", transitionEffect);
        }
        
        private void SaveGameState()
        {
            // Save current scene
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt("HasSaveData", 1);
            
            // Save inventory
            InventoryManager.Instance?.SaveInventory();
            
            PlayerPrefs.Save();
        }
        
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