using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.UI
{
public class TextTriggerZone : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private string textCategory = "Tutorial";      // Category corresponds to each level
    [SerializeField] private string[] customMessages;               // [Optional] Custom message as an alternative to database
    [SerializeField] private TextPanelController textPanel;         // Reference to text panel logic in the scene
    
    [Header("Trigger Settings")]
    [SerializeField] private bool oneTimeOnly = false;              // Some messages will only appear once

    //[Header("Help Item Settings")]
    //[SerializeField] private bool hasItem = false;
    //[SerializeField] private string itemName = "Flashlight";
    
    private bool hasBeenTriggered = false;
    
    private void Start()
    {
        if (textPanel == null)
        {
            Debug.LogError("No TextPanelController found in scene!");
            enabled = false;
        }
    }

    #region Collision detection

    /// <summary>
    /// Reveals panel when player enters the trigger zone
    /// </summary>
    /// <param name="other">Player collider</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTimeOnly && hasBeenTriggered) return;

        //if (hasItem) textPanel.SendHelp(itemName);
        
        ShowText();
        hasBeenTriggered = true;
    }

    /// <summary>
    /// Hide panel when player exits the trigger zone
    /// </summary>
    /// <param name="other">Player collider</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        textPanel.HidePanel();
    }
    #endregion

    #region UI
    
    /// <summary>
    /// Shows custom message if found. Otherwise shows message from database (json file)
    /// </summary>
    private void ShowText()
    {
        if (textPanel == null) return;
        
        if (customMessages.Length > 0)
        {
            // Use custom messages
            textPanel.ShowText(textCategory, customMessages);
        }
        else
        {
            // Use database
            textPanel.ShowTextFromDatabase(textCategory);
        }
    }
    #endregion

}
}