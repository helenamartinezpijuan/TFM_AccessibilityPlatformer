using UnityEngine;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Sunglasses", menuName = "PlatformerGame/Inventory/Items/Sunglasses")]
    public class Sunglasses : Item
    {
        [Header("Events")]
        [SerializeField] private ItemEvents itemEvents;
        
        private bool isEquipped = false;

        public bool IsEquipped() => isEquipped;
        
        public override void Use(PlayerInventory inventory)
        {
            // Sunglasses auto-equip on pickup
        }
        
        public void OnObtained()
        {
            isEquipped = true;
            
            // Notify listeners
            if (itemEvents != null)
            {
                itemEvents.NotifySunglassesObtained();
            }
            
            // Find flashlight and upgrade it
            Flashlight flashlight = FindFlashlightInInventory();
            if (flashlight != null)
            {
                flashlight.UpdateFlashlight();
            }
            
            Debug.Log("Sunglasses obtained - auto-equipped");
        }
        
        private Flashlight FindFlashlightInInventory()
        {
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory != null)
            {
                foreach (Item item in inventory.Items)
                {
                    if (item is Flashlight flashlight)
                    {
                        return flashlight;
                    }
                }
            }
            return null;
        }
        
        public override void OnAddToInventory(PlayerInventory inventory)
        {
            // Auto-equip when added to inventory
            OnObtained();
        }
    }
}