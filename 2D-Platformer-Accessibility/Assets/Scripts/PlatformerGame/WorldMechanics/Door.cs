using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private bool isOpen = true;
        [SerializeField] private Transform destination;
        
        [Header("Visuals")]
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Collider2D doorCollider;
        
        private SpriteRenderer spriteRenderer;
        private Animator doorAnimator;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            doorAnimator = GetComponent<Animator>();
            UpdateDoorState();
        }
        
        public bool CanInteract()
        {
            return isOpen;
        }
        
        public void Interact(GameObject interactor)
        {
            if (!CanInteract()) return;
            
            // Teleport player
            interactor.transform.position = destination.position;
            
            // Optional: Play teleport animation/sound
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Teleport");
            }
            
            Debug.Log("Player teleported through door");
        }
        
        public void OpenDoor()
        {
            isOpen = true;
            UpdateDoorState();
        }
        
        public void CloseDoor()
        {
            isOpen = false;
            UpdateDoorState();
        }
        
        private void UpdateDoorState()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
            }
            
            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen;
            }
            
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("IsOpen", isOpen);
            }
        }
        
        // Call this from levers or other triggers
        public void ToggleDoor()
        {
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
    }
}