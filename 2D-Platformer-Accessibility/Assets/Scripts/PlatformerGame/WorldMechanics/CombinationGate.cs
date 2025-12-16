using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.Inventory.Items.AccessibleItems;

namespace PlatformerGame.WorldMechanics
{
    public enum LeverType
    {
        A, B, C, D, E, F, G, H
    }
    
    public class CombinationGate : MonoBehaviour
    {
        [Header("Gate Settings")]
        [SerializeField] private string gateId = "Gate1";
        [SerializeField] private List<LeverType> requiredLeverTypes = new List<LeverType>();
        [SerializeField] private bool requireExactMatch = true;
        [SerializeField] private bool isOpen;
        
        [Header("Accessibility Visuals")]
        [SerializeField] private float clueRadius = 5f;
        
        [Header("Gate Markers")]
        [SerializeField] private List<GateMarker> gateMarkers = new List<GateMarker>();
        
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D gateCollider;
        private GameObject currentClueInstance;
        
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            gateCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            
            // Register this gate with the combination system
            GateCombinationSystem.Instance.RegisterGate(this);

            // Hide all markers at the beginning of the level
            foreach (GateMarker marker in gateMarkers)
            {
                marker.HideMarker();
            }
            
            UpdateGateState();
        }
        
        public void CheckCombination(HashSet<LeverType> activeLeverTypes)
        {
            bool shouldOpen;
            
            if (requireExactMatch)
            {
                // Exact match: Only the required lever types can be active, no others
                shouldOpen = activeLeverTypes.SetEquals(new HashSet<LeverType>(requiredLeverTypes));
            }
            else
            {
                // Contains all required lever types, but others can also be active
                shouldOpen = requiredLeverTypes.TrueForAll(leverType => activeLeverTypes.Contains(leverType));
            }
            
            if (shouldOpen != isOpen)
            {
                isOpen = shouldOpen;
                UpdateGateState();
            }
        }
        
        private void UpdateGateState()
        {
            if (gateCollider != null)
            {
                gateCollider.enabled = !isOpen;
                animator.SetBool("isOpen", !isOpen);
                //spriteRenderer.enabled = !isOpen;
            }
            
            Debug.Log($"Gate {gateId} is now {(isOpen ? "OPEN" : "CLOSED")}");
        }
        
        public void TryShowAccessibleVisuals()
        {
            // Check if player has flashlight or sunglasses
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            bool hasFlashlight = HasItemInInventory(player, "Flashlight");
            bool hasSunglasses = HasItemInInventory(player, "Sunglasses");

            Debug.Log("Player has flashlight: " + hasFlashlight + ", has sunglasses: " + hasSunglasses);
            
            if (!hasFlashlight && !hasSunglasses) return;
            
            // Check distance if using flashlight
            if (hasFlashlight)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance <= clueRadius)
                {
                    foreach (GateMarker gateMarker in gateMarkers)
                    {
                        gateMarker.ShowMarker();
                    }
                }
                else
                {
                    foreach (GateMarker gateMarker in gateMarkers)
                    {
                        gateMarker.HideMarker();
                    }
                    return;
                }
            }
            
            // Sunglasses shows clue regardless of distance, flashlight only within radius
            foreach (GateMarker gateMarker in gateMarkers)
            {
                gateMarker.ShowMarker();
            }
        }
        
        private bool HasItemInInventory(GameObject player, string itemTag)
        {
            // Check if player has the item as a child or component
            Transform item = player.transform.Find(itemTag);
            if (item != null) return true;
            
            return false;
        }

        public string GetGateId() => gateId;
        public List<LeverType> GetRequiredLeverTypes() => requiredLeverTypes;
    }
}
