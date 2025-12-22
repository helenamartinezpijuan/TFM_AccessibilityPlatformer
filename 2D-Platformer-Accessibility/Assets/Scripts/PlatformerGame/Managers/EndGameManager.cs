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
    [SerializeField] private SceneTransitioner sceneTransitioner;
    [SerializeField] private TransitionEffect transitionEffect;
    [SerializeField] private float transitionTime = 1f;
    private AudioSource buttonAudioSource;

    private void Start()
    {
        buttonAudioSource = GetComponent<AudioSource>();
    }

    public void OnRestart()
    {
        buttonAudioSource.Play();
        
        StartCoroutine(ReturnToMenuRoutine());
    }
    
    private IEnumerator ReturnToMenuRoutine()
    {
        // Clear game data
        ClearSaveData();
        
        yield return new WaitForSeconds(transitionTime);
        
        // Load main menu scene
        sceneTransitioner.LoadScene("MainMenu", transitionEffect);
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
