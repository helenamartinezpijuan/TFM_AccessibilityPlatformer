using UnityEngine;
using System.Collections.Generic;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Inventory.Items
{
    [CreateAssetMenu(fileName = "Sunglasses", menuName = "PlatformerGame/Inventory/Items/Sunglasses")]
    public class Sunglasses : Item
    {
        private bool isEquipped = false;
        private List<GameObject> revealedClues = new List<GameObject>();
        
        public override void Use(PlayerInventory inventory)
        {
            if (!isEquipped)
            {
                EquipSunglasses();
            }
            else
            {
                UnequipSunglasses();
            }
        }
        
        private void EquipSunglasses()
        {
            isEquipped = true;
            RevealAllClues();
            Debug.Log("Sunglasses equipped - revealing all clues");
        }
        
        private void UnequipSunglasses()
        {
            isEquipped = false;
            HideAllClues();
            Debug.Log("Sunglasses unequipped");
        }
        
        private void RevealAllClues()
        {
            // Find all clue revealers in the scene
            ClueRevealer[] allClueRevealers = FindObjectsByType<ClueRevealer>(FindObjectsSortMode.None);
            
            foreach (ClueRevealer clueRevealer in allClueRevealers)
            {
                if (clueRevealer != null)
                {
                    clueRevealer.RevealClue();
                    revealedClues.Add(clueRevealer.gameObject);
                }
            }
        }
        
        private void HideAllClues()
        {
            foreach (GameObject clueObject in revealedClues)
            {
                if (clueObject != null)
                {
                    ClueRevealer clue = clueObject.GetComponent<ClueRevealer>();
                    if (clue != null && clue.revealOnlyWithSunglasses)
                    {
                        clue.HideClue();
                    }
                }
            }
            revealedClues.Clear();
        }
        
        public override void OnRemoveFromInventory(PlayerInventory inventory)
        {
            if (isEquipped)
            {
                UnequipSunglasses();
            }
        }
        
        public bool IsEquipped => isEquipped;
    }
}