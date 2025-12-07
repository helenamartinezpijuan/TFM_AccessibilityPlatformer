using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class GateMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private LeverType requiredLever;
        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private Vector2 markerOffset = new Vector2(0, 1f);
        
        private GameObject currentMarker;
        private bool isVisible = false;
        public LeverType GetRequiredLever() => requiredLever;
        
        public void ShowMarker()
        {
            if (markerPrefab != null && !isVisible)
            {
                Vector3 spawnPosition = transform.position + (Vector3)markerOffset;
                currentMarker = Instantiate(markerPrefab, spawnPosition, Quaternion.identity);
                currentMarker.transform.SetParent(transform);
                isVisible = true;
            }
        }
        
        public void HideMarker()
        {
            if (currentMarker != null)
            {
                Destroy(currentMarker);
                currentMarker = null;
            }
            isVisible = false;
        }
    }
}