using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "StickerBag", menuName = "PlatformerGame/Inventory/StickerBag")]
    public class StickerBag : Item
    {
        [Header("Sticker Settings")]
        public List<GameObject> stickerPrefabs;
        public int maxStickers = 10;
        
        [Header("Usage Settings")]
        public float placementRange = 3f;
        public LayerMask placementLayers;
        
        private List<GameObject> placedStickers = new List<GameObject>();
        private int currentStickerIndex = 0;
        private bool isPlacingMode = false;
        
        public override void Use(Inventory inventory)
        {
            if (stickerPrefabs.Count == 0)
            {
                Debug.LogWarning("StickerBag has no sticker prefabs!");
                return;
            }
            
            if (!isPlacingMode)
            {
                StartPlacingMode(inventory.transform);
            }
            else
            {
                CancelPlacingMode();
            }
        }
        
        private void StartPlacingMode(Transform player)
        {
            isPlacingMode = true;
            Debug.Log("Sticker placing mode activated. Click to place, ESC to cancel.");
            
            // You might want to show a UI indicator here
        }
        
        private void CancelPlacingMode()
        {
            isPlacingMode = false;
            Debug.Log("Sticker placing mode cancelled");
        }
        
        // This should be called from a separate input handler
        public void TryPlaceSticker(Vector3 position, Inventory inventory)
        {
            if (!isPlacingMode || stickerPrefabs.Count == 0) return;
            
            if (placedStickers.Count >= maxStickers)
            {
                Debug.Log("Sticker bag is empty!");
                return;
            }
            
            // Place the sticker
            GameObject stickerPrefab = stickerPrefabs[currentStickerIndex];
            GameObject sticker = Instantiate(stickerPrefab, position, Quaternion.identity);
            placedStickers.Add(sticker);
            
            // Cycle to next sticker
            currentStickerIndex = (currentStickerIndex + 1) % stickerPrefabs.Count;
            
            // Consume if it's a consumable item
            if (isConsumable)
            {
                inventory.RemoveItem(inventoryPosition);
            }
            
            Debug.Log($"Sticker placed! {placedStickers.Count}/{maxStickers} used");
        }
        
        public override bool CanUse(Inventory inventory)
        {
            return true;
        }
        
        public List<GameObject> GetPlacedStickers()
        {
            return placedStickers;
        }
        
        public int GetRemainingStickers()
        {
            return maxStickers - placedStickers.Count;
        }
    }
}