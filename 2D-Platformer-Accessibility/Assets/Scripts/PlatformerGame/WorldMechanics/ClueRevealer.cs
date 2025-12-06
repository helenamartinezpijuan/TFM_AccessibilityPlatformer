using UnityEngine;
using PlatformerGame.Inventory.Items;

namespace PlatformerGame.WorldMechanics
{
    public class ClueRevealer : MonoBehaviour
    {
        [Header("Clue Settings")]
        [SerializeField] private GameObject cluePrefab;
        [SerializeField] public bool revealOnlyWithSunglasses = false;
        
        private GameObject currentClue;
        private bool isRevealed = false;
        
        public bool IsRevealed => isRevealed;
        
        public void RevealClue()
        {
            if (isRevealed || cluePrefab == null) return;
            
            currentClue = Instantiate(cluePrefab, transform.position, Quaternion.identity);
            currentClue.transform.SetParent(transform);
            isRevealed = true;
        }
        
        public void HideClue()
        {
            if (!isRevealed) return;
            
            if (currentClue != null)
            {
                Destroy(currentClue);
                currentClue = null;
            }
            isRevealed = false;
        }
        
        private void OnDestroy()
        {
            HideClue();
        }
        
        // Optional: Draw gizmo to show clue radius
        private void OnDrawGizmosSelected()
        {
            if (cluePrefab != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}