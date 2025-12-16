using UnityEngine;
using UnityEngine.Tilemaps;
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
        [SerializeField] private Tilemap hiddenWallTilemap;
        [SerializeField] private Tile replacementTile;
        [SerializeField] private int wallRevealRadius = 2; // 2 tiles radius
        
        private bool isEquipped = false;
        private bool hasSunglasses = false;
        private Transform playerTransform;
        private HashSet<Vector3Int> revealedTiles = new HashSet<Vector3Int>();
        
        public override void Use(PlayerInventory inventory)
        {
            // Flashlight auto-equips on pickup, this is just for compatibility
        }
        
        public void OnObtained(Transform player)
        {
            playerTransform = player;
            isEquipped = true;
            
            // Check if sunglasses are already in inventory
            CheckForSunglasses();
            
            Debug.Log("Flashlight obtained - auto-equipped");
        }
        
        private void CheckForSunglasses()
        {
            if (playerTransform == null) return;
            
            PlayerInventory inventory = playerTransform.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                foreach (Item item in inventory.Items)
                {
                    if (item is Sunglasses)
                    {
                        hasSunglasses = true;
                        Debug.Log("Flashlight upgraded: Player has sunglasses");
                        return;
                    }
                }
            }
            hasSunglasses = false;
        }
        
        public void OnSunglassesObtained()
        {
            hasSunglasses = true;
            Debug.Log("Flashlight upgraded by sunglasses");
        }
        
        // Called from player's Update loop
        public void UpdateFlashlight()
        {
            if (!isEquipped || playerTransform == null) return;
            
            if (hasSunglasses)
            {
                // Phase 2: Reveal hidden walls and all markers
                RevealAllMarkers();
                RevealHiddenWalls();
            }
            else
            {
                // Phase 1: Reveal markers within radius
                RevealMarkersInRadius();
            }
        }
        
        private void RevealMarkersInRadius()
        {
            // Find all gate markers
            GateMarker[] allMarkers = FindObjectsByType<GateMarker>(FindObjectsSortMode.None);
            
            foreach (GateMarker marker in allMarkers)
            {
                if (marker == null) continue;
                
                float distance = Vector2.Distance(playerTransform.position, marker.transform.position);
                if (distance <= revealRadius)
                {
                    marker.ShowMarker();
                }
                else
                {
                    marker.HideMarker();
                }
            }
        }
        
        private void RevealAllMarkers()
        {
            // Show all markers in the scene
            GateMarker[] allMarkers = FindObjectsByType<GateMarker>(FindObjectsSortMode.None);
            foreach (GateMarker marker in allMarkers)
            {
                if (marker != null) marker.ShowMarker();
            }
        }
        
        private void RevealHiddenWalls()
        {
            if (hiddenWallTilemap == null) return;
            
            // Get player's tile position
            Vector3Int playerTilePos = hiddenWallTilemap.WorldToCell(playerTransform.position);
            
            // Check tiles around player
            for (int x = -wallRevealRadius; x <= wallRevealRadius; x++)
            {
                for (int y = -wallRevealRadius; y <= wallRevealRadius; y++)
                {
                    Vector3Int tilePos = new Vector3Int(playerTilePos.x + x, playerTilePos.y + y, 0);
                    
                    // Skip if already revealed
                    if (revealedTiles.Contains(tilePos)) continue;
                    
                    // Check if tile exists and has "reveal" tag
                    TileBase tile = hiddenWallTilemap.GetTile(tilePos);
                    if (tile != null)
                    {
                        // Check tile name or use a custom tile with tag property
                        if (tile.name.Contains("reveal"))
                        {
                            RevealTile(tilePos);
                        }
                    }
                }
            }
        }
        private void RevealTile(Vector3Int tilePos)
        {
            if (hiddenWallTilemap == null || replacementTile == null) return;
            
            // Replace the hidden tile with the revealed tile
            hiddenWallTilemap.SetTile(tilePos, replacementTile);
            revealedTiles.Add(tilePos);
            
            Debug.Log($"Revealed hidden wall at {tilePos}");
        }
        
        public override void OnAddToInventory(PlayerInventory inventory)
        {
            // Auto-equip when added to inventory
            OnObtained(inventory.transform);
        }
    }
}