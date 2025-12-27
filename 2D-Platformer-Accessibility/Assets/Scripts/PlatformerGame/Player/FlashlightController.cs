using UnityEngine;
using PlatformerGame.Inventory;
using PlatformerGame.Inventory.Items;

namespace PlatformerGame.Player
{
    public class FlashlightController : MonoBehaviour
    {
        private Flashlight currentFlashlight;
        private PlayerInventory inventory;
        
        private void Start()
        {
            inventory = GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                inventory = FindObjectOfType<PlayerInventory>();
            }
        }
        
        
    }
}