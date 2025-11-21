using UnityEngine;
using PlatformerGame.Inventory.Items;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Flashlight", menuName = "PlatformerGame/Inventory/Flashlight")]
    public class Flashlight : Item
    {
        [Header("Flashlight Settings")]
        public GameObject flashlightPrefab;
        public LayerMask interactableLayers;
        public float rayDistance = 3f;
        public Sprite defaultSprite;
        public Sprite revealedSprite;
        
        [Header("Particle Effects")]
        public GameObject revealParticleEffect;
        
        private FlashlightController currentFlashlight;
        private bool isEquipped = false;

        public override void Use(Inventory inventory)
        {
            Debug.Log($"Flashlight in use");
            
            // Get the player transform
            Transform player = inventory.transform;
            
            if (!isEquipped)
            {
                EquipFlashlight(player);
            }
            else
            {
                UnequipFlashlight();
            }
        }

        private void EquipFlashlight(Transform player)
        {
            if (flashlightPrefab != null)
            {
                // Instantiate flashlight as child of player
                GameObject flashlightObj = Object.Instantiate(flashlightPrefab, player.position, Quaternion.identity, player);
                currentFlashlight = flashlightObj.GetComponent<FlashlightController>();
                
                if (currentFlashlight != null)
                {
                    currentFlashlight.Initialize(this);
                    isEquipped = true;
                    Debug.Log("Flashlight equipped");
                }
            }
            else
            {
                Debug.LogWarning("Flashlight prefab not assigned!");
            }
        }

        private void UnequipFlashlight()
        {
            if (currentFlashlight != null)
            {
                Object.Destroy(currentFlashlight.gameObject);
                currentFlashlight = null;
            }
            isEquipped = false;
            Debug.Log("Flashlight unequipped");
        }

        public override bool CanUse(Inventory inventory)
        {
            return true;
        }

        public override void OnRemoveFromInventory(Inventory inventory)
        {
            // Unequip if removed from inventory
            if (isEquipped)
            {
                UnequipFlashlight();
            }
        }
    }
}
