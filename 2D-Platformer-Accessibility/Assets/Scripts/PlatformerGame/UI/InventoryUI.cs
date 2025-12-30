using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace PlatformerGame.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform inventorySlotsParent;
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        [Header("Settings")]
        [SerializeField] private Color normalSlotColor = Color.white;
        //[SerializeField] private Color selectedSlotColor = Color.yellow;
        //[SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        
        private PlayerInventory playerInventory;
        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
        private List<GameObject> existingSlotObjects = new List<GameObject>();
        private int currentSelectedIndex = -1;
        
        private void Awake()
        {
            // Clear any existing slots
            //ClearAllSlots();
            
            // Try to find player inventory
            FindAndConnectToInventory();
        }
        
        private void Start()
        {
            // Initialize UI after everything is loaded
            InitializeUI();
        }
        
        #region Initialization
        public void Initialize(PlayerInventory inventory)
        {
            if (inventory == null)
            {
                Debug.LogError("InventoryUI: Received null inventory!");
                return;
            }
            
            playerInventory = inventory;
            Debug.Log("InventoryUI: Connected to player inventory");
            
            // Subscribe to events
            SubscribeToInventoryEvents();
            
            // Initialize UI
            InitializeUI();
        }
        
        private void FindAndConnectToInventory()
        {
            // Method 1: Use singleton instance
            if (PlayerInventory.Instance != null)
            {
                Initialize(PlayerInventory.Instance);
                return;
            }
            
            // Method 2: Find in scene
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    Initialize(inventory);
                    return;
                }
            }
            
            // Method 3: Find any inventory in scene
            PlayerInventory[] allInventories = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);
            if (allInventories.Length > 0)
            {
                Initialize(allInventories[0]);
                return;
            }
            
            Debug.LogWarning("InventoryUI: No inventory found. Will try to connect later.");
            
            // Try again after a delay
            Invoke(nameof(FindAndConnectToInventory), 1f);
        }
        #endregion

        #region UI Handler
        
        private void InitializeUI()
        {
            if (playerInventory == null)
            {
                Debug.LogError("InventoryUI: Cannot initialize without inventory reference");
                return;
            }
            
            // Clear existing slots
            ClearAllSlots();
            slotUIs.Clear();
            existingSlotObjects.Clear();
            
            /*if (inventorySlotsParent != null)
            {
                InitializeExistingSlots();
            }*/
            
            // Create slots based on inventory size
            for (int i = 0; i < playerInventory.InventorySize; i++)
            {
                CreateSlot(i);
            }
            
            // Set initial selection
            if (playerInventory.CurrentSelectedPosition >= 0 && 
                playerInventory.CurrentSelectedPosition < slotUIs.Count)
            {
                UpdateSelection(playerInventory.CurrentSelectedPosition);
            }
            
            Debug.Log($"InventoryUI: Initialized with {slotUIs.Count} slots");
        }

        private void InitializeExistingSlots()
        {
            for (int i = 0; i < inventorySlotsParent.childCount; i++)
            {
                Transform child = inventorySlotsParent.GetChild(i);
                GameObject slotObject = child.gameObject;
                
                // Skip if already added
                if (existingSlotObjects.Contains(slotObject)) continue;
                
                existingSlotObjects.Add(slotObject);
                
                // Get or add InventorySlotUI component
                InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
                if (slotUI == null)
                {
                    // Try to find nested UI references automatically
                    slotUI = AutoConfigureSlot(slotObject, i);
                }
                
                if (slotUI != null)
                {
                    slotUIs.Add(slotUI);
                }
            }
        }
        #endregion

        #region Slot Configuration

        private InventorySlotUI AutoConfigureSlot(GameObject slotObject, int index)
        {
            // Get/Add the InventorySlotUI component
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();//AddComponent<InventorySlotUI>();
            
            // Look for Button component
            Button button = slotObject.GetComponentInChildren<Button>();
            if (button != null)
            {
                // Connect button click
                button.onClick.AddListener(() => OnSlotClicked(index));
            }
            
            // Look for Image components
            Image[] allImages = slotObject.GetComponentsInChildren<Image>();
            
            // Try to identify which image is which based on hierarchy or naming
            foreach (Image img in allImages)
            {
                // Background image (usually the parent image)
                /*if (img.transform.parent == slotObject.transform && slotUI.backgroundImage == null)
                {
                    slotUI.backgroundImage = img;
                }
                // Item icon (often deeper in hierarchy)
                else */if (img.name.ToLower().Contains("icon") || 
                        img.transform.parent != null && 
                        img.transform.parent.name.ToLower().Contains("icon"))
                {
                    slotUI.itemIcon = img;
                }
            }
            
            // Look for TextMeshProUGUI components
            /*TextMeshProUGUI[] allTexts = slotObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in allTexts)
            {
                // Item count text
                if (text.name.ToLower().Contains("count") || text.name.ToLower().Contains("number"))
                {
                    slotUI.itemCountText = text;
                }
                // Slot number text
                else if (text.name.ToLower().Contains("slot") || text.text == (index + 1).ToString())
                {
                    slotUI.slotNumberText = text;
                }
            }*/
            
            return slotUI;
        }
        
        private void CreateSlot(int index)
        {
            if (inventorySlotPrefab == null || inventorySlotsParent == null)
            {
                Debug.LogError("InventoryUI: Missing slot prefab or parent!");
                return;
            }
            
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventorySlotsParent);
            slotObject.name = $"InventorySlot_{index}";
            
            // Get or add InventorySlotUI component
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI == null)
            {
                slotUI = slotObject.AddComponent<InventorySlotUI>();
            }
            
            // Initialize slot
            Item item = playerInventory?.GetItem(index);
            slotUI.Initialize(index, item);
            
            slotUIs.Add(slotUI);
        }
        
        private void ClearAllSlots()
        {
            foreach (Transform child in inventorySlotsParent)
            {
                Destroy(child.gameObject);
            }
            slotUIs.Clear();
        }

        public void RefreshAllSlots()
        {
            if (playerInventory == null) return;

            if (slotUIs.Count != playerInventory.InventorySize)
            {
                InitializeUI();
            }
            
            for (int i = 0; i < slotUIs.Count; i++)
            {
                if (i < playerInventory.InventorySize)
                {
                    Item item = playerInventory.GetItem(i);
                    if (slotUIs[i] != null)
                    {
                        slotUIs[i].UpdateSlot(item);
                    }
                }
            }
        }
        #endregion

        #region Even Handlers

        private void SubscribeToInventoryEvents()
        {
            if (playerInventory == null) return;
            
            playerInventory.OnItemAdded += OnItemAdded;
            playerInventory.OnItemRemoved += OnItemRemoved;
            playerInventory.OnSelectionChanged += OnSelectionChanged;
            playerInventory.OnInventoryToggle += OnInventoryToggle;
        }
        
        private void UnsubscribeFromInventoryEvents()
        {
            if (playerInventory == null) return;
            
            playerInventory.OnItemAdded -= OnItemAdded;
            playerInventory.OnItemRemoved -= OnItemRemoved;
            playerInventory.OnSelectionChanged -= OnSelectionChanged;
            playerInventory.OnInventoryToggle -= OnInventoryToggle;
        }

        private void OnSlotClicked(int slotIndex)
        {
            // Forward click to the InventorySlotUI
            if (slotIndex >= 0 && slotIndex < slotUIs.Count && slotUIs[slotIndex] != null)
            {
                slotUIs[slotIndex].OnSlotClicked();
            }
        }
        
        private void OnItemAdded(Item item)
        {
            if (item == null) return;
            
            int position = item.inventoryPosition;
            if (position >= 0 && position < slotUIs.Count)
            {
                slotUIs[position].UpdateSlot(item);
                Debug.Log($"InventoryUI: Updated slot {position} with {item.itemName}");
            }
        }
        
        private void OnItemRemoved(Item item)
        {
            if (item == null) return;
            
            int position = item.inventoryPosition;
            if (position >= 0 && position < slotUIs.Count)
            {
                slotUIs[position].UpdateSlot(null);
            }
        }
        
        private void OnSelectionChanged(int newPosition)
        {
            UpdateSelection(newPosition);
        }
        
        private void OnInventoryToggle(bool isOpen)
        {
            gameObject.SetActive(isOpen);
            
            if (isOpen)
            {
                RefreshAllSlots();
                if (playerInventory != null)
                {
                    UpdateSelection(playerInventory.CurrentSelectedPosition);
                }
            }
        }
        #endregion
        
        #region Update UI Selection
        private void UpdateSelection(int newPosition)
        {
            // Deselect previous slot
            if (currentSelectedIndex >= 0 && currentSelectedIndex < slotUIs.Count)
            {
                slotUIs[currentSelectedIndex].SetSelected(false, normalSlotColor);
            }
            
            // Select new slot
            if (newPosition >= 0 && newPosition < slotUIs.Count)
            {
                //slotUIs[newPosition].SetSelected(true, selectedSlotColor);
                currentSelectedIndex = newPosition;
                
                // Update item info display
                UpdateItemInfoDisplay(newPosition);
                
                // Update highlight position
                /*if (selectedSlotHighlight != null)
                {
                    selectedSlotHighlight.transform.SetParent(slotUIs[newPosition].transform);
                    selectedSlotHighlight.rectTransform.anchoredPosition = Vector2.zero;
                    selectedSlotHighlight.rectTransform.sizeDelta = Vector2.zero;
                }*/
            }
        }

        private void UpdateItemInfoDisplay(int slotIndex)
        {
            if (playerInventory == null) return;
            
            Item item = playerInventory.GetItem(slotIndex);

            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
            }
            
            if (itemNameText != null)
            {
                itemNameText.text = item != null ? item.itemName : "";
            }
            
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = item != null ? item.description : "";
            }
        }
        #endregion

        #region Clean up
        private void OnDestroy()
        {
            UnsubscribeFromInventoryEvents();
        }

        #endregion        
    }
}