using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField] private float speed = 5f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        private Rigidbody2D rb;
        private Animator animator;
        private float horizontalAxis;
        private bool isFacingRight = true;
        private bool isGrounded;
        private bool canJump;
        private bool controlsLocked = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            CheckGround();
            Move();
        }

        private void Move()
        {
            float moveForce = horizontalAxis * speed;
            rb.linearVelocity = new Vector2(moveForce, rb.linearVelocity.y);

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

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        private void CheckGround()
        {
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }
        }

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
