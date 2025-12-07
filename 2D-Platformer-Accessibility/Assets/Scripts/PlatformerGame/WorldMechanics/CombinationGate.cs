using System;
using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.Inventory;
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
        
        [Header("Gate Markers")]
        [SerializeField] private List<GateMarker> gateMarkers = new List<GateMarker>();

        [Header("Marker Settings")]
        [SerializeField] private bool showMarkersWhenFlashlightNear = true;
        [SerializeField] private float flashlightDetectionRadius = 2f;
        
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D gateCollider;
        private GateMarker gateMarker;

        private bool isOpen = false;
        private bool markersVisible = false;
        private GameObject currentClueInstance;

        public string GetGateId() => gateId;
        public List<LeverType> GetRequiredLeverTypes() => requiredLeverTypes;
        
        private void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            gateCollider = GetComponentInChildren<BoxCollider2D>();
            gateMarker = GetComponentInChildren<GateMarker>();
            
            // Register this gate with the combination system
            GateCombinationSystem.Instance.RegisterGate(this);

            // Hide all markers at the beginning of the level
            foreach (GateMarker marker in gateMarkers)
            {
                marker.HideMarkers();
            }
        }

        private void Update()
        {
            // Flashlight proximity detection
            if (showMarkersWhenFlashlightNear && gateMarker != null)
            {
                CheckFlashlightProximity();
            }
        }

        private void CheckFlashlightProximity()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float distance;
            if (player != null) 
            {
                distance = Vector2.Distance(transform.position, player.transform.position);
            }
            else
            {
                distance = flashlightDetectionRadius + 1f;;
            }

            // Check if player has flashlight equipped
            bool hasFlashlight = HasItemInInventory(player, "Advanced Flashlight");
            bool hasSunglasses = HasItemInInventory(player, "Sunglasses");
            
            // Determine if markers should be visible
            bool shouldShowMarkers = false;
            
            if (hasSunglasses)
            {
                // Sunglasses: Always show markers
                shouldShowMarkers = true;
            }
            else if (hasFlashlight && distance <= flashlightDetectionRadius)
            {
                // Flashlight: Show only when within radius
                shouldShowMarkers = true;
            }
            
            // Update marker visibility if changed
            if (shouldShowMarkers != markersVisible)
            {
                markersVisible = shouldShowMarkers;
                
                if (markersVisible)
                {
                    gateMarker.ShowMarkers();
                }
                else
                {
                    gateMarker.HideMarkers();
                }
            }
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
            }
        }
         
        private bool HasItemInInventory(GameObject player, string itemTag)
        {
            // Check if player has the item in Inventory
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                foreach (Item item in inventory.Items)
                {
                    if (item != null && item.itemName.Contains(itemTag))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
