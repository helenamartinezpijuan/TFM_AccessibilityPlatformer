using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("Item Parameters")]
        [SerializeField] private string inventorySceneName = "InventoryUI";
        [SerializeField] private int inventorySize = 8;

        private int currentSelectedPosition = 0;

        public List<Item> items = new List<Item>();
        private bool isOpen;
        private bool inventorySceneLoaded = false;

        public System.Action<Item> OnItemAdded;
        public System.Action<Item> OnItemRemoved;
        public System.Action<bool> OnInventoryToggle;
        public System.Action<int> OnSelectionChanged;

        public bool IsOpen => isOpen;
        public int CurrentSelectedPosition => currentSelectedPosition;
        public List<Item> Items => items;


        private void Awake()
        {
            // Initialize empty inventory
            for (int i = 0; i < inventorySize; i++)
            {
                items.Add(null);
            }
            Debug.Log($"Inventory initialized with {inventorySize} slots");
        }

        public Item GetItem(int position)
        {
            if (position >= 0 && position < inventorySize)
            {
                return items[position];
            }
            return null;
        }

        public bool AddItem(Item newItem)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (items[i] == null)
                {
                    items[i] = newItem;
                    newItem.inventoryPosition = i;
                    Debug.Log($"Added {newItem.itemName} to slot {i}");

                    OnItemAdded?.Invoke(newItem);
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
                OnItemRemoved?.Invoke(removedItem);
                return true;
            }
            return false;
        }

        private async void OpenInventory()
        {
            Debug.Log("Inventory opened");
            isOpen = true;

            // Load additive scene for inventory UI
            if (!SceneManager.GetSceneByName(inventorySceneName).IsValid())
            {
                AsyncOperation loadOperation = SceneManager.LoadSceneAsync(inventorySceneName, LoadSceneMode.Additive);
                await loadOperation;
                inventorySceneLoaded = true;
            }

            OnInventoryToggle?.Invoke(true);
            Time.timeScale = 0f; // Pause game
        }

        private void CloseInventory()
        {
            Debug.Log("Inventory closed");
            isOpen = false;

            // Unload inventory scene
            if (inventorySceneLoaded && SceneManager.GetSceneByName(inventorySceneName).IsValid())
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

            if (direction.y > 0.5f) // Up
            {
                newPosition -= 4;
            }
            else if (direction.y < -0.5f) // Down
            {
                newPosition += 4;
            }
            else if (direction.x > 0.5f) // Right
            {
                newPosition += 1;
            }
            else if (direction.x < -0.5f) // Left
            {
                newPosition -= 1;
            }

            // Clamp to inventory bounds
            newPosition = Mathf.Clamp(newPosition, 0, inventorySize - 1);

            if (newPosition != currentSelectedPosition)
            {
                currentSelectedPosition = newPosition;
                OnSelectionChanged?.Invoke(currentSelectedPosition);
            }
        }

        private void UseSelectedItem()
        {
            if (!isOpen) return;

            Item selectedItem = GetItem(currentSelectedPosition);
            if (selectedItem != null)
            {
                selectedItem.Use(this);
                // Close inventory after use/equip
                CloseInventory();

                // If item is consumable, remove it after use
                if (selectedItem.isConsumable)
                {
                    RemoveItem(currentSelectedPosition);
                }
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            ItemPickup pickup = collision.GetComponent<ItemPickup>();
            if (pickup != null && pickup.item != null)
            {
                if (AddItem(pickup.item))
                {
                    Destroy(pickup.gameObject);
                }
            }
        }


        #region Input System Callbacks
        public void OnNavigate(InputAction.CallbackContext context) // WASD/ARROWS
        {
            if (context.performed && isOpen)
            {
                Vector2 input = context.ReadValue<Vector2>();
                NavigateSelection(input);
            }
        }

        public void OnSubmit(InputAction.CallbackContext context) // ENTER
        {
            if (context.performed)
            {
                if (isOpen)
                {
                    UseSelectedItem();
                }
                else
                {
                    // Regular game interaction when inventory is closed
                    Debug.Log("Interact action performed");
                }
            }
        }

        public void OnInventory(InputAction.CallbackContext context) // SPACE
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
                    Debug.Log("Inventory action performed");
                }
            }
        }

        public void OnCancel(InputAction.CallbackContext context) // ESCAPE
        {
            if (context.performed && isOpen)
            {
                CloseInventory();
            }
        }

        #endregion
    }
}
