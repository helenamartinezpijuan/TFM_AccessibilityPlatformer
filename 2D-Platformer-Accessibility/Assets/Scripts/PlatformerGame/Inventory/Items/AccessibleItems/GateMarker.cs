using System;
using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class GateMarker : MonoBehaviour
    {        
        [Header("Marker Settings")]
        [SerializeField] private List<GameObject> leverMarkers = new List<GameObject>();
                
        private void Awake()
        {
            // Hide all markers initially
            HideMarkers();
        }
        
        public void ShowMarkers()
        {
            foreach (var marker in leverMarkers)
            {
                marker.SetActive(true);
                Debug.Log($"Showing markers for: {this.name}");
            }
        }
        
        public void HideMarkers()
        {
            foreach (var marker in leverMarkers)
            {
                marker.SetActive(false);
                Debug.Log($"Hiding markers for: {this.name}");
            }
        }
    }
}