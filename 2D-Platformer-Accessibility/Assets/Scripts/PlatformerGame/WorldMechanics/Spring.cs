using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.WorldMechanics
{
    public class Spring : MonoBehaviour, IInteractable
    {
        [Header("Spring Configuration")]
        [SerializeField] private GameObject destination;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private Vector2 destinationPosition;

        [Header("Animation")]
        [SerializeField] private bool isOne;
        private Animator springAnimator;
        [SerializeField] private Animator landingAnimator;
        private SpriteRenderer landingSpriteRenderer;


        // Spring state
        private bool isSpringActive = false;
        private PlayerMovement playerMovement;
        private Rigidbody2D playerRigidbody;

        // Events
        public Action OnSpringLaunch;
        public Action OnSpringLand;

        void Awake()
        {
            // Set animator parameters
            springAnimator = GetComponent<Animator>();
            springAnimator.SetBool("IsOne", isOne);
            // Hide animation until it is triggered
            landingSpriteRenderer = destination.GetComponent<SpriteRenderer>();
            landingSpriteRenderer.enabled = false;

            // Set destination parameters
            destinationPosition = destination.transform.position;
        }

        public bool CanInteract()
        {
            return !isSpringActive;
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract()) return;

            playerMovement = interactor.GetComponent<PlayerMovement>();
            playerRigidbody = interactor.GetComponent<Rigidbody2D>();
            if (playerMovement == null || playerRigidbody == null) return;

            StartCoroutine(PerformSpringJump(interactor.transform));
        }

        private IEnumerator PerformSpringJump(Transform playerTransform)
        {
            // Lock player controls
            playerMovement.LockControls();

            // Disable physics and enable kinematic to prevent falling during jump
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.bodyType = RigidbodyType2D.Kinematic;

            // Trigger spring animation
            isSpringActive = true;

            if (springAnimator != null)
            {
                springAnimator.SetTrigger("Spring");
            }

            Vector2 jumpStartPosition = playerTransform.position;
            OnSpringLaunch?.Invoke();

            float timer = 0f;
            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / jumpDuration;
                float curveValue = jumpCurve.Evaluate(progress);

                // Calculate current position using lerp
                Vector2 currentPosition = Vector2.Lerp(jumpStartPosition, destinationPosition, curveValue);

                // Add arc to the jump (parabolic motion)
                float arcHeight = Mathf.Sin(progress * Mathf.PI) * 2f;
                currentPosition.y = arcHeight + Mathf.Lerp(jumpStartPosition.y, destinationPosition.y, progress);

                playerTransform.position = currentPosition;
                yield return null;
            }

            // Ensure player lands exactly at destination
            playerTransform.position = destinationPosition;

            // Re-enable physics
            playerRigidbody.bodyType = RigidbodyType2D.Dynamic;

            // Small downward force to ensure collision with ground
            playerRigidbody.linearVelocity = new Vector2(0f, -1f);

            CompleteSpringJump();
        }

        private void CompleteSpringJump()
        {
            landingSpriteRenderer.enabled = true;
            landingAnimator.SetTrigger("Land");

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
            isSpringActive = false;

            // Hide landing animation after it plays
            yield return new WaitForSeconds(0.5f); // Wait for animation to complete
            if (landingSpriteRenderer != null)
            {
                landingSpriteRenderer.enabled = false;
            }
        }
    }
}