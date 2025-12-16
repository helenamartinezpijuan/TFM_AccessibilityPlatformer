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
        }
        
        private void Update()
        {
            // Find flashlight in inventory if not already found
            if (currentFlashlight == null)
            {
                FindFlashlightInInventory();
            }
            
            // Update flashlight effects
            if (currentFlashlight != null)
            {
                currentFlashlight.UpdateFlashlight();
            }
        }
        
        private void FindFlashlightInInventory()
        {
            if (inventory == null) return;
            
            foreach (Item item in inventory.Items)
            {
                if (item is Flashlight flashlight)
                {
                    currentFlashlight = flashlight;
                    break;
                }
            }
        }
        
        public void OnItemAdded(Item item)
        {
            // Check if sunglasses were added
            if (item is Sunglasses sunglasses && currentFlashlight != null)
            {
                currentFlashlight.OnSunglassesObtained();
            }
        }
    }
}