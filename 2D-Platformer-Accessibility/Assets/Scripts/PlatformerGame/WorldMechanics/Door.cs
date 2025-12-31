using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private bool isOpen = true;
        [SerializeField] private Transform destination;
        
        [Header("Accessibility Settings")]
        [SerializeField] private SpriteRenderer numberSprite;
        private AudioSource audio;
        
        private void Awake()
        {
            audio = GetComponent<AudioSource>();
            numberSprite.enabled = false;
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
            
            if (audio != null)
            {
                audio.Play();
            }
            
            Debug.Log("Player teleported through door");
        }

        public void EnableNumberSprite()
        {
            if (numberSprite != null)
            {
                numberSprite.enabled = true;
            }
        }
    }
}