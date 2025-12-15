using UnityEngine;
using PlatformerGame.Inventory.Items;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Key", menuName = "PlatformerGame/Inventory/Key")]
    public class Key : Item
    {
        public string doorId;

        public override void Use(Inventory inventory)
        {
            Debug.Log($"Used {itemName} to unlock door with ID: {doorId}");
            // Implement door unlocking logic here
        }

        public override bool CanUse(Inventory inventory)
        {
            // Implement logic to check if the key can be used (e.g., if the player is near a door with matching ID)
            return true; // Placeholder
        }
    }
}
