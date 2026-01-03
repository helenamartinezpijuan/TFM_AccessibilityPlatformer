using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerGame.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        [Header("Item Settings")]
        public Item item;
        
        [Header("Pickup Settings")]
        [SerializeField] private float pickupRadius = 1f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private bool autoPickup = false; // Set to true for automatic pickup without button press
        
        [Header("Visual Feedback")]
        [SerializeField] private GameObject pickupPrompt; // UI prompt like "Press E to pickup"
        [SerializeField] private float promptDisplayDistance = 2f;
        
        private bool playerInRange = false;
        private PlayerInventory playerInventory;
        private PlayerInput playerInput;
        private InputAction interactAction;
        
        private void Start()
        {
            // Disable pickup prompt if set
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false);
            }
        }
        
        private void Update()
        {
            CheckForPlayerInRange();
            
            if (playerInRange)
            {
                // Handle auto-pickup
                if (autoPickup)
                {
                    TryPickupItem();
                }
                
                // Update UI prompt position if needed
                UpdatePickupPrompt();
            }
        }
        
        private void CheckForPlayerInRange()
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, pickupRadius, playerLayer);
            
            bool wasInRange = playerInRange;
            playerInRange = playerCollider != null;
            
            if (playerInRange && !wasInRange)
            {
                // Player just entered range
                OnPlayerEnterRange(playerCollider);
            }
            else if (!playerInRange && wasInRange)
            {
                // Player just left range
                OnPlayerExitRange();
            }
        }
        
        private void OnPlayerEnterRange(Collider2D playerCollider)
        {
            // Get player inventory and input
            playerInventory = playerCollider.GetComponent<PlayerInventory>();
            playerInput = playerCollider.GetComponent<PlayerInput>();
            
            if (playerInput != null)
            {
                // Get the interact action from the player's Input Actions
                interactAction = playerInput.actions["Interact"];
                
                // Subscribe to the performed event
                if (!autoPickup && interactAction != null)
                {
                    interactAction.performed += OnInteractPerformed;
                }
            }
            
            // Show pickup prompt if not auto-pickup
            if (!autoPickup && pickupPrompt != null)
            {
                pickupPrompt.SetActive(true);
            }
            
            Debug.Log($"Player entered pickup range of {item?.itemName}");
        }
        
        private void OnPlayerExitRange()
        {
            // Unsubscribe from input events
            if (interactAction != null && !autoPickup)
            {
                interactAction.performed -= OnInteractPerformed;
            }
            
            // Hide pickup prompt
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false);
            }
            
            playerInventory = null;
            playerInput = null;
            interactAction = null;
            
            Debug.Log($"Player left pickup range of {item?.itemName}");
        }
        
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (playerInRange && playerInventory != null)
            {
                TryPickupItem();
            }
        }
        
        private void TryPickupItem()
        {
            if (playerInventory != null && item != null)
            {
                // Use the silent add option first to avoid UI refresh issues during pickup
                if (playerInventory.AddItem(item, silent: true))
                {
                    Debug.Log($"Picked up {item.itemName}");
                    
                    // Save inventory immediately
                    playerInventory.SaveInventory();
                    
                    // Visual feedback
                    OnItemPickedUp();
                    
                    // Clean up input subscription
                    if (interactAction != null && !autoPickup)
                    {
                        interactAction.performed -= OnInteractPerformed;
                    }
                    
                    // Destroy or disable the pickup object
                    DestroyPickupObject();
                }
                else
                {
                    Debug.Log($"Failed to pick up {item.itemName} - inventory might be full");
                    // Optional: Play a "can't pickup" sound or show message
                }
            }
        }
        
        private void OnItemPickedUp()
        {
            // Visual/audio feedback
            // You can add particle effects, sounds, etc. here
            
            // Hide the pickup prompt
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false);
            }
            
            // Optional: Play pickup sound
            // AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            
            // Optional: Show floating text
            // ShowPickupText($"+1 {item.itemName}");
        }
        
        private void DestroyPickupObject()
        {
            // Option 1: Immediate destroy
            Destroy(gameObject);
            
            // Option 2: Fade out then destroy (if you want animation)
            // StartCoroutine(FadeOutAndDestroy());
            
            // Option 3: Just disable renderer and collider (if you want to keep GameObject for some reason)
            /*
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) renderer.enabled = false;
            
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            
            // Disable this script
            enabled = false;
            */
        }
        
        private void UpdatePickupPrompt()
        {
            if (pickupPrompt != null && playerInRange)
            {
                // Position the prompt above the item
                Vector3 promptPosition = transform.position + Vector3.up * promptDisplayDistance;
                pickupPrompt.transform.position = Camera.main.WorldToScreenPoint(promptPosition);
                
                // Optional: Make the prompt face the camera
                // pickupPrompt.transform.rotation = Quaternion.identity;
            }
        }
        
        // Fallback for keyboard input (optional, for debugging)
        private void OnKeyInput()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E) && !autoPickup)
            {
                TryPickupItem();
            }
        }
        
        // For Editor visualization
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
            
            if (pickupPrompt != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * promptDisplayDistance);
            }
        }
        
        // Clean up to prevent memory leaks
        private void OnDestroy()
        {
            if (interactAction != null && !autoPickup)
            {
                interactAction.performed -= OnInteractPerformed;
            }
        }
        
        // Optional: Coroutine for fade out animation
        /*
        private System.Collections.IEnumerator FadeOutAndDestroy()
        {
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            float fadeTime = 0.5f;
            float elapsedTime = 0f;
            Color startColor = renderer.color;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
            
            Destroy(gameObject);
        }
        */
    }
}