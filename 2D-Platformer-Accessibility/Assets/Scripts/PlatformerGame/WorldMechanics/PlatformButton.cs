using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public class PlatformButton : MonoBehaviour, IInteractable
    {       
        [Header("Activation Target")]
        [SerializeField] private bool isExclusive = false;
        [SerializeField] private MovingPlatform platformToActivate;
        [SerializeField] private MovingPlatform platformToDeactivate;

        // Button animation
        private Animator buttonAnimator;
        private bool isPressed = false;
        
        private void Awake()
        {
            buttonAnimator = GetComponent<Animator>();
            
            if (buttonAnimator != null)
            {
                buttonAnimator.SetBool("IsPressed", isPressed);
            }
        }
        
        public bool CanInteract()
        {
            return true; // Button can always be pressed
        }
        
        public void Interact(GameObject interactor)
        {
            PressButton();
        }
        
        public void PressButton()
        {
            isPressed = !isPressed;
            
            // Update animation
            if (buttonAnimator != null)
            {
                buttonAnimator.SetBool("IsPressed", isPressed);
            }
            
            // Activate/deactivate platform
            if (platformToActivate != null)
            {
                platformToActivate.OnInteractorActivated();

                if (isExclusive && platformToDeactivate != null)
                    platformToDeactivate.OnInteractorActivated();
            }
        }
        
        public void TriggerAnimation()
        {        
            // This method can be called from other scripts if needed
            PressButton();
        }
    }
}