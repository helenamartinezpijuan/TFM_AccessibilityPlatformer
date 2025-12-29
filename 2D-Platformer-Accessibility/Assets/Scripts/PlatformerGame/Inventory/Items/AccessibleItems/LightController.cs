using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class LightController : MonoBehaviour
    {
        [Header("Light Settings")]
        [SerializeField] private bool isGlobalLight = false;
        [SerializeField] private ItemEvents itemEvents;

        private Light2D playerLight2D;
        private Light2D globalLight2D;

        private void Start()
        {
            // Register event listeners
            if (itemEvents != null)
            {
                if (isGlobalLight)
                {
                    itemEvents.OnSunglassesObtained += ActivateGlobalLight;
                }
                else
                {
                    itemEvents.OnFlashlightObtained += ActivatePlayerLight;
                }
            }
            
            // Initialize lights as disabled
            if(!isGlobalLight)
            {
                playerLight2D = GetComponent<Light2D>();

                if (playerLight2D != null)
                    playerLight2D.enabled = false;
            }
            else
            {
                globalLight2D = GetComponent<Light2D>();

                if (playerLight2D != null)
                    playerLight2D.enabled = false;
            }  
        }

        private void OnDestroy()
        {
            // Clean up event listeners
            if (itemEvents != null)
            {
                if (isGlobalLight)
                {
                    itemEvents.OnSunglassesObtained -= ActivateGlobalLight;
                }
                else
                {
                    itemEvents.OnFlashlightObtained -= ActivatePlayerLight;
                }
            }
        }

        #region Light Manager

        private void ActivatePlayerLight()
        {
            if (playerLight2D == null)
                playerLight2D = GetComponent<Light2D>();

            if (playerLight2D != null)
            {
                playerLight2D.enabled = true;
                playerLight2D.color = Color.white;
                Debug.Log("Player flashlight light activated");
            }
            else
            {
                Debug.LogWarning("Player flashlight light not found!");
            }
        }
        
        private void ActivateGlobalLight()
        {
            if (globalLight2D == null)
                globalLight2D = GetComponent<Light2D>();
            
            if (globalLight2D != null)
            {
                globalLight2D.enabled = true;
                globalLight2D.color = Color.white;
                Debug.Log("Global light activated");
            }
            else
            {
                Debug.LogWarning("Global light not found!");
            }
        }
        #endregion
        
        // Kept for backward compatibility if called elsewhere
        public void OnSunglassesObtained(Transform player)
        {
            if(isGlobalLight)
                ActivateGlobalLight();
            else
                ActivatePlayerLight();      
        }
    }
}