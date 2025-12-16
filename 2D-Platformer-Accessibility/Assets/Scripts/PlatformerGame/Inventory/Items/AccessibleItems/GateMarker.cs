using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class GateMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private LeverType requiredLever;
        [SerializeField] private SpriteRenderer markerSprite;
        
        public LeverType GetRequiredLever() => requiredLever;
        
        public void ShowMarker()
        {
            if (markerSprite != null)
            {
                markerSprite.enabled = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null)
            {
                markerSprite.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Flashlight"))
            {
                Flashlight flashlight = other.GetComponent<Flashlight>();
                if (flashlight != null)
                {
                    ShowMarker();
                }
            }
        }
    }
}