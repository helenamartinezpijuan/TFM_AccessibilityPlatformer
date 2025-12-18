using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxClimbAngle = 45f;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D rb;
        private Animator animator;
        private float horizontalAxis;
        private bool isFacingRight = true;
        private bool controlsLocked = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            // Add particle effect for pebbles flying off when moving wheelchair
        }

        private void FixedUpdate()
        {
            Move();
        }

        #region Movement Logic
        private void Move()
        {
            //float moveForce = horizontalAxis * speed;
            //rb.linearVelocity = new Vector2(moveForce, rb.linearVelocity.y);

            if (Mathf.Abs(horizontalAxis) > 0.01f)
            {
                // Cast rays to detect ground ahead
                Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
                float stepHeight = GetStepHeight(direction);
                
                // Apply movement with step adjustment
                Vector2 targetPosition = rb.position + new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, stepHeight);
                
                rb.MovePosition(targetPosition);
            }

            // Handle character flipping
            if (!isFacingRight && horizontalAxis > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontalAxis < 0f)
            {
                Flip();
            }

            // Wheel animation
            animator.SetFloat("Speed", Mathf.Abs(horizontalAxis));
        }

        private float GetStepHeight(Vector2 direction)
        {
            // Cast multiple rays to detect ground profile
            float[] rayOriginsY = new float[] { -0.4f, -0.2f, 0f };
            float maxHeight = 0f;
            
            foreach (float originY in rayOriginsY)
            {
                Vector2 rayOrigin = rb.position + new Vector2(0, originY);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, 
                    groundCheckDistance * 2f, groundLayer);
                
                if (hit.collider != null)
                {
                    // Check if this is a walkable slope
                    float angle = Vector2.Angle(hit.normal, Vector2.up);
                    if (angle <= maxClimbAngle)
                    {
                        float heightDiff = hit.point.y - rayOrigin.y;
                        if (heightDiff > maxHeight && heightDiff < 0.3f)
                        {
                            maxHeight = heightDiff;
                        }
                    }
                }
            }
            
            return maxHeight;
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        #endregion

        #region Input System Callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!controlsLocked)
            {
                horizontalAxis = context.ReadValue<Vector2>().x;
            }
            
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Add attack logic here
                Debug.Log("Attack action performed");
            }
        }
        #endregion

        #region Lock/Unlock Movement

        public void LockControls()
        {
            controlsLocked = true;
            // Disable input or movement here
        }

        public void UnlockControls()
        {
            controlsLocked = false;
            // Enable input or movement here
        }

        public bool AreControlsLocked()
        {
            return controlsLocked;
        }

        #endregion
    }
}
