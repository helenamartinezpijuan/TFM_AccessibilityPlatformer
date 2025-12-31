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
public class EndGameManager : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float transitionTime = 1f;
    private AudioSource buttonAudioSource;

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
    }

    public void OnRestart()
    {
        buttonAudioSource.Play();
        
        // Clear game data
        ClearSaveData();
        
        // Load main menu scene
        GameManager.Instance?.ReturnToMainMenu();
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
}
}
