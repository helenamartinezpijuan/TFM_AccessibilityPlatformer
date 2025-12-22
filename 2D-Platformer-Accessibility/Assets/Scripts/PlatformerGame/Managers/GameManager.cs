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
    public static GameManager Instance;

    [Header("Scene Settings")]
    [SerializeField] private string firstLevelScene = "Tutorial";
    [SerializeField] private string saveSceneName = "SavedScene";

    [Header("Transition Settings")]
    [SerializeField] private TransitionEffect transitionEffect;

    public void LoadNewGame()
    {
        SceneTransitioner.Instance?.LoadScene(firstLevelScene, transitionEffect);
    }

    public void LoadMainMenu()
    {
        SceneTransitioner.Instance?.LoadScene("MainMenu", transitionEffect);
    }

    public void ContinueGame()
    {
        // Load the saved scene
        string savedScene = PlayerPrefs.GetString("LastScene", firstLevelScene);

        // Mark as continuing, not new game
        PlayerPrefs.SetInt("IsNewGame", 0);
        PlayerPrefs.Save();

        SceneTransitioner.Instance?.LoadScene(savedScene, transitionEffect);
    }

    public void SetNewGamePrefs()
    {
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.SetString("LastScene", firstLevelScene);
        PlayerPrefs.Save();
    }

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
