using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace PlatformerGame.Inventory.Items.AccessibleItems
{
    public class HiddenWalls : MonoBehaviour
    {
        [SerializeField] private ItemEvents itemEvents;
        
        private bool hasBeenRevealed = false;

        private void Start()
        {
            if (itemEvents != null)
            {
                itemEvents.OnHiddenWallsShouldReveal += RevealHiddenWall;
            }
        }

        private void OnDestroy()
        {
            if (itemEvents != null)
            {
                itemEvents.OnHiddenWallsShouldReveal -= RevealHiddenWall;
            }
        }

       /*#region Trigger Detection

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hasBeenRevealed && other.CompareTag("Player"))
            {
                bool hasSunglasses = other.GetComponent<PlayerInventory>().HasSunglasses();

                if (hasSunglasses)
                {
                    RevealHiddenWall();
                }
            }
        }
        #endregion*/

        public void RevealHiddenWall()
        {
            if (hasBeenRevealed) return;
            
            hasBeenRevealed = true;
            
            // Disable the tilemap renderer (hides the walls)
            TilemapRenderer renderer = GetComponent<TilemapRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
                Debug.Log("Hidden walls tilemap renderer disabled");
            }
            
            // Disable collider
            TilemapCollider2D collider = GetComponent<TilemapCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
}