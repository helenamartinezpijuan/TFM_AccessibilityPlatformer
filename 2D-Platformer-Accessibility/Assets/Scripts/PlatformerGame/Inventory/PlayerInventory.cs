using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PlatformerGame.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private string inventorySceneName = "InventoryUI";
        [SerializeField] private int inventorySize = 8;
        [SerializeField] private bool debugMode = true;

        private int currentSelectedPosition = 0;
        private List<Item> items = new List<Item>();
        private bool isOpen = false;
        private bool inventorySceneLoaded = false;
        
        // Static reference for easy access
        public static PlayerInventory Instance { get; private set; }

        // Events
        public System.Action<Item> OnItemAdded;
        public System.Action<Item> OnItemRemoved;
        public System.Action<bool> OnInventoryToggle;
        public System.Action<int> OnSelectionChanged;

        // Properties
        public bool IsOpen => isOpen;
        public int CurrentSelectedPosition => currentSelectedPosition;
        public List<Item> Items => new List<Item>(items);
        public int InventorySize => inventorySize;

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
                return;
            }

            // Initialize empty inventory slots
            items.Clear();
            for (int i = 0; i < inventorySize; i++)
            {
                items.Add(null);
            }
            
            if (debugMode) Debug.Log($"Inventory initialized with {inventorySize} slots");
        }

        private void Start()
        {
            // Find any existing InventoryUI and connect it
            ConnectToInventoryUI();
        }

        public Item GetItem(int position)
        {
            if (position >= 0 && position < items.Count)
            {
                return items[position];
            }
            return null;
        }

        public bool AddItem(Item newItem)
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
                    items[i] = newItem;
                    newItem.inventoryPosition = i;
                    
                    // Notify item was added
                    newItem.OnAddToInventory(this);
                    
                    if (debugMode) Debug.Log($"Added {newItem.itemName} to slot {i}");
                    
                    // Trigger event
                    OnItemAdded?.Invoke(newItem);
                    
                    // Force refresh UI
                    RefreshInventoryUI();
                    
                    return true;
                }
            }
            
            if (debugMode) Debug.Log("Inventory full, cannot add item.");
            return false;
        }

        public bool RemoveItem(int position)
        {
            if (position >= 0 && position < inventorySize && items[position] != null)
            {
                Item removedItem = items[position];
                items[position] = null;
                
                // Notify item was removed
                removedItem.OnRemoveFromInventory(this);
                
                OnItemRemoved?.Invoke(removedItem);
                
                // Force refresh UI
                RefreshInventoryUI();
                
                return true;
            }
            return false;
        }

        // Public method to open inventory
        public void OpenInventory()
        {
            if (isOpen) return;
            
            isOpen = true;
            if (debugMode) Debug.Log("Inventory opened");

            // Load additive scene for inventory UI
            if (!string.IsNullOrEmpty(inventorySceneName))
            {
                SceneManager.LoadScene(inventorySceneName, LoadSceneMode.Additive);
                inventorySceneLoaded = true;
            }

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
            if (inventorySceneLoaded && !string.IsNullOrEmpty(inventorySceneName))
            {
                SceneManager.UnloadSceneAsync(inventorySceneName);
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
                    
                    // If item is consumable, remove it after use
                    if (selectedItem.isConsumable)
                    {
                        RemoveItem(currentSelectedPosition);
                    }
                }
            }
        }

        // Connect to existing InventoryUI in scene
        private void ConnectToInventoryUI()
        {
            InventoryUI[] allUI = FindObjectsOfType<InventoryUI>(true);
            foreach (InventoryUI ui in allUI)
            {
                ui.Initialize(this);
            }
        }

        // Force refresh the inventory UI
        public void RefreshInventoryUI()
        {
            if (!isOpen) return;
            
            InventoryUI[] allUI = FindObjectsOfType<InventoryUI>();
            foreach (InventoryUI ui in allUI)
            {
                if (ui != null)
                {
                    ui.RefreshAllSlots();
                }
            }
        }

        // Input System Callbacks
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

        // Clean up when destroyed
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
