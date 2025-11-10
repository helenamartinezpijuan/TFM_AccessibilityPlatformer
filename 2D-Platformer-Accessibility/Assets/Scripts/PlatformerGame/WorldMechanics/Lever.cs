using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace PlatformerGame.WorldMechanics
{
    public class Lever : MonoBehaviour, IInteractable
    {
        [Header("Lever Configuration")]
        [SerializeField] private bool isOn = false;
        [SerializeField] private Animator leverAnimator;

        // Animation hashes
        private static readonly int LeverOn = Animator.StringToHash("On");
        private static readonly int LeverOff = Animator.StringToHash("Off");

        public bool CanInteract()
        {
            return true; // Lever can always be interacted with
        }

        public void Interact(GameObject interactor)
        {
            isOn = !isOn;

            // Trigger appropriate animation
            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger(isOn ? LeverOn : LeverOff);
            }

            // Do lever functionality (open doors, activate platforms, etc.)
            OnLeverToggle?.Invoke(isOn);

            Debug.Log($"Lever turned {(isOn ? "ON" : "OFF")} by {interactor.name}");
        }

        // Event for other objects to listen to
        public System.Action<bool> OnLeverToggle;
    }
}