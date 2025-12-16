using UnityEngine;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Sunglasses", menuName = "PlatformerGame/Inventory/Items/Sunglasses")]
    public class Sunglasses : Item
    {
        private bool isEquipped = false;
        
        public override void Use(PlayerInventory inventory)
        {
            // Sunglasses auto-equip on pickup
        }
        
        public void OnObtained()
        {
            isEquipped = true;
            
            // Find flashlight and upgrade it
            Flashlight flashlight = FindFlashlightInInventory();
            if (flashlight != null)
            {
                flashlight.OnSunglassesObtained();
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
        
        public bool IsEquipped => isEquipped;
    }
}