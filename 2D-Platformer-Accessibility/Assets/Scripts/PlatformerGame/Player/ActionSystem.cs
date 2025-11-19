using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Player
{
    public class ActionSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableLayerMask = ~0;

        [Header("Events")]
        public UnityEvent<IInteractable> OnInteractableFound;
        public UnityEvent OnInteractableLost;

        private List<IInteractable> availableInteractables = new List<IInteractable>();
        private IInteractable currentClosestInteractable;


        private void Start()
        {
            Debug.Log("InteractionSystem Started - looking for Input System events");
        }

        public void OnInteract(InputAction.CallbackContext context)
        {           
            if (context.performed)
            {
                TryInteract();
            }
        }

        private void Update()
        {
            FindNearbyInteractables();
        }

        private void FindNearbyInteractables()
        {
            // Clear previous list
            availableInteractables.Clear();

            // Find all interactables in range
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayerMask);

            foreach (Collider2D collider in hitColliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract())
                {
                    availableInteractables.Add(interactable);
                }
            }

            // Find the closest interactable
            IInteractable newClosest = GetClosestInteractable();

            // Handle interactable change events
            if (newClosest != currentClosestInteractable)
            {
                if (currentClosestInteractable != null)
                {
                    OnInteractableLost?.Invoke();
                }

                currentClosestInteractable = newClosest;

                if (currentClosestInteractable != null)
                {
                    OnInteractableFound?.Invoke(currentClosestInteractable);
                }
            }

            // You could add UI feedback here (show interaction prompt for currentClosestInteractable)
        }

        private IInteractable GetClosestInteractable()
        {
            IInteractable closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (IInteractable interactable in availableInteractables)
            {
                float distance = Vector2.Distance(transform.position, (interactable as MonoBehaviour).transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }

            return closest;
        }

        private void TryInteract()
        {
            Debug.Log("Try Interact called");
            if (currentClosestInteractable != null && currentClosestInteractable.CanInteract())
            {
                currentClosestInteractable.Interact(gameObject);
                Debug.Log("Interaction called from ActionSystem.cs script");
            }
        }

    }
}