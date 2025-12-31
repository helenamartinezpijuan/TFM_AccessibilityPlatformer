using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace PlatformerGame.WorldMechanics
{
    public class Interactor : MonoBehaviour, IInteractable
    {
        [Header("Activation Object")]
        [SerializeField] private CombinationLever lever;
        [SerializeField] private PlatformButton button;

        [Header("Activation Target")]
        [SerializeField] private MovingPlatform platformToActivate;
        [SerializeField] private CombinationGate gateToActivate;
        
        public bool CanInteract()
        {
            return true; // Always interactable
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract()) return;
            
            if(lever != null)
            {
                lever.Interact(interactor);
            }
            else if (button != null)
            {
                button.Interact(interactor);
            }
            else if (platformToActivate != null)
            {
                platformToActivate.OnInteractorActivated();
            }
            else if (gateToActivate != null)
            {
                Debug.Log("Note: Gates are controlled by lever combinations, not directly");
            }
        }
    }
}