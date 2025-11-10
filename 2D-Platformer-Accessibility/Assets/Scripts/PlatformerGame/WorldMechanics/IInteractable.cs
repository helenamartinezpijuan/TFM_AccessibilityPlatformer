using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public interface IInteractable
    {
        bool CanInteract();
        void Interact(GameObject interactor);
    }
}