using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Flashlight", menuName = "PlatformerGame/Inventory/Flashlight")]
    public class Flashlight : Item
    {      
        [Header("Events")]
        [SerializeField] private ItemEvents itemEvents;
        
        private bool isEquipped = false;
        public bool IsEquipped() => isEquipped;
        
        public override void Use(PlayerInventory inventory)
        {
            // Flashlight auto-equips on pickup
        }
        
        public void OnObtained(Transform player)
        {
            isEquipped = true;
            
            // Notify listeners
            if (itemEvents != null)
            {
                itemEvents.NotifyFlashlightObtained();
            }
            
            Debug.Log("Flashlight obtained - auto-equipped");
        }
        
        public void UpdateFlashlight()
        {
            // Phase 2: Reveal hidden walls
            if (itemEvents != null)
            {
                itemEvents.NotifyRevealHiddenWalls();
            }
        }
        
        public override void OnAddToInventory(PlayerInventory inventory)
        {
            // Auto-equip when added to inventory
            OnObtained(inventory.transform);
        }
    }
}