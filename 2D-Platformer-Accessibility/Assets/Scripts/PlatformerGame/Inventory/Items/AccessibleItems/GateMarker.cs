using UnityEngine;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class GateMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private LeverType requiredLever;
        [SerializeField] private SpriteRenderer markerSprite;
        [SerializeField] private Light markerLight;
        
        public LeverType GetRequiredLever() => requiredLever;
        
        private void Start()
        {
            // Start hidden
            if (markerSprite != null)
                markerSprite.enabled = false;
            
            if (markerLight != null)
                markerLight.enabled = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            bool hasFlahslight = other.GetComponent<PlayerInventory>().HasFlashlight();
            bool hasSunglasses = other.GetComponent<PlayerInventory>().HasSunglasses();

            if (other.CompareTag("Player"))
            {
                if (hasFlahslight && !hasSunglasses)
                {
                    ShowMarker();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            bool hasSunglasses = other.GetComponent<PlayerInventory>().HasSunglasses();

            if (other.CompareTag("Player"))
            {
                if (!hasSunglasses)
                {
                    HideMarker();
                }
            }
        }
        
        public void ShowMarker()
        {
            if (markerSprite != null && !markerSprite.enabled)
            {
                markerSprite.enabled = true;
                markerLight.enabled = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null && markerSprite.enabled)
            {
                markerSprite.enabled = false;
                markerLight.enabled = false;
            }
        }
    }
}