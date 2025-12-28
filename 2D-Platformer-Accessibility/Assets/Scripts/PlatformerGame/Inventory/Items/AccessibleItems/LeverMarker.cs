using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;
using UnityEngine.Rendering.Universal;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class LeverMarker : MonoBehaviour
    {
        [Header("Marker Settings")]
        [SerializeField] private LeverType requiredLever;
        [SerializeField] private Sticker sticker;

        private SpriteRenderer markerSprite;
        private Animator markerAnimator;
        private Light2D markerLight;
        
        public LeverType GetRequiredLever() => requiredLever;
        
        private void Start()
        {
            markerSprite = GetComponent<SpriteRenderer>();
            markerAnimator = GetComponent<Animator>();
            markerLight = GetComponent<Light2D>();

            // Start hidden
            if (markerSprite != null)
                markerSprite.enabled = false;

            if (markerAnimator != null)
                markerAnimator.enabled = false;
            
            if (markerLight != null)
                markerLight.enabled = false;
        }
        
        #region Trigger Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                bool hasFlahslight = other.GetComponent<PlayerInventory>().HasFlashlight();
                bool hasSticker = other.GetComponent<PlayerInventory>().HasSticker(sticker);

                if (hasFlahslight && hasSticker)
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
            if (markerSprite != null && !markerSprite.enabled)
            {
                markerSprite.enabled = true;
                markerAnimator.enabled = true;
                markerLight.enabled = true;
            }
        }
        
        public void HideMarker()
        {
            if (markerSprite != null && markerSprite.enabled)
            {
                markerSprite.enabled = false;
                markerAnimator.enabled = false;
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