using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using PlatformerGame.UI;
using PlatformerGame.Inventory.Items;
using PlatformerGame.Managers;

namespace PlatformerGame.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int inventoryScene = 6;
        [SerializeField] private int inventorySize = 4;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private ItemDatabase itemDatabase;

        private int currentSelectedPosition = 0;
        [SerializeField] private List<Item> items = new List<Item>();
        private bool isOpen = false;
        private bool inventorySceneLoaded = false;
        
        // Static reference for easy access
        public static PlayerInventory Instance { get; private set; }

        // Events
        public Action<Item> OnItemAdded;
        public Action<Item> OnItemRemoved;
        public Action<bool> OnInventoryToggle;
        public Action<int> OnSelectionChanged;

        // Properties
        public bool IsOpen => isOpen;
        public int CurrentSelectedPosition => currentSelectedPosition;
        public List<Item> Items => new List<Item>(items);
        public int InventorySize => inventorySize;

        private void Awake()
        {
            // Initialize empty inventory slots
            items.Clear();
            for (int i = 0; i < inventorySize; i++)
            {
                items.Add(null);
            }
            
            if (debugMode) Debug.Log($"Inventory initialized with {inventorySize} slots");
        
            // Load inventory from save data
            LoadInventoryFromSave();
        }

        private void Start()
        {
            // Find any existing InventoryUI and connect it
            ConnectToInventoryUI();
        }

        #region Load Inventory Between Scenes

        private void LoadInventoryFromSave()
        {
            if (InventoryManager.Instance != null)
            {
                // Get saved item names from InventoryManager
                List<string> savedItemNames = InventoryManager.Instance.GetSavedItemNames();
                
                if (savedItemNames.Count > 0)
                {
                    // Clear current inventory
                    for (int i = 0; i < inventorySize; i++)
                    {
                        items[i] = null;
                    }
                    
                    // Load items
                    foreach (string itemName in savedItemNames)
                    {
                        if (!string.IsNullOrEmpty(itemName))
                        {
                            // Get item from database
                            Item item = GetItemFromDatabase(itemName);
                            if (item != null)
                            {
                                AddItem(item, true); // Silent add (no UI refresh)
                            }
                        }
                    }
                    
                    if (debugMode) Debug.Log($"Loaded {savedItemNames.Count} items from save");
                    
                    // Refresh UI
                    RefreshInventoryUI();
                }
            }
        }

        private Item GetItemFromDatabase(string itemName)
        {
            if (itemDatabase != null)
            {
                return itemDatabase.GetItemByName(itemName);
            }
            
            // Fallback: Try to find item in resources
            Item[] allItems = Resources.FindObjectsOfTypeAll<Item>();
            foreach (Item item in allItems)
            {
                if (item.itemName == itemName)
                {
                    return Instantiate(item); // Create instance to avoid sharing
                }
            }
            
            Debug.LogWarning($"Item '{itemName}' not found!");
            return null;
        }
        #endregion

        #region Update Inventory Items

        public Item GetItem(int position)
        {
            if (position >= 0 && position < items.Count)
            {
                return items[position];
            }
            return null;
        }

        public bool AddItem(Item newItem, bool silent = false)
        {
            if (newItem == null)
            {
                Debug.LogError("Tried to add null item to inventory");
                return false;
            }

            // Find first empty slot
            for (int i = 0; i < inventorySize; i++)
            {
                if (items[i] == null)
                {
                    // Create instance to avoid sharing ScriptableObject references
                    Item itemInstance = Instantiate(newItem);
                    itemInstance.itemName = newItem.itemName;
                    itemInstance.description = newItem.description;
                    itemInstance.icon = newItem.icon;
                    
                    items[i] = itemInstance;
                    itemInstance.inventoryPosition = i;
                    
                    // Notify item was added
                    itemInstance.OnAddToInventory(this);
                    
                    if (debugMode && !silent) Debug.Log($"Added {itemInstance.itemName} to slot {i}");
                    
                    // Trigger event
                    if (!silent) OnItemAdded?.Invoke(itemInstance);
                    
                    // Save inventory
                    if (!silent && InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.SaveCurrentInventory(this);
                    }
                    
                    // Force refresh UI
                    if (!silent) RefreshInventoryUI();
                    
                    return true;
                }
            }
            
            if (debugMode && !silent) Debug.Log("Inventory full, cannot add item.");
            return false;
        }

        // Overload for regular use
        public bool AddItem(Item newItem)
        {
            return AddItem(newItem, false);
        }

        public bool RemoveItem(int position, bool silent = false)
        {
            if (position >= 0 && position < inventorySize && items[position] != null)
            {
                Item removedItem = items[position];
                items[position] = null;
                
                // Notify item was removed
                removedItem.OnRemoveFromInventory(this);
                
                if (!silent) OnItemRemoved?.Invoke(removedItem);
                
                // Save inventory
                if (!silent && InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.SaveCurrentInventory(this);
                }
                
                // Force refresh UI
                if (!silent) RefreshInventoryUI();
                
                return true;
            }
            return false;
        }
        
        // Overload for regular use
        public bool RemoveItem(int position)
        {
            return RemoveItem(position, false);
        }

        // Save current inventory to InventoryManager
        public void SaveInventory()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SaveCurrentInventory(this);
            }
        }
        #endregion

        #region UI logic

        // Public method to open inventory
        public void OpenInventory()
        {
            if (isOpen) return;
            
            isOpen = true;
            if (debugMode) Debug.Log("Inventory opened");

            // Load additive scene for inventory UI
            SceneManager.LoadScene(inventoryScene, LoadSceneMode.Additive);
            inventorySceneLoaded = true;

            OnInventoryToggle?.Invoke(true);
            Time.timeScale = 0f; // Pause game
            
            // Force refresh UI after a small delay to ensure scene is loaded
            Invoke(nameof(RefreshInventoryUI), 0.1f);
        }

        // Public method to close inventory
        public void CloseInventory()
        {
            if (!isOpen) return;
            
            isOpen = false;
            if (debugMode) Debug.Log("Inventory closed");

            // Unload inventory scene
            if (inventorySceneLoaded)
            {
                SceneManager.UnloadSceneAsync(inventoryScene);
                inventorySceneLoaded = false;
            }

            OnInventoryToggle?.Invoke(false);
            Time.timeScale = 1f; // Resume game
        }

        private void NavigateSelection(Vector2 direction)
        {
            if (!isOpen) return;

            int newPosition = currentSelectedPosition;

            // Simple 2x4 grid navigation
            if (direction.y > 0.5f) // Up
            {
                newPosition = Mathf.Clamp(newPosition - 4, 0, inventorySize - 1);
            }
            else if (direction.y < -0.5f) // Down
            {
                newPosition = Mathf.Clamp(newPosition + 4, 0, inventorySize - 1);
            }
            else if (direction.x > 0.5f) // Right
            {
                newPosition = Mathf.Clamp(newPosition + 1, 0, inventorySize - 1);
            }
            else if (direction.x < -0.5f) // Left
            {
                newPosition = Mathf.Clamp(newPosition - 1, 0, inventorySize - 1);
            }

            if (newPosition != currentSelectedPosition)
            {
                currentSelectedPosition = newPosition;
                OnSelectionChanged?.Invoke(currentSelectedPosition);
                if (debugMode) Debug.Log($"Selected slot: {currentSelectedPosition}");
            }
        }

        public void UseSelectedItem()
        {
            if (!isOpen) return;

            Item selectedItem = GetItem(currentSelectedPosition);
            if (selectedItem != null)
            {                
                if (selectedItem.CanUse(this))
                {
                    selectedItem.Use(this);
                }
            }
        }

        // Connect to existing InventoryUI in scene
        private void ConnectToInventoryUI()
        {
            InventoryUI[] allUI = FindObjectsByType<InventoryUI>(FindObjectsSortMode.None);
            foreach (InventoryUI ui in allUI)
            {
                ui.Initialize(this);
            }
        }

        // Force refresh the inventory UI
        public void RefreshInventoryUI()
        {
            if (!isOpen) return;
            
            InventoryUI[] allUI = FindObjectsByType<InventoryUI>(FindObjectsSortMode.None);
            foreach (InventoryUI ui in allUI)
            {
                if (ui != null)
                {
                    ui.RefreshAllSlots();
                }
            }
        }
        #endregion

        public bool HasGloves()
        {
            foreach (var item in items)
            {
                if (item is Gloves gloves)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool HasFlashlight()
        {
            foreach (var item in items)
            {
                if (item is Flashlight flashlight)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool HasSunglasses()
        {
            foreach (var item in items)
            {
                if (item is Sunglasses sunglasses)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasSticker(Sticker sticker)
        {
            foreach (var item in items)
            {
                if (item is StickerBag stickerBag)
                {
                    foreach (var collectedSticker in stickerBag.GetStickers())
                    {
                        if (sticker == collectedSticker)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool HasNumberStickers()
        {
            foreach (var item in items)
            {
                if (item is StickerBag stickerBag)
                {
                    if (stickerBag.GetNumberStickers() != null)
                        return true;
                }
            }
            return false;
        }

        #region Input System Callbacks
        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.performed && isOpen)
            {
                Vector2 input = context.ReadValue<Vector2>();
                NavigateSelection(input);
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isOpen)
                {
                    UseSelectedItem();
                }
                else
                {
                    // Regular game interaction
                    if (debugMode) Debug.Log("Interact action performed");
                }
            }
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isOpen)
                {
                    CloseInventory();
                }
                else
                {
                    OpenInventory();
                }
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed && isOpen)
            {
                CloseInventory();
            }
        }
        #endregion

        private void OnDestroy()
        {
            // Destroy any instantiated items
            foreach (Item item in items)
            {
                if (item != null && !Application.isPlaying)
                {
                    Destroy(item);
                }
            }
        }
    }
}
