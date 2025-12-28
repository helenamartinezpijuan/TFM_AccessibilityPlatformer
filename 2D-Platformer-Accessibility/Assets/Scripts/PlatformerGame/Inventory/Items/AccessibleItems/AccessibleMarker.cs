using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public abstract class AccessibleMarker : ScriptableObject
    {
        [Header("Marker Settings")]
        public string markerName;
        public bool isNumber;
        public Sprite icon;
        public Light2D light;

        // Abstract method that must be implemented by all derived items
        public abstract void ShowMarker();
        public abstract void HideMarker();

        // Virtual methods that can be overridden by derived classes
        public virtual void OnAddToStickerBag(StickerBag stickerBag)
        {
            Debug.Log($"{markerName} added to sticker bag");
        }

        public virtual void OnRemoveFromStickerBag(StickerBag stickerBag)
        {
            Debug.Log($"{markerName} removed from sticker bag");
        }
    }
}