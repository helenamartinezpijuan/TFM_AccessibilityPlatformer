using UnityEngine;
using UnityEngine.InputSystem;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public abstract class Item : ScriptableObject
    {
        [Header("Item Basic Info")]
        public string itemName;
        public string description;
        public Sprite icon;
        public int maxStackSize = 1;
        public bool isConsumable = false;

        [HideInInspector] public int inventoryPosition = -1;

        // Abstract method that must be implemented by all derived items
        public abstract void Use(Inventory inventory);

        // Virtual methods that can be overridden by derived classes
        public virtual bool CanUse(Inventory inventory)
        {
            return true; // Default: item can always be used
        }

        public virtual void OnAddToInventory(Inventory inventory)
        {
            Debug.Log($"{itemName} added to inventory at position {inventoryPosition}");
        }

        public virtual void OnRemoveFromInventory(Inventory inventory)
        {
            Debug.Log($"{itemName} removed from inventory");
        }
    }

}
