using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen;
    [SerializeField] private Transform destination;

    [Header("Visuals")]
    [SerializeField] private GameObject accessibleVisuals;
    private SpriteRenderer spriteRenderer;


    public bool CanInteract()
    {
        return isOpen;
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;
        
        interactor.transform.position = destination.position;
        // Trigger effect here
    }
}
}