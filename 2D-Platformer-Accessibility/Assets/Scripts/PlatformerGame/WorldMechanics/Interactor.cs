using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public class Interactor : MonoBehaviour, IInteractable
    {
        [Header("Target Lever")]
        [SerializeField] private CombinationLever targetLever;
        
        public bool CanInteract()
        {
            return true; // Always interactable
        }
        
        public void Interact(GameObject interactor)
        {
            if (targetLever != null)
            {
                targetLever.Interact(interactor);
            }
        }
    }
}