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
        
        private bool isVisible = false;
        public LeverType GetRequiredLever() => requiredLever;
        
        public void ShowMarker()
        {
            if (markerSprite != null && !isVisible)
            {
                markerSprite.enabled = true;
                isVisible = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null)
            {
                markerSprite.enabled = false;
            }
            isVisible = false;
        }
    }
}