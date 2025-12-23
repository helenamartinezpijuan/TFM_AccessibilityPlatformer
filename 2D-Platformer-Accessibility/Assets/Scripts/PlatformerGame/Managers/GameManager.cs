using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using PixeLadder.EasyTransition;
using PixeLadder.EasyTransition.Effects;
using PlatformerGame.Player;
using PlatformerGame.Inventory;

namespace PlatformerGame.Managers
{
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private string firstLevelScene = "Tutorial";

    [Header("Transition Settings")]
    [SerializeField] private float transitionTime = 1f;

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

    public void ContinueGame()
    {
        // Load the saved scene
        string savedScene = PlayerPrefs.GetString("LastScene", firstLevelScene);

        // Mark as continuing, not new game
        PlayerPrefs.SetInt("IsNewGame", 0);
        PlayerPrefs.Save();
    }

    public void SetNewGamePrefs()
    {
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.SetString("LastScene", firstLevelScene);
        PlayerPrefs.Save();
    }

    #region Pause Menu Logic
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
        SceneTransitionManager.Instance?.LoadScene("MainMenu");
    }
    #endregion

    #region Player Prefs Configuration

    public bool CheckForSaveData()
    {
        // Check for player prefs or save file
        return PlayerPrefs.HasKey("LastScene") || PlayerPrefs.GetInt("HasSaveData", 0) == 1;
    }
    public void ClearSaveData()
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
    
    public void SaveGameState()
    {
        // Save current scene
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("HasSaveData", 1);
        
        // Save inventory
        InventoryManager.Instance?.SaveInventory();
        
        PlayerPrefs.Save();
    }

    #endregion
}
}
