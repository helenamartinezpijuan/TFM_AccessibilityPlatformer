using UnityEngine;

namespace PlatformerGame.Inventory.Items
{
    public class SwitchVision : MonoBehaviour
    {
        [Header("Revealable Settings")]
        [SerializeField] private SpriteRenderer objectRenderer;
        [SerializeField] private Sprite defaultSprite;
        
        private bool isRevealed = false;
        private Sprite originalSprite;

        private void Start()
        {
            if (objectRenderer == null)
            {
                objectRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (objectRenderer != null)
            {
                originalSprite = objectRenderer.sprite;
            }
            
            // Use default sprite if provided, otherwise keep current sprite
            if (defaultSprite != null)
            {
                originalSprite = defaultSprite;
                objectRenderer.sprite = defaultSprite;
            }
        }

        public void Reveal(Sprite revealedSprite)
        {
            if (isRevealed || objectRenderer == null) return;

            if (revealedSprite != null)
            {
                objectRenderer.sprite = revealedSprite;
            }
            
            isRevealed = true;
            
            // Optional: Add reveal effects
            OnRevealed();
        }

        public void Hide()
        {
            if (!isRevealed || objectRenderer == null) return;

            objectRenderer.sprite = originalSprite;
            isRevealed = false;
            
            // Optional: Add hide effects
            OnHidden();
        }

        protected virtual void OnRevealed()
        {
            // Override for custom reveal behavior
            // Example: Play sound, trigger events, etc.
        }

        protected virtual void OnHidden()
        {
            // Override for custom hide behavior
        }

        private void OnDisable()
        {
            // Reset when object is disabled
            if (isRevealed)
            {
                Hide();
            }
        }
    }
}