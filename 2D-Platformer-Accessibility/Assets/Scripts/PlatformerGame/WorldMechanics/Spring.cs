using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.WorldMechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class Spring : MonoBehaviour
    {
    [Header("Spring Configuration")]
    [SerializeField] private Vector2 destinationPosition;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator springAnimator;
    
    // Animation hashes
    private static readonly int SpringTrigger = Animator.StringToHash("Spring");
    
    // Spring state
    private bool isSpringActive = false;
    private float jumpTimer = 0f;
    private Vector2 jumpStartPosition;
    private PlayerMovement playerMovement;
    private InputAction interactAction;
    
    // Events
    public Action OnSpringLaunch;
    public Action OnSpringLand;

    private void Awake()
    {
        // Ensure we have a collider for interaction
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
            playerInput = FindFirstObjectByType<PlayerInput>();
        
        if (interactAction == null && playerInput != null)
        {
            interactAction = playerInput.actions["Interact"];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerMovement == null)
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSpringActive)
        {
            playerMovement = null;
            playerTransform = null;
        }
    }

    private void Update()
    {
        if (playerMovement != null && interactAction != null && interactAction.triggered)
        {
            ActivateSpring();
        }
    }

    private void ActivateSpring()
    {
        if (isSpringActive || playerMovement == null) return;

        // Lock player controls
        playerMovement.LockControls();
        
        // Set up jump parameters
        isSpringActive = true;
        jumpTimer = 0f;
        jumpStartPosition = playerTransform.position;
        
        // Trigger spring animation
        if (springAnimator != null)
        {
            springAnimator.SetTrigger(SpringTrigger);
        }
        
        // Start coroutine for the jump
        StartCoroutine(PerformSpringJump(destinationPosition));
        
        OnSpringLaunch?.Invoke();
    }

    private IEnumerator PerformSpringJump(Vector2 finalDestination)
    {
        while (jumpTimer < jumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float progress = jumpTimer / jumpDuration;
            float curveValue = jumpCurve.Evaluate(progress);
            
            // Calculate current position using lerp
            Vector2 currentPosition = Vector2.Lerp(jumpStartPosition, finalDestination, curveValue);
            
            // Add arc to the jump (parabolic motion)
            float arcHeight = Mathf.Sin(progress * Mathf.PI) * 2f;
            currentPosition.y += arcHeight;
            
            playerTransform.position = currentPosition;
            
            yield return null;
        }
        
        // Ensure player lands exactly at destination
        playerTransform.position = finalDestination;
        CompleteSpringJump();
    }

    private void CompleteSpringJump()
    {
        isSpringActive = false;
        
        // Unlock player controls after a brief moment to ensure landing
        if (playerMovement != null)
        {
            StartCoroutine(EnableControlsAfterDelay(0.1f));
        }
        
        OnSpringLand?.Invoke();
    }

    private IEnumerator EnableControlsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerMovement.UnlockControls();
    }

    // Public methods for external control
    public void SetDestination(Vector2 newDestination)
    {
        destinationPosition = newDestination;
    }

    public void ForceActivate()
    {
        if (!isSpringActive && playerMovement != null)
        {
            ActivateSpring();
        }
    }
}
}