using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace PlatformerGame.WorldMechanics
{
public class Lever : MonoBehaviour, IInteractable
{
    [Header("Lever One or Lever Two")]
    [SerializeField] private bool isOne;

    [Header("Activation Target")]
    [SerializeField] private MovingPlatform platformToActivate;
    
    [Header("Visuals")]
    [SerializeField] private GameObject accessibleVisuals;
    private Animator leverAnimator;
    
    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        leverAnimator = GetComponent<Animator>();
        leverAnimator.SetBool("IsOne", isOne);
    }

    public bool CanInteract()
    {
        return !isActivated; // Can only interact once

        // If this changes, add logic to trigger TURN OFF animation (ready in Unity Animator)
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract()) return;
        
        ActivatePlatform();
        TriggerAnimation();
        isActivated = true;
    }

    private void ActivatePlatform()
    {
        if (platformToActivate != null)
        {
            platformToActivate.OnLeverActivated();
            Debug.Log("Platform activated");
        }
    }

    private void TriggerAnimation()
    {        
        if (leverAnimator != null)
        {
            leverAnimator.SetBool("IsOn", true);
        }
    }

    public void InstantiateAccessibleVisuals()
    {
        GameObject.Instantiate(accessibleVisuals, transform.position, Quaternion.identity);
    }
}
}