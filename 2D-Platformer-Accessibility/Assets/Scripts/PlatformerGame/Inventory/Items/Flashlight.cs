using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using PlatformerGame.Inventory.Items.AccessibleItems;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Flashlight", menuName = "PlatformerGame/Inventory/Items/Flashlight")]
    public class Flashlight : Item
    {
        [Header("Phase 1: Marker Reveal")]
        [SerializeField] private float revealRadius = 5f;
        
        [Header("Phase 2: Wall Reveal (After Sunglasses)")]
        [SerializeField] private string hiddenTilemapTag = "Reveal";
        [SerializeField] private string hiddenTilemapLayerName = "HiddenWalls";
        [SerializeField] private int wallRevealRadius = 3; // 3 tiles radius

        [Header("Light Settings")]
        [SerializeField] private string playerLightName = "FlashlightLight";
        [SerializeField] private string globalLightName = "GlobalLight2D";
        
        private bool isEquipped = false;
        private bool hasSunglasses = false;
        private Transform playerTransform;
        private Light2D playerLight2D;
        private Light2D globalLight2D;
        private Tilemap hiddenWallTilemap;
        private List<GameObject> revealedTileObjects = new List<GameObject>();
        
        public override void Use(PlayerInventory inventory)
        {
            // Flashlight auto-equips on pickup, this is just for compatibility
        }
        
        public void OnObtained(Transform player)
        {
            playerTransform = player;
            isEquipped = true;

            // Find and store references
            FindTilemapReference();
            FindLightReferences();
            
            // Activate player's flashlight light
            ActivatePlayerLight();

            // Check if sunglasses are already in inventory
            CheckForSunglasses();
            
            Debug.Log("Flashlight obtained - auto-equipped");            
        }

        #region Tilemap Manager

        private void FindTilemapReference()
        {
            // Method 1: Find by tag
            GameObject tilemapGO = GameObject.FindGameObjectWithTag(hiddenTilemapTag);
            if (tilemapGO != null)
            {
                hiddenWallTilemap = tilemapGO.GetComponent<Tilemap>();
                if (hiddenWallTilemap != null)
                {
                    Debug.Log($"Found hidden tilemap by tag: {tilemapGO.name}");
                }
                else
                {
                    Debug.LogWarning($"Found GameObject with tag '{hiddenTilemapTag}' but no Tilemap component");
                }
            }
            
            // Method 2: Find by layer name (fallback)
            if (hiddenWallTilemap == null)
            {
                Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
                foreach (Tilemap tm in allTilemaps)
                {
                    if (tm.gameObject.layer == LayerMask.NameToLayer(hiddenTilemapLayerName))
                    {
                        hiddenWallTilemap = tm;
                        Debug.Log($"Found hidden tilemap by layer: {tm.gameObject.name}");
                        break;
                    }
                }
            }
            
            // Method 3: Find by name (final fallback)
            if (hiddenWallTilemap == null)
            {
                GameObject tmGO = GameObject.Find("HiddenWalls");
                if (tmGO != null)
                {
                    hiddenWallTilemap = tmGO.GetComponent<Tilemap>();
                    if (hiddenWallTilemap != null)
                    {
                        Debug.Log($"Found hidden tilemap by name: {tmGO.name}");
                    }
                }
            }
            
            if (hiddenWallTilemap == null)
            {
                Debug.LogWarning("Could not find hidden wall tilemap!");
            }
        }       
        #endregion

        #region Light Manager

        private void FindLightReferences()
        {
            // Find player's flashlight light
            if (playerTransform != null)
            {
                // Look for a child light
                foreach (Transform child in playerTransform.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name.Contains(playerLightName))
                    {
                        playerLight2D = child.GetComponent<Light2D>();
                        if (playerLight2D != null)
                        {
                            Debug.Log($"Found player light: {child.name}");
                            break;
                        }
                    }
                }
                
                // If not found, try to find any Light2D on player
                if (playerLight2D == null)
                {
                    playerLight2D = playerTransform.GetComponentInChildren<Light2D>(true);
                    if (playerLight2D != null)
                    {
                        Debug.Log($"Found player light (any): {playerLight2D.name}");
                    }
                }
            }
            
            // Find global light
            GameObject globalLightGO = GameObject.Find(globalLightName);
            if (globalLightGO != null)
            {
                globalLight2D = globalLightGO.GetComponent<Light2D>();
                if (globalLight2D != null)
                {
                    Debug.Log($"Found global light: {globalLightGO.name}");
                }
            }
            
            if (globalLight2D == null)
            {
                // Search for any global light with Light2D component
                Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
                foreach (Light2D light in allLights)
                {
                    if (light.lightType == Light2D.LightType.Global)
                    {
                        globalLight2D = light;
                        Debug.Log($"Found global light (by type): {light.name}");
                        break;
                    }
                }
            }
        }

        private void ActivatePlayerLight()
        {
            if (playerLight2D != null)
            {
                playerLight2D.enabled = true;
                Debug.Log("Player flashlight light activated");
            }
            else
            {
                Debug.LogWarning("Player flashlight light not found!");
            }
        }
        
        private void ActivateGlobalLight()
        {
            if (globalLight2D != null)
            {
                globalLight2D.enabled = true;
                Debug.Log("Global light activated");
            }
            else
            {
                Debug.LogWarning("Global light not found!");
            }
        }
        #endregion

        #region Sunglasses Logic
        
        private void CheckForSunglasses()
        {
            if (playerTransform == null) return;
            
            PlayerInventory inventory = playerTransform.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                hasSunglasses = inventory.HasSunglasses();
                if (hasSunglasses)
                {
                    Debug.Log("Flashlight upgraded: Player has sunglasses");
                    // Activate global light when sunglasses are in inventory
                    ActivateGlobalLight();
                    UpdateFlashlight();
                }
            }
            else
            {
                hasSunglasses = false;
            }
        }
        
        // Called from player's Update loop
        public void UpdateFlashlight()
        {
            if (!isEquipped || playerTransform == null) return;
            
            if (hasSunglasses)
            {
                // Phase 2: Reveal hidden walls and all markers
                RevealAllMarkers();
                //RevealHiddenWalls();
                RevealHiddenWallsInRadius();
            }
            else
            {
                // Phase 1: Reveal markers within radius
                RevealMarkersInRadius();
            }
        }

        private void RevealMarkersInRadius()
        {
            // Handled by Gate Marker script
        }

        private void RevealAllMarkers()
        {
            // Handled by Gate Marker script
        }
        
        private void RevealHiddenWallsInRadius()
        {
            if (hiddenWallTilemap == null) 
            {
                // Try to find tilemap again if null
                FindTilemapReference();
                if (hiddenWallTilemap == null) return;
            }
            
            // Get player's tile position
            Vector3Int playerTilePos = hiddenWallTilemap.WorldToCell(playerTransform.position);
            
            // Clean up previously revealed tiles outside radius
            CleanupRevealedTiles(playerTilePos);
            
            // Check tiles around player
            for (int x = -wallRevealRadius; x <= wallRevealRadius; x++)
            {
                for (int y = -wallRevealRadius; y <= wallRevealRadius; y++)
                {
                    Vector3Int tilePos = new Vector3Int(playerTilePos.x + x, playerTilePos.y + y, 0);
                    
                    // Check if tile exists
                    if (hiddenWallTilemap.HasTile(tilePos))
                    {
                        RevealTile(tilePos);
                    }
                }
            }
        }
        
        private void RevealTile(Vector3Int tilePos)
        {
            // Get world position of the tile
            Vector3 worldPos = hiddenWallTilemap.CellToWorld(tilePos) + hiddenWallTilemap.cellSize * 0.5f;
            
            // Create a visual representation of the revealed tile
            GameObject revealedTile = new GameObject($"RevealedTile_{tilePos.x}_{tilePos.y}");
            revealedTile.transform.position = worldPos;
            revealedTile.transform.SetParent(hiddenWallTilemap.transform);
            
            // Add a sprite renderer to show the tile
            SpriteRenderer sr = revealedTile.AddComponent<SpriteRenderer>();
            TileBase tile = hiddenWallTilemap.GetTile(tilePos);
            
            if (tile is Tile)
            {
                sr.sprite = ((Tile)tile).sprite;
            }
            else if (tile is RuleTile)
            {
                // For RuleTiles, we need to get the default sprite
                sr.sprite = hiddenWallTilemap.GetSprite(tilePos);
            }
            
            sr.sortingLayerName = hiddenWallTilemap.GetComponent<TilemapRenderer>().sortingLayerName;
            sr.sortingOrder = hiddenWallTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
            
            // Add collider if the original tile had one
            // (You can check if you want to add colliders to revealed tiles)
            
            revealedTileObjects.Add(revealedTile);
            
            // Optional: Remove the tile from the hidden tilemap
            // hiddenWallTilemap.SetTile(tilePos, null);
            
            Debug.Log($"Revealed hidden wall tile at {tilePos}");
        }
        
        private void CleanupRevealedTiles(Vector3Int playerTilePos)
        {
            // Remove tiles that are too far away
            for (int i = revealedTileObjects.Count - 1; i >= 0; i--)
            {
                if (revealedTileObjects[i] == null)
                {
                    revealedTileObjects.RemoveAt(i);
                    continue;
                }
                
                Vector3Int tilePos = hiddenWallTilemap.WorldToCell(revealedTileObjects[i].transform.position);
                int distanceX = Mathf.Abs(tilePos.x - playerTilePos.x);
                int distanceY = Mathf.Abs(tilePos.y - playerTilePos.y);
                
                if (distanceX > wallRevealRadius || distanceY > wallRevealRadius)
                {
                    Destroy(revealedTileObjects[i]);
                    revealedTileObjects.RemoveAt(i);
                }
            }
        }
        
        // Alternative method: Disable the entire tilemap (simpler approach)
        public void RevealAllHiddenWalls()
        {
            if (hiddenWallTilemap != null)
            {
                // Disable the tilemap renderer (hides the walls)
                TilemapRenderer renderer = hiddenWallTilemap.GetComponent<TilemapRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                    Debug.Log("Hidden walls tilemap renderer disabled");
                }
                
                // Also disable colliders if they exist
                TilemapCollider2D collider = hiddenWallTilemap.GetComponent<TilemapCollider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
                
                CompositeCollider2D compositeCollider = hiddenWallTilemap.GetComponent<CompositeCollider2D>();
                if (compositeCollider != null)
                {
                    compositeCollider.enabled = false;
                }
            }
        }
        #endregion

        #region Clean Up

        // Cleanup method
        public void OnUnequip()
        {
            isEquipped = false;
            
            // Deactivate player light
            if (playerLight2D != null)
            {
                playerLight2D.enabled = false;
            }
            
            // Clean up revealed tiles
            foreach (GameObject tileObj in revealedTileObjects)
            {
                if (tileObj != null)
                {
                    Destroy(tileObj);
                }
            }
            revealedTileObjects.Clear();
        }
        #endregion
        
        public override void OnAddToInventory(PlayerInventory inventory)
        {
            // Auto-equip when added to inventory
            OnObtained(inventory.transform);
        }
    }
}