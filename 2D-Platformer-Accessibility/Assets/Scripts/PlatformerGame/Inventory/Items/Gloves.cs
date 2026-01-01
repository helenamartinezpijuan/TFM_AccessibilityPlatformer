using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Gloves", menuName = "PlatformerGame/Inventory/Gloves")]
    public class Gloves : Item
    {      
        [Header("Events")]
        [SerializeField] private ItemEvents itemEvents;
        
        private bool isEquipped = false;
        public bool IsEquipped() => isEquipped;
        
        public override void Use(PlayerInventory inventory)
        {
            // Gloves auto-equips on pickup
        }
        
        public void OnObtained(Transform player)
        {
            isEquipped = true;
            
            // Notify listeners
            if (itemEvents != null)
            {
                itemEvents.NotifyGlovesObtained();
            }
            
            Debug.Log("Gloves obtained - auto-equipped");
        }
        
        public override void OnAddToInventory(PlayerInventory inventory)
        {
            // Auto-equip when added to inventory
            OnObtained(inventory.transform);
        }
    }
}