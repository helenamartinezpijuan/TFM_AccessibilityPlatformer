using UnityEngine;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "ItemEvents", menuName = "PlatformerGame/Inventory/ItemEvents")]
    public class ItemEvents : ScriptableObject
    {
        // Simple events that components can listen to
        public System.Action OnFlashlightObtained;
        public System.Action OnSunglassesObtained;
        public System.Action OnHiddenWallsShouldReveal;
        
        public void NotifyFlashlightObtained() => OnFlashlightObtained?.Invoke();
        public void NotifySunglassesObtained() => OnSunglassesObtained?.Invoke();
        public void NotifyRevealHiddenWalls() => OnHiddenWallsShouldReveal?.Invoke();
    }
}