using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;
using UnityEngine.Rendering.Universal;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class DoorMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private int doorNumber;

        private SpriteRenderer markerSprite;
        private Light2D markerLight;
        
        private void Start()
        {
            markerSprite = GetComponent<SpriteRenderer>();
            markerLight = GetComponent<Light2D>();

            // Start hidden
            if (markerSprite != null)
                markerSprite.enabled = false;
            
            if (markerLight != null)
                markerLight.enabled = false;
        }
        
        #region Trigger Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                bool hasFlahslight = other.GetComponent<PlayerInventory>().HasFlashlight();
                bool hasNumberSticker = other.GetComponent<PlayerInventory>().HasNumberStickers();

                if (hasFlahslight && hasNumberSticker)
                {
                    ShowMarker();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HideMarker();
            }
        }
        #endregion

        #region Enable/Disable Markers
        
        public void ShowMarker()
        {
            if (markerSprite != null && !markerSprite.enabled)
            {
                markerSprite.enabled = true;
                markerLight.enabled = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null && markerSprite.enabled)
            {
                markerSprite.enabled = false;
                markerLight.enabled = false;
            }
        }

        public void ShowAllMarkers()
        {
            Destroy(GetComponent<CircleCollider2D>());
            ShowMarker();
        }
    }
    #endregion
}