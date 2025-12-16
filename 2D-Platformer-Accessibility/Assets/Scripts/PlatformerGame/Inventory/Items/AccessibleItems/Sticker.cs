using UnityEngine;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    public class Sticker : MonoBehaviour
    {
        [Header("Sticker Settings")]
        [SerializeField] private LeverType leverType;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        
        private bool isCollected = false;

        public LeverType GetLeverType() => leverType;
        public Sprite GetSprite() => spriteRenderer.sprite;
        
        private void Start()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Start floating animation
            if (animator != null)
                animator.Play("StickerFloat");
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected) return;
            
            if (other.CompareTag("Player"))
            {
                CollectSticker(other.GetComponent<PlayerInventory>());
            }
        }
        
        private void CollectSticker(PlayerInventory inventory)
        {
            if (inventory == null || isCollected) return;
            
            isCollected = true;
            
            // Add sticker to player's sticker bag
            StickerBag stickerBag = FindStickerBag(inventory);
            if (stickerBag != null)
            {
                stickerBag.AddSticker(this);
            }
            
            // Play collection effect
            if (animator != null)
                animator.SetTrigger("Collect");
            
            // Disable after collection
            spriteRenderer.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            
            // Destroy after animation
            Destroy(gameObject, 1f);
            
            Debug.Log($"Collected sticker: {leverType}");
        }
        
        private StickerBag FindStickerBag(PlayerInventory inventory)
        {
            foreach (Item item in inventory.Items)
            {
                if (item is StickerBag stickerBag)
                {
                    return stickerBag;
                }
            }
            return null;
        }
    }
}