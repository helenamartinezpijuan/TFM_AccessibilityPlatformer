using UnityEngine;
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

        private PlayerInput playerInput;
        private InputAction interactAction;
        private List<IInteractable> availableInteractables = new List<IInteractable>();
        private IInteractable currentClosestInteractable;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
            }
        }

        private void Update()
        {
            FindNearbyInteractables();

            if (interactAction != null && interactAction.triggered)
            {
                TryInteract();
            }
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
            currentClosestInteractable = GetClosestInteractable();

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
            if (currentClosestInteractable != null && currentClosestInteractable.CanInteract())
            {
                currentClosestInteractable.Interact(gameObject);
            }
        }

    }
}