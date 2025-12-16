using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "StickerBag", menuName = "PlatformerGame/Inventory/Items/StickerBag")]
    public class StickerBag : Item
    {
        [Header("Sticker Management")]
        [SerializeField] private List<Sticker> collectedStickers = new List<Sticker>();
        [SerializeField] private int maxStickers = 8;
        
        [Header("Visual Feedback")]
        [SerializeField] private GameObject collectionEffectPrefab;
        
        public override void Use(PlayerInventory inventory)
        {
            // Sticker bag doesn't need to be equipped/unequipped
            Debug.Log($"Sticker Bag: {collectedStickers.Count}/{maxStickers} stickers collected");
        }
        
        public void AddSticker(Sticker sticker)
        {
            if (collectedStickers.Count >= maxStickers)
            {
                Debug.LogWarning("Sticker bag is full!");
                return;
            }
            
            collectedStickers.Add(sticker);
            
            // Show collection effect
            if (collectionEffectPrefab != null)
            {
                Instantiate(collectionEffectPrefab, sticker.transform.position, Quaternion.identity);
            }
            
            Debug.Log($"Added sticker {sticker.GetLeverType()} to bag. Total: {collectedStickers.Count}/{maxStickers}");
        }
        
        public bool HasSticker(LeverType leverType)
        {
            foreach (Sticker sticker in collectedStickers)
            {
                if (sticker.GetLeverType() == leverType)
                    return true;
            }
            return false;
        }
        
        public List<Sticker> GetStickers() => new List<Sticker>(collectedStickers);
        public int GetStickerCount() => collectedStickers.Count;
        public int GetMaxStickers() => maxStickers;
    }
}