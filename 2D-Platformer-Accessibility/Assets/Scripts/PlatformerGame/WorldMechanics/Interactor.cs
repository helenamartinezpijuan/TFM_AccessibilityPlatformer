using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace PlatformerGame.WorldMechanics
{
public class Interactor : MonoBehaviour, IInteractable
{
    [Header("Activation Object")]
    [SerializeField] private Lever lever;
    [SerializeField] private PlatformButton button;

    [Header("Activation Target")]
    [SerializeField] private MovingPlatform platformToActivate;
    [SerializeField] private Gate gateToActivate;
    
    private bool isLeverActivated = false;
    private bool isButtonActivated = false;

    public bool CanInteract()
    {
        return true; //!isActivated; // Can only interact once

        // If this changes, add logic to trigger TURN OFF animation (ready in Unity Animator)
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;
        
        if(lever != null)
        {
            ActivateGate();
            lever.TriggerAnimation();
        }
        else if (button != null)
        {
            ActivatePlatform();
            // button.TriggerAnimation();
        }
        
        //isActivated = true;
    }

    private void ActivatePlatform()
    {
        if (platformToActivate != null)
        {
            platformToActivate.OnInteractorActivated();
            Debug.Log("Platform activated");
        }
    }

    private void ActivateGate()
    {
        if (gateToActivate != null)
        {
            gateToActivate.OnInteractorActivated();
            Debug.Log("Gate activated");
        }
    }
}
}