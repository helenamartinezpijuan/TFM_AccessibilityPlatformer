using UnityEngine;
using TMPro;

namespace PlatformerGame.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        public Item item;
        [SerializeField] private float pickupRadius = 1f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private TextMeshPro promptText;
        
        private bool playerInRange = false;
        
        private void Start()
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            if (promptText != null && item != null)
            {
                promptText.text = $"Press E to pick up\n{item.itemName}";
            }
        }
        
        private void Update()
        {
            CheckForPlayerPickup();
        }
        
        private void CheckForPlayerPickup()
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, pickupRadius, playerLayer);
            
            bool wasInRange = playerInRange;
            playerInRange = player != null;
            
            // Show/hide prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(playerInRange);
            }
            
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
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