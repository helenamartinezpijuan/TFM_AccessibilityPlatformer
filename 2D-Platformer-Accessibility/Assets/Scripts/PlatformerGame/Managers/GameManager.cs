using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using PlatformerGame.Player;
using PlatformerGame.Inventory;

namespace PlatformerGame.Managers
{
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private int firstLevelScene = 1;
    private bool isLoadingScene = false;

    // Static reference for easy access
    private static int currentSceneIndex = 1;
    public static int CurrentSceneIndex => currentSceneIndex;

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

        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ContinueGame()
    {
        // Load the saved scene
        int savedScene = PlayerPrefs.GetInt("LastScene", firstLevelScene);

        // Mark as continuing, not new game
        PlayerPrefs.SetInt("IsNewGame", 0);
        PlayerPrefs.Save();

        // Load the scene
        LoadScene(savedScene);
    }

    public void NewGame()
    {
        // Clear all saved data
        ClearSaveData();
        
        // Mark as new game
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.SetInt("LastScene", firstLevelScene);
        PlayerPrefs.Save();
        
        Debug.Log("Starting new game");
        
        // Load first level
        LoadScene(firstLevelScene);
    }

    #region Pause Menu Logic

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMenuRoutine());
    }
    
    private IEnumerator ReturnToMenuRoutine()
    {
        if (isLoadingScene) yield break;
        isLoadingScene = true;

        // Save game before returning
        SaveGameState();
        
        yield return new WaitForSeconds(0.5f);
        
        // Load main menu scene
        LoadScene(0);

        isLoadingScene = false;
    }
    #endregion

    #region Scene Loading

    public void LoadScene(int sceneIndex)
    {
        if (isLoadingScene) return;
        
        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }
    
    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        if (isLoadingScene) yield break;
        isLoadingScene = true;
        
        Debug.Log($"Loading scene {sceneIndex}");
        
        // Save game state before transition
        SaveGameState();
        
        // Wait for transition effect
        yield return new WaitForSeconds(0.1f);
        
        // Load the scene
        SceneManager.LoadScene(sceneIndex);
        
        isLoadingScene = false;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name} (Index: {scene.buildIndex})");
        
        // Save the current scene index
        currentSceneIndex = scene.buildIndex;
        
        // If this is NOT the main menu, set it as the last scene
        if (scene.buildIndex != 0) // Assuming 0 is main menu
        {
            PlayerPrefs.SetInt("LastScene", scene.buildIndex);
            PlayerPrefs.Save();
        }
        
        // Initialize inventory for the new scene
        InitializeSceneInventory(scene.buildIndex);
    }
    
    private void InitializeSceneInventory(int sceneIndex)
    {
        // Don't initialize inventory in main menu
        if (sceneIndex == 0) return;
        
        // Find player in the new scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // The PlayerInventory will load from saved data in its Awake()
                Debug.Log("Player inventory initialized in new scene");
            }
            
            // If this is a new game, clear inventory
            bool isNewGame = PlayerPrefs.GetInt("IsNewGame", 1) == 1;
            if (isNewGame && sceneIndex == firstLevelScene)
            {
                Debug.Log("New game - inventory should be empty");
                // Inventory will be empty by default
            }
        }
    }
    #endregion

    #region Player Prefs Configuration

    public bool CheckForSaveData()
    {
        // Check for player prefs or save file
        return PlayerPrefs.HasKey("LastScene") && PlayerPrefs.GetInt("HasSaveData", 0) == 1;
    }
    
    public void ClearSaveData()
    {
        Debug.Log("Clearing all save data");
        
        // Clear player position
        PlayerPrefs.DeleteKey("PlayerPositionX");
        PlayerPrefs.DeleteKey("PlayerPositionY");
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.SetInt("HasSaveData", 0);
        
        // Clear inventory save
        InventoryManager.Instance?.ClearCurrentSave();
        
        // Mark as new game
        PlayerPrefs.SetInt("IsNewGame", 1);
        
        PlayerPrefs.Save();
    }
    
    public void SaveGameState()
    {
        Debug.Log("Saving game state");
        
        // Save current scene if not main menu
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
        }
        
        PlayerPrefs.SetInt("HasSaveData", 1);
        
        // Save inventory through InventoryManager
        SaveCurrentInventory();
        
        // Save player position and health
        SavePlayerState();
        
        PlayerPrefs.Save();
    }
    
    private void SaveCurrentInventory()
    {
        // Find current player and save their inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && InventoryManager.Instance != null)
            {
                // This will save to PlayerPrefs
                InventoryManager.Instance.SaveCurrentInventory(inventory);
            }
        }
    }
    
    private void SavePlayerState()
    {
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
    }
    #endregion

    #region End Game

    public void Defeat()
    {
        Debug.Log("Player defeated");
        LoadScene(4); // Defeat Scene
    }

    public void Victory()
    {
        Debug.Log("Victory!");
        LoadScene(5); // Victory Scene
    }
    
    public void QuitGame()
    {
        Debug.Log("Saving and quitting game");
        
        // Save before quitting
        SaveGameState();
        
        #if UNITY_EDITOR
        Debug.Log("Quit game (would quit in build)");
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endregion
}
}
