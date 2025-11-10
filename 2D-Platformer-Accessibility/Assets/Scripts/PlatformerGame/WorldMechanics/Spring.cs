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
        [SerializeField] private Vector2 destinationPosition;
        [SerializeField] private float jumpDuration = 1f;
        [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Animator springAnimator;
        [SerializeField] private bool isActive = true;

        // Animation hashes
        private static readonly int SpringTrigger = Animator.StringToHash("Spring");

        // Spring state
        private bool isSpringActive = false;
        private PlayerMovement playerMovement;

        // Events
        public Action OnSpringLaunch;
        public Action OnSpringLand;


        public void Start()
        {
            springAnimator = GetComponent<Animator>();
        }

        public bool CanInteract()
        {
            return isActive && !isSpringActive;
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract()) return;

            playerMovement = interactor.GetComponent<PlayerMovement>();
            if (playerMovement == null) return;

            StartCoroutine(PerformSpringJump(interactor.transform));
        }

        private IEnumerator PerformSpringJump(Transform playerTransform)
        {
            // Lock player controls
            playerMovement.LockControls();
            isSpringActive = true;

            Vector2 jumpStartPosition = playerTransform.position;

            // Trigger spring animation
            if (springAnimator != null)
            {
                springAnimator.SetTrigger(SpringTrigger);
            }

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
                currentPosition.y += arcHeight;

                playerTransform.position = currentPosition;

                yield return null;
            }

            // Ensure player lands exactly at destination
            playerTransform.position = destinationPosition;
            CompleteSpringJump();
        }

        private void CompleteSpringJump()
        {
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
        }

        // Editor visualization
        private void OnDrawGizmosSelected()
        {
            if (!isActive) return;

            // Draw spring position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);

            // Draw destination position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(destinationPosition, Vector3.one * 0.5f);

            // Draw line from spring to destination
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, destinationPosition);

            // Draw horizontal variance range
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                destinationPosition + Vector2.left,
                destinationPosition + Vector2.right
            );
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }
    }
}