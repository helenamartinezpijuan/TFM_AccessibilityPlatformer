using UnityEngine;
using PlatformerGame.Inventory;

namespace PlatformerGame.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        public Item item;
        [SerializeField] private float pickupRadius = 1f;
        [SerializeField] private LayerMask playerLayer;
        
        private void Update()
        {
            CheckForPlayerPickup();
        }

        private void CheckForPlayerPickup()
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, pickupRadius, playerLayer);
            if (player != null && Input.GetKeyDown(KeyCode.E))
            {
                PickupItem(player.GetComponent<Inventory>());
            }
        }

        private void PickupItem(Inventory inventory)
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