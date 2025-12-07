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
        [SerializeField] private Image selectedSlotHighlight;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        
        [Header("Settings")]
        [SerializeField] private Color normalSlotColor = Color.white;
        [SerializeField] private Color selectedSlotColor = Color.yellow;
        [SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        
        private PlayerInventory playerInventory;
        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
        private int currentSelectedIndex = -1;
        
        private void Awake()
        {
            // Clear any existing slots
            ClearAllSlots();
            
            // Try to find player inventory
            FindAndConnectToInventory();
        }
        
        private void Start()
        {
            // Initialize UI after everything is loaded
            InitializeUI();
        }
        
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
            
            Debug.Log("InventoryUI: No inventory found. Will try to connect later.");
            
            // Try again after a delay
            Invoke(nameof(FindAndConnectToInventory), 0.5f);
        }
        
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
        
        private void InitializeUI()
        {
            if (playerInventory == null)
            {
                Debug.LogError("InventoryUI: Cannot initialize without inventory reference");
                return;
            }
            
            // Clear existing slots
            ClearAllSlots();
            
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
        
        // Event Handlers
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
        
        // UI Update Methods
        public void RefreshAllSlots()
        {
            if (playerInventory == null) return;
            
            for (int i = 0; i < slotUIs.Count; i++)
            {
                Item item = playerInventory.GetItem(i);
                slotUIs[i].UpdateSlot(item);
            }
        }
        
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
                slotUIs[newPosition].SetSelected(true, selectedSlotColor);
                currentSelectedIndex = newPosition;
                
                // Update item info display
                UpdateItemInfoDisplay(newPosition);
                
                // Update highlight position
                if (selectedSlotHighlight != null)
                {
                    selectedSlotHighlight.transform.SetParent(slotUIs[newPosition].transform);
                    selectedSlotHighlight.rectTransform.anchoredPosition = Vector2.zero;
                    selectedSlotHighlight.rectTransform.sizeDelta = Vector2.zero;
                }
            }
        }
        
        private void UpdateItemInfoDisplay(int slotIndex)
        {
            if (playerInventory == null) return;
            
            Item item = playerInventory.GetItem(slotIndex);
            
            if (itemNameText != null)
            {
                itemNameText.text = item != null ? item.itemName : "";
            }
            
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = item != null ? item.description : "";
            }
        }
        
        // Clean up
        private void OnDestroy()
        {
            UnsubscribeFromInventoryEvents();
        }
    }
}