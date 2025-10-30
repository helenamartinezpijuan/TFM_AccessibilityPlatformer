using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 12f;

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

            // Apply jump if requested
            if (canJump && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canJump = false;
            }
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

        // Input System Callbacks
        public void OnMove(InputAction.CallbackContext context)
        {
            horizontalAxis = context.ReadValue<Vector2>().x;
            Debug.Log($"Move input received: {horizontalAxis}");
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                canJump = true;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Add interaction logic here
                Debug.Log("Interact action performed");
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
    }
}
