using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class ButtonMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private BeamEffectController beamEffect;
        [SerializeField] private string connectionName;

        private void Start()
        {
            // Start hidden
            HideMarker();
        }

        #region Trigger Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                bool hasFlahslight = other.GetComponent<PlayerInventory>().HasFlashlight();

                if (hasFlahslight)
                {
                    ShowMarker();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                bool hasFlahslight = other.GetComponent<PlayerInventory>().HasFlashlight();
                
                if (hasFlahslight)
                {
                    HideMarker();
                }
            }
        }
        #endregion

        #region Enable/Disable Markers
        
        public void ShowMarker()
        {
            if(beamEffect != null)
                beamEffect.ActivateBeam(connectionName);
        }
        
        public void HideMarker()
        {
            if(beamEffect != null)
                beamEffect.DeactivateBeam(connectionName);
        }

        public void ShowAllMarkers()
        {
            Destroy(GetComponent<CircleCollider2D>());
            ShowMarker();
        }
    }
    #endregion
}