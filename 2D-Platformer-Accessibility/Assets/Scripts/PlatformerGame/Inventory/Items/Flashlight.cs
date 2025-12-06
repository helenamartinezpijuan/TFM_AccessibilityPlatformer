using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Flashlight", menuName = "PlatformerGame/Inventory/Flashlight")]
    public class Flashlight : Item
    {
        [Header("Flashlight Settings")]
        public float revealRadius = 2f;
        public LayerMask clueLayers;
        
        private bool isEquipped = false;
        private Transform playerTransform;
        private List<GameObject> revealedClues = new List<GameObject>();

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
            Debug.Log("Flashlight equipped");
            
            // Start checking for clues
            if (Application.isPlaying)
            {
                // Use coroutine to check for clues periodically
                MonoBehaviour playerMono = player.GetComponent<MonoBehaviour>();
                if (playerMono != null)
                {
                    playerMono.StartCoroutine(CheckForCluesRoutine());
                }
            }
        }

        private System.Collections.IEnumerator CheckForCluesRoutine()
        {
            while (isEquipped)
            {
                CheckForNearbyClues();
                yield return new WaitForSeconds(0.1f); // Check 10 times per second
            }
        }

        private void CheckForNearbyClues()
        {
            if (playerTransform == null) return;
            
            // Clear old clues outside radius
            ClearDistantClues();
            
            // Find all clue revealers within radius
            ClueRevealer[] allClueRevealers = FindObjectsOfType<ClueRevealer>();
            
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
            
            // Hide all revealed clues
            foreach (GameObject clueObject in revealedClues)
            {
                if (clueObject != null)
                {
                    ClueRevealer clue = clueObject.GetComponent<ClueRevealer>();
                    if (clue != null && !clue.revealOnlyWithSunglasses)
                    {
                        clue.HideClue();
                    }
                }
            }
            
            revealedClues.Clear();
            Debug.Log("Flashlight unequipped");
        }

        public override bool CanUse(Inventory inventory)
        {
            return true;
        }

        public override void OnRemoveFromInventory(Inventory inventory)
        {
            if (isEquipped)
            {
                UnequipFlashlight();
            }
        }
        
        public bool IsEquipped => isEquipped;
    }
}