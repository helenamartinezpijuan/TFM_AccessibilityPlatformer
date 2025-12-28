using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerGame.UI
{
public class TextPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI panelTitleText;        // Title - corresponds to current level
    [SerializeField] private TextMeshProUGUI coreText;              // Message - text displayed to the player
    [SerializeField] private Image itemIcon;                        // Item icon - active on collection
    [SerializeField] private GameObject backgroundPanel;            // Panel - set active/inactive based on player collision

    [Header("Item References")]
    [SerializeField] private GameObject flashlight;
    [SerializeField] private GameObject stickerBag;
    [SerializeField] private GameObject sunglasses;
    [SerializeField] private Sprite[] iconList;
    [SerializeField] private GameObject itemIconObject;
    [SerializeField] private GameObject coreTextObject;
    
    [Header("Text Settings")]
    [SerializeField] private TextAsset textDataFile;                // JSON text data with Title and Message values
    [SerializeField] private float charactersPerSecond = 20f;       // Reading speed
    [SerializeField] private float minDisplayTime = 2f;             // Minimum seconds per message
    [SerializeField] private float panelOffset = 100f;              // Offset from player position
    
    [Header("Animation")]
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string showTrigger = "Show";
    //[SerializeField] private string hideTrigger = "Hide";

    private AudioSource audioSource;
    
    // Text data storage
    private Dictionary<string, string[]> textDatabase = new Dictionary<string, string[]>();
    private Queue<string> currentMessageQueue = new Queue<string>();
    private Coroutine displayCoroutine;
    
    // State
    private bool isShowing = false;
    private bool isPaused = false;
    private Transform playerTransform;
    private Transform currentTextPoint;
    
    private void Awake()
    {
        // Hide panel initially
        backgroundPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        
        // Load text data if provided
        if (textDataFile != null)
        {
            LoadTextData();
        }
    }
    
    private void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    private void Update()
    {
        // Update panel position to follow player with offset
        if (isShowing && playerTransform != null)
        {
            UpdatePanelPosition();
        }
    }
    
    private void UpdatePanelPosition()
    {
        if (playerTransform == null) return;
        
        // Calculate screen position with offset
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        
        // Position panel to avoid obstruction (right side of player)
        Vector3 panelPosition = playerScreenPos;
        panelPosition.x += panelOffset;
        panelPosition.y += panelOffset;
        
        // Clamp to screen bounds
        RectTransform rectTransform = GetComponent<RectTransform>();
        float halfWidth = rectTransform.rect.width / 2;
        float halfHeight = rectTransform.rect.height / 2;
        
        panelPosition.x = Mathf.Clamp(panelPosition.x, halfWidth, Screen.width - halfWidth);
        panelPosition.y = Mathf.Clamp(panelPosition.y, halfHeight, Screen.height - halfHeight);
        
        transform.position = panelPosition;
    }
    
    private void LoadTextData()
    {
        // Parse JSON text data
        // Format: {"messages": [{"category":"Tutorial","texts":["Line1","Line2"]}]}
        try
        {
            TextDataWrapper data = JsonUtility.FromJson<TextDataWrapper>(textDataFile.text);
            foreach (var message in data.messages)
            {
                textDatabase[message.category] = message.texts;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load text data: {e.Message}");
        }
    }
    
    // Call this from your trigger zones
    public void ShowText(string category, string[] messages)
    {
        if (isShowing) return;
        
        // Set title (category name)
        panelTitleText.text = category;
        
        // Queue messages
        currentMessageQueue.Clear();
        foreach (string message in messages)
        {
            currentMessageQueue.Enqueue(message);
        }
        
        // Start displaying
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        backgroundPanel.SetActive(true);
        displayCoroutine = StartCoroutine(DisplayMessages());
    }
    
    // Alternative: Show text from database
    public void ShowTextFromDatabase(string category)
    {
        if (textDatabase.ContainsKey(category))
        {
            ShowText(category, textDatabase[category]);
        }
        else
        {
            Debug.LogWarning($"Text category not found: {category}");
        }
    }
    
    // Direct text input
    public void ShowDirectText(string title, string message)
    {
        string[] messages = message.Split(new string[] { "\\n" }, StringSplitOptions.None);
        ShowText(title, messages);
    }
    
    private IEnumerator DisplayMessages()
    {
        isShowing = true;
        
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(showTrigger);
            yield return new WaitForSeconds(0f); // Animation time
        }
        
        // Display each message
        while (currentMessageQueue.Count > 0)
        {
            string message = currentMessageQueue.Dequeue();
            
            // Calculate display time based on text length
            float displayTime = Mathf.Max(
                message.Length / charactersPerSecond,
                minDisplayTime
            );
            
            // Set text
            coreText.text = message;
            
            // Wait for reading time
            yield return new WaitForSeconds(displayTime);
            
            // If there are more messages, wait a moment between them
            if (currentMessageQueue.Count > 0)
            {
                coreText.text = "...";
                yield return new WaitForSeconds(0.4f);
            }
        }
        
        // Hide panel after all messages
        HidePanel();
    }
    
    public void HidePanel()
    {
        if (!isShowing) return;
        
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }
        
        backgroundPanel.SetActive(false);
        
        isShowing = false;
        currentMessageQueue.Clear();
    }
    
    private IEnumerator DeactivateAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        backgroundPanel.SetActive(false);
    }
    
    public void SendHelp(string itemName)
    {
        if (itemName == "Flashlight")
        {
            itemIcon.sprite = iconList[0];
            flashlight.SetActive(true);
        }  
        else if (itemName == "StickerBag")
        {
            itemIcon.sprite = iconList[1];
            stickerBag.SetActive(true);
        }  
        else if (itemName == "Sunglasses")
        {
            itemIcon.sprite = iconList[2];
            sunglasses.SetActive(true);
        }

        DisplayItem();
    }

    private IEnumerator DisplayItem()
    {
        backgroundPanel.SetActive(true);
        coreTextObject.SetActive(false);
        itemIconObject.SetActive(true);

        audioSource.Play();

        yield return new WaitForSeconds(2f);

        coreTextObject.SetActive(true);
        itemIconObject.SetActive(false);
        backgroundPanel.SetActive(false);
    }
    
    // Data classes for JSON parsing
    [Serializable]
    private class TextDataWrapper
    {
        public TextMessage[] messages;
    }
    
    [Serializable]
    private class TextMessage
    {
        public string category;
        public string[] texts;
    }
}
}