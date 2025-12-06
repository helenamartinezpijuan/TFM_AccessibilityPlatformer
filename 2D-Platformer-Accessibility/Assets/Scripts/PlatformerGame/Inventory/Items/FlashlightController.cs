using UnityEngine;

namespace PlatformerGame.Inventory.Items
{
    public class FlashlightController : MonoBehaviour
    {/*
        [Header("Components")]
        [SerializeField] private SpriteRenderer lightConeRenderer;
        [SerializeField] private Transform rayOrigin;
        
        private Flashlight flashlightData;
        private bool isOn = false;
        private GameObject currentRevealedObject;
        private Vector2 lastDirection = Vector2.right;

        public bool IsActive => isOn;

        public void Initialize(Flashlight data)
        {
            flashlightData = data;
            TurnOn();
        }

        private void Update()
        {
            if (!isOn) return;

            HandleDirection();
            CastRay();
            UpdateLightCone();
        }

        private void HandleDirection()
        {
            // Get input direction (you might want to use your input system)
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 direction = Vector2.zero;
            
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                direction.x = Mathf.Sign(horizontal);
                direction.y = 0;
            }
            else if (Mathf.Abs(vertical) > 0.1f)
            {
                direction.x = 0;
                direction.y = Mathf.Sign(vertical);
            }

            if (direction != Vector2.zero)
            {
                lastDirection = direction.normalized;
            }

            // Rotate light cone based on direction
            if (lightConeRenderer != null)
            {
                float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
                lightConeRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void CastRay()
        {
            if (rayOrigin == null) return;

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin.position, 
                lastDirection, 
                flashlightData.rayDistance, 
                flashlightData.interactableLayers
            );

            // Debug visualization
            Debug.DrawRay(rayOrigin.position, lastDirection * flashlightData.rayDistance, 
                         hit.collider != null ? Color.green : Color.red);

            if (hit.collider != null)
            {
                HandleHitObject(hit.collider.gameObject);
            }
            else
            {
                ClearRevealedObject();
            }
        }

        private void HandleHitObject(GameObject hitObject)
        {
            // Check if it's the same object
            if (currentRevealedObject == hitObject) return;

            // Clear previous object
            ClearRevealedObject();

            // Set new object
            currentRevealedObject = hitObject;
            
            // Try to reveal the object
            SwitchVision revealable = hitObject.GetComponent<SwitchVision>();
            if (revealable != null)
            {
                revealable.Reveal(flashlightData.revealedSprite);
                
                // Spawn particle effect
                if (flashlightData.revealParticleEffect != null)
                {
                    Instantiate(flashlightData.revealParticleEffect, 
                               hitObject.transform.position, 
                               Quaternion.identity);
                }
            }
        }

        private void ClearRevealedObject()
        {
            if (currentRevealedObject != null)
            {
                SwitchVision revealable = currentRevealedObject.GetComponent<SwitchVision>();
                if (revealable != null)
                {
                    revealable.Hide();
                }
                currentRevealedObject = null;
            }
        }

        private void UpdateLightCone()
        {
            if (lightConeRenderer != null)
            {
                lightConeRenderer.enabled = isOn;
                
                // Optional: Adjust alpha based on distance to objects
                float alpha = 0.7f;
                lightConeRenderer.color = new Color(1, 1, 1, alpha);
            }
        }

        public void TurnOn()
        {
            isOn = true;
            if (lightConeRenderer != null)
            {
                lightConeRenderer.enabled = true;
            }
        }

        public void TurnOff()
        {
            isOn = false;
            ClearRevealedObject();
            if (lightConeRenderer != null)
            {
                lightConeRenderer.enabled = false;
            }
        }

        private void OnDestroy()
        {
            ClearRevealedObject();
        }*/
    }
}