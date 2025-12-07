using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
    public class CombinationLever : MonoBehaviour, IInteractable
    {
        [Header("Lever Type")]
        [SerializeField] private LeverType leverType = LeverType.A;
        [SerializeField] private bool isOne; // For animation only
        
        [Header("Accessibility Visuals")]
        [SerializeField] private GameObject accessibleVisualsPrefab;
        [SerializeField] private float clueRadius = 5f;

        [Header("Enemy Check")]
        [SerializeField] private GameObject linkedEnemy; // Enemy behind the gate
        [SerializeField] private bool resetOnPlayerDeath = true;

        private bool wasActivatedThisLife = false;
        
        private Animator leverAnimator;
        private bool isActive = false;
        private GameObject currentClueInstance;

        public LeverType GetLeverType() => leverType;
        public bool IsActive() => isActive;
        
        private void Awake()
        {
            leverAnimator = GetComponent<Animator>();
            
            if (leverAnimator != null)
            {
                leverAnimator.SetBool("IsOne", isOne);
                leverAnimator.SetBool("IsOn", isActive);
            }
        }
        
        public bool CanInteract()
        {
            return true; // Levers can always be toggled
        }
        
        public void Interact(GameObject interactor)
        {
            ToggleLever();
        }
        
        public void ToggleLever()
        {
            // Check if enemy is alive
            if (linkedEnemy != null && linkedEnemy.activeSelf)
            {
                // Player will die - don't activate lever
                Debug.Log($"Enemy behind lever {leverType} is still alive!");
                return;
                // Add way to simplify this, skipping checkPlayerDeath methtod
            }
            
            isActive = !isActive;
            wasActivatedThisLife = isActive;
            
            // Update animation only
            if (leverAnimator != null)
            {
                leverAnimator.SetBool("IsOn", isActive);
            }
            
            // Notify combination system
            if (GateCombinationSystem.Instance != null)
            {
                GateCombinationSystem.Instance.LeverStateChanged(leverType, isActive);
            }
            
            Debug.Log($"Lever {leverType} toggled to {(isActive ? "ACTIVE" : "INACTIVE")}");
        }

        public void CheckPlayerDeath(GameObject player)
        {
            if (!resetOnPlayerDeath || !wasActivatedThisLife) return;
            
            // If player died and lever was activated this life, reset it
            if (player == null || !player.activeSelf)
            {
                ResetLever();
            }
        }

        public void ResetLever()
        {
            if (isActive)
            {
                isActive = false;
                wasActivatedThisLife = false;
                
                if (leverAnimator != null)
                {
                    leverAnimator.SetBool("IsOn", false);
                }
                
                // Notify combination system
                if (GateCombinationSystem.Instance != null)
                {
                    GateCombinationSystem.Instance.LeverStateChanged(leverType, false);
                }
                
                Debug.Log($"Lever {leverType} reset due to player death");
            }
        }
        
        public void TryShowAccessibleVisuals()
        {
            // Check if player has flashlight or sunglasses
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            bool hasFlashlight = HasItemInInventory(player, "Flashlight");
            bool hasSunglasses = HasItemInInventory(player, "Sunglasses");
            
            if (!hasFlashlight && !hasSunglasses) return;
            
            // Check distance if using flashlight
            if (hasFlashlight)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance > clueRadius)
                {
                    HideAccessibleVisuals();
                    return;
                }
            }
            
            // Sunglasses shows clue regardless of distance, flashlight only within radius
            ShowAccessibleVisuals();
        }
        
        private bool HasItemInInventory(GameObject player, string itemTag)
        {
            // Check if player has the item as a child or component
            Transform item = player.transform.Find(itemTag);
            if (item != null) return true;
            
            // Alternative: Check by tag in children
            foreach (Transform child in player.transform)
            {
                if (child.CompareTag(itemTag))
                    return true;
            }
            
            return false;
        }
        
        private void ShowAccessibleVisuals()
        {
            if (accessibleVisualsPrefab != null && currentClueInstance == null)
            {
                currentClueInstance = Instantiate(accessibleVisualsPrefab, transform.position, Quaternion.identity);
                currentClueInstance.transform.SetParent(transform);
            }
        }
        
        private void HideAccessibleVisuals()
        {
            if (currentClueInstance != null)
            {
                Destroy(currentClueInstance);
                currentClueInstance = null;
            }
        }
        
        private void Update()
        {
            // Check every frame if we should show/hide accessible visuals
            if (accessibleVisualsPrefab != null)
            {
                TryShowAccessibleVisuals();
            }
        }

    }
}