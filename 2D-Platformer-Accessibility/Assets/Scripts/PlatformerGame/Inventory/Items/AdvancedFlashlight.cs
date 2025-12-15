using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "AdvancedFlashlight", menuName = "PlatformerGame/Inventory/Items/AdvancedFlashlight")]
    public class AdvancedFlashlight : Item
    {
        [Header("Phase 1: Clue Revealing")]
        public float revealRadius = 2f;
        
        [Header("Phase 2: Wall Revealing (After Sunglasses)")]
        public GameObject newBulbPrefab;
        public LayerMask invisibleWallLayer;
        public Tilemap invisibleWallTilemap;
        
        [Header("Sticker Rewards")]
        public List<GameObject> hiddenStickerPrefabs;
        
        private bool hasSunglasses = false;
        private bool isEquipped = false;
        private Transform playerTransform;
        private Coroutine revealCoroutine;
        private List<GameObject> revealedClues = new List<GameObject>();
        private List<GameObject> revealedWalls = new List<GameObject>();
        public bool IsEquipped => isEquipped;
        
        public override void Use(Inventory inventory)
        {
            if (!isEquipped)
            {
                EquipFlashlight(inventory.transform);
            }
            else
            {
                UnequipFlashlight();
            }
        }
        
        private void EquipFlashlight(Transform player)
        {
            playerTransform = player;
            isEquipped = true;
            
            // Check if player has sunglasses
            hasSunglasses = CheckForSunglasses(player);
            
            if (hasSunglasses)
            {
                // Phase 2: Reveal invisible walls
                MonoBehaviour playerMonoBehaviour = player.GetComponent<MonoBehaviour>();
                if (playerMonoBehaviour != null)
                {
                    revealCoroutine = playerMonoBehaviour.StartCoroutine(RevealInvisibleWallsRoutine());
                }
                Debug.Log("Advanced Flashlight equipped - Can see invisible walls");
            }
            else
            {
                // Phase 1: Original functionality
                MonoBehaviour playerMonoBehaviour = player.GetComponent<MonoBehaviour>();
                if (playerMonoBehaviour != null)
                {
                    revealCoroutine = playerMonoBehaviour.StartCoroutine(RevealCluesRoutine());
                }
                Debug.Log("Flashlight equipped - Can see clues");
            }
        }
        
        private bool CheckForSunglasses(Transform player)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                foreach (Item item in inventory.Items)
                {
                    if (item is Sunglasses sunglasses && sunglasses.IsEquipped)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private IEnumerator RevealCluesRoutine()
        {
            // Original clue revealing logic
            while (isEquipped && !hasSunglasses)
            {
                CheckForNearbyClues();
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator RevealInvisibleWallsRoutine()
        {
            // New invisible wall revealing logic
            while (isEquipped && hasSunglasses)
            {
                CheckForInvisibleWalls();
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void CheckForNearbyClues()
        {
            if (playerTransform == null) return;
            
            // Clear old clues outside radius
            ClearDistantClues();
            
            // Find all clue revealers within radius
            ClueRevealer[] allClueRevealers = FindObjectsByType<ClueRevealer>(FindObjectsSortMode.None);
            
            foreach (ClueRevealer clueRevealer in allClueRevealers)
            {
                if (clueRevealer == null) continue;
                
                float distance = Vector2.Distance(playerTransform.position, clueRevealer.transform.position);
                
                if (distance <= revealRadius)
                {
                    // Check if this clue is already revealed
                    if (!clueRevealer.IsRevealed)
                    {
                        clueRevealer.RevealClue();
                        revealedClues.Add(clueRevealer.gameObject);
                    }
                }
                else if (distance > revealRadius && !clueRevealer.revealOnlyWithSunglasses)
                {
                    // Hide clues that are outside radius (unless they require sunglasses)
                    clueRevealer.HideClue();
                    revealedClues.Remove(clueRevealer.gameObject);
                }
            }
        }
        
        private void CheckForInvisibleWalls()
        {
            if (playerTransform == null || invisibleWallTilemap == null) return;
            
            // Get player cell position
            Vector3Int playerCell = invisibleWallTilemap.WorldToCell(playerTransform.position);
            
            // Check surrounding cells
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    Vector3Int checkCell = new Vector3Int(playerCell.x + x, playerCell.y + y, playerCell.z);
                    
                    if (invisibleWallTilemap.HasTile(checkCell))
                    {
                        // Reveal this tile
                        RevealTile(checkCell);
                        
                        // Check if this tile contains a sticker
                        Vector3 worldPos = invisibleWallTilemap.CellToWorld(checkCell) + new Vector3(0.5f, 0.5f, 0);
                        CheckForHiddenSticker(worldPos);
                    }
                }
            }
        }
        
        private void RevealTile(Vector3Int cellPosition)
        {
            // Remove tile and collider
            invisibleWallTilemap.SetTile(cellPosition, null);
            
            // Add to revealed list for cleanup
            if (!revealedWalls.Contains(invisibleWallTilemap.gameObject))
            {
                revealedWalls.Add(invisibleWallTilemap.gameObject);
            }
        }
        
        private void CheckForHiddenSticker(Vector3 position)
        {
            // Spawn a random sticker from hidden stickers
            if (hiddenStickerPrefabs.Count > 0)
            {
                //int randomIndex = Random.Range(0, hiddenStickerPrefabs.Count);
                GameObject sticker = Instantiate(hiddenStickerPrefabs[0], position, Quaternion.identity);
                
                // Add to player's sticker bag if they have one
                AddStickerToBag(sticker);
            }
        }
        
        private void AddStickerToBag(GameObject sticker)
        {
            if (playerTransform == null) return;
            
            Inventory inventory = playerTransform.GetComponent<Inventory>();
            if (inventory != null)
            {
                foreach (Item item in inventory.Items)
                {
                    if (item is StickerBag stickerBag)
                    {
                        // Add sticker to bag's prefab list
                        // Modify StickerBag to accept new stickers
                        Debug.Log("Found hidden sticker!");
                        break;
                    }
                }
            }
        }

        private void ClearDistantClues()
        {
            for (int i = revealedClues.Count - 1; i >= 0; i--)
            {
                if (revealedClues[i] == null)
                {
                    revealedClues.RemoveAt(i);
                    continue;
                }
                
                ClueRevealer clue = revealedClues[i].GetComponent<ClueRevealer>();
                if (clue == null || !clue.revealOnlyWithSunglasses)
                {
                    float distance = Vector2.Distance(playerTransform.position, revealedClues[i].transform.position);
                    if (distance > revealRadius)
                    {
                        clue.HideClue();
                        revealedClues.RemoveAt(i);
                    }
                }
            }
        }
        
        private void UnequipFlashlight()
        {
            isEquipped = false;
            playerTransform = null;
            
            if (revealCoroutine != null)
            {
                MonoBehaviour playerMono = playerTransform?.GetComponent<MonoBehaviour>();
                if (playerMono != null)
                {
                    playerMono.StopCoroutine(revealCoroutine);
                }
                revealCoroutine = null;
            }
            
            Debug.Log("Flashlight unequipped");
        }
        
        public override void OnRemoveFromInventory(Inventory inventory)
        {
            if (isEquipped)
            {
                UnequipFlashlight();
            }
        }
    }
}