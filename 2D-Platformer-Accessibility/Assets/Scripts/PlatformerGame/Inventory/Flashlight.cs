using UnityEngine;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    [CreateAssetMenu(fileName = "Flashlight", menuName = "PlatformerGame/Inventory/Flashlight")]
    public class Flashlight : Item
    {
        public string doorId;

        public override void Use(Inventory inventory)
        {
            Debug.Log($"Flashlight in use");
            // On/Off logic
        }

        public override bool CanUse(Inventory inventory)
        {
            return true;
        }
    }
}
