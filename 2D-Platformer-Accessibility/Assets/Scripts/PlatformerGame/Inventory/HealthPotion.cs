using UnityEngine;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "PlatformerGame/Inventory/HealthPotion")]
    public class HealthPotion : Item
    {
        public int healAmount = 50;

        public override void Use(Inventory inventory)
        {
            // Heal the player logic here
            Debug.Log($"Used {itemName} to heal {healAmount} HP");
        }

        public override bool CanUse(Inventory inventory)
        {
            // Check if player needs healing
            return true;
        }
    }
}