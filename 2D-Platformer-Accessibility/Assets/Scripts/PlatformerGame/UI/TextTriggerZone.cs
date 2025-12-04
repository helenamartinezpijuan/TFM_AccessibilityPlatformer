using UnityEngine;

namespace PlatformerGame.UI
{
public class TextTriggerZone : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private string textCategory = "Tutorial";
    [SerializeField] private string[] customMessages; // Leave empty to use database
    [SerializeField] private TextPanelController textPanel;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool oneTimeOnly = false;
    
    private bool hasBeenTriggered = false;
    
    private void Start()
    {
        if (textPanel == null)
        {
            Debug.LogError("No TextPanelController found in scene!");
            enabled = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player entered text trigger zone.");

        if (oneTimeOnly && hasBeenTriggered) return;
        
        ShowText();
        hasBeenTriggered = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Optional: Hide text when player leaves
        textPanel.HidePanel();
    }
    
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

            // Hacer enum para textCategory y así poder elegir desde el Inspector de Unity con un dropdown menu
        }
    }
}
}