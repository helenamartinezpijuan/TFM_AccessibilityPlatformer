using UnityEngine;
using PlatformerGame.Inventory;
public class GateMarkerDebug : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"GateMarker: OnTriggerEnter2D with {other.name}, Tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("GateMarker: Player detected!");
            
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("GateMarker: PlayerInventory component is missing on player!");
            }
            else
            {
                Debug.Log($"GateMarker: HasFlashlight: {playerInventory.HasFlashlight()}, HasSunglasses: {playerInventory.HasSunglasses()}");
            }
        }
    }
}
