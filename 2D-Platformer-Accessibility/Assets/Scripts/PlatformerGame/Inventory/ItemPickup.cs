using UnityEngine;
using TMPro;

namespace PlatformerGame.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        public Item item;
        [SerializeField] private float pickupRadius = 1f;
        [SerializeField] private LayerMask playerLayer;
        
        private bool playerInRange = false;
        
        private void Update()
        {
            CheckForPlayerPickup();
        }
        
        private void CheckForPlayerPickup()
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, pickupRadius, playerLayer);
            
            bool wasInRange = playerInRange;
            playerInRange = player != null;
            
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                PickupItem(player.GetComponent<PlayerInventory>());
            }
        }
        
        private void PickupItem(PlayerInventory inventory)
        {
            if (inventory != null && item != null)
            {
                if (inventory.AddItem(item))
                {
                    Debug.Log($"Picked up {item.itemName}");
                    Destroy(gameObject);
                }
            }
        }
    }
}