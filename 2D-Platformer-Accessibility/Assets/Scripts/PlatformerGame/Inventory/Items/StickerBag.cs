using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "StickerBag", menuName = "PlatformerGame/Inventory/Items/StickerBag")]
    public class StickerBag : Item
    {
        [Header("Sticker Settings")]
        public List<GameObject> stickerPrefabs; // Index 0 = Marker1, 1 = Marker2, etc.
        public float placementRadius = 2f;
        
        [Header("Visuals")]
        public GameObject bagVisualPrefab;
        
        private GameObject currentBagVisual;
        private bool isEquipped = false;
        private Transform playerTransform;

        public bool IsEquipped => isEquipped;
        
        public override void Use(Inventory inventory)
        {
            if (!isEquipped)
            {
                EquipStickerBag(inventory.transform);
            }
            else
            {
                UnequipStickerBag();
            }
        }
        
        private void EquipStickerBag(Transform player)
        {
            playerTransform = player;
            isEquipped = true;
            
            // Show bag on player
            if (bagVisualPrefab != null)
            {
                currentBagVisual = Instantiate(bagVisualPrefab, player.position, Quaternion.identity, player);
            }
            
            Debug.Log("Sticker Bag equipped - Press E near levers to mark them");
        }
        
        private void UnequipStickerBag()
        {
            isEquipped = false;
            
            if (currentBagVisual != null)
            {
                Destroy(currentBagVisual);
                currentBagVisual = null;
            }
            
            playerTransform = null;
            Debug.Log("Sticker Bag unequipped");
        }
        
        // This should be called from player's interaction system
        public bool TryPlaceStickerOnLever(CombinationLever lever)
        {
            if (!isEquipped || stickerPrefabs.Count == 0) return false;
            
            // Get lever type index (A=0, B=1, C=2, etc.)
            int leverIndex = (int)lever.GetLeverType();
            
            if (leverIndex < stickerPrefabs.Count && stickerPrefabs[leverIndex] != null)
            {
                // Place sticker above lever
                Vector3 stickerPosition = lever.transform.position + new Vector3(0, 1f, 0);
                GameObject sticker = Instantiate(stickerPrefabs[leverIndex], stickerPosition, Quaternion.identity);
                sticker.transform.SetParent(lever.transform);
                
                Debug.Log($"Placed sticker on Lever {lever.GetLeverType()}");
                return true;
            }
            
            return false;
        }
        
        public bool IsInRange(Transform leverTransform)
        {
            if (!isEquipped || playerTransform == null) return false;
            
            float distance = Vector2.Distance(playerTransform.position, leverTransform.position);
            return distance <= placementRadius;
        }
        
        public override void OnRemoveFromInventory(Inventory inventory)
        {
            if (isEquipped)
            {
                UnequipStickerBag();
            }
        }
    }
}