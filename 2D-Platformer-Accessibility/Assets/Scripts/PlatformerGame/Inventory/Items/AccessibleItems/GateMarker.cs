using UnityEngine;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class GateMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private LeverType requiredLever;
        [SerializeField] private SpriteRenderer markerSprite;
        [SerializeField] private string flashlightTag = "FlashlightBeam";
        
        public LeverType GetRequiredLever() => requiredLever;
        
        private void Start()
        {
            // Start hidden
            if (markerSprite != null)
                markerSprite.enabled = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(flashlightTag))
            {
                ShowMarker();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(flashlightTag))
            {
                HideMarker();
            }
        }
        
        public void ShowMarker()
        {
            if (markerSprite != null && !markerSprite.enabled)
            {
                markerSprite.enabled = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null && markerSprite.enabled)
            {
                markerSprite.enabled = false;
            }
        }
    }
}