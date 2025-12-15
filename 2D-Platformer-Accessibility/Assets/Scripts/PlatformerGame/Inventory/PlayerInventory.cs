using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PlatformerGame.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private string inventorySceneName = "InventoryUI";
        [SerializeField] private int inventorySize = 4;

        private int currentSelectedPosition = 0;
        public List<Item> items = new List<Item>();
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
            
            Debug.Log($"Inventory initialized with {inventorySize} slots");
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
                    
                    Debug.Log($"Added {newItem.itemName} to slot {i}");
                    
                    // Trigger event
                    OnItemAdded?.Invoke(newItem);
                    
                    // Force refresh UI
                    RefreshInventoryUI();
                    
                    return true;
                }
            }
            
            Debug.Log("Inventory full, cannot add item.");
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

        public void OpenInventory()
        {
            Debug.Log($"=== OPEN INVENTORY CALLED ===");
            Debug.Log($"Current isOpen: {isOpen}");
            Debug.Log($"Scene to load: {inventorySceneName}");

            if (isOpen) return;
            
            isOpen = true;
            Debug.Log($"Opening inventory, loading scene: {inventorySceneName}");

            SceneManager.sceneLoaded += OnInventorySceneLoaded;
            StartCoroutine(LoadInventoryScene());
            
            //OnInventoryToggle?.Invoke(true);
            Time.timeScale = 0f;
        }

        private IEnumerator LoadInventoryScene()
        {
            AsyncOperation asyncLoad = null;
            try
            {
                asyncLoad = SceneManager.LoadSceneAsync(inventorySceneName, LoadSceneMode.Additive);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to start loading inventory scene '{inventorySceneName}': {ex.Message}");
            }

            if (asyncLoad == null)
            {
                // Restore state and bail out
                Debug.LogError($"LoadInventoryScene: Async operation is null for scene '{inventorySceneName}'");
                SceneManager.sceneLoaded -= OnInventorySceneLoaded;
                isOpen = false;
                Time.timeScale = 1f;
                yield break;
            }

            float elapsed = 0f;
            float timeout = 5f; // seconds

            while (!asyncLoad.isDone && elapsed < timeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!asyncLoad.isDone)
            {
                Debug.LogError($"Timed out loading inventory scene '{inventorySceneName}' after {timeout} seconds.");
                SceneManager.sceneLoaded -= OnInventorySceneLoaded;
                isOpen = false;
                Time.timeScale = 1f;
                yield break;
            }

            inventorySceneLoaded = true;
            Debug.Log("Inventory scene fully loaded");

            yield return new WaitForEndOfFrame(); // Wait for UI to initialize

            OnInventoryToggle?.Invoke(true);

            RefreshInventoryUI();
        }

        // Public method to close inventory
        public void CloseInventory()
        {
            if (!isOpen) return;
            
            isOpen = false;
            Debug.Log("Inventory closed");

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
                Debug.Log($"Selected slot: {currentSelectedPosition}");
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

        private void OnInventorySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == inventorySceneName)
            {
                inventorySceneLoaded = true;
                SceneManager.sceneLoaded -= OnInventorySceneLoaded;
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
                    Debug.Log("Interact action performed");
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
