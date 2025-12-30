using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField] private float speed = 5f;

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
        }

        public void UnlockControls()
        {
            controlsLocked = false;
        }

        public bool AreControlsLocked()
        {
            return controlsLocked;
        }

        #endregion
    }
}
