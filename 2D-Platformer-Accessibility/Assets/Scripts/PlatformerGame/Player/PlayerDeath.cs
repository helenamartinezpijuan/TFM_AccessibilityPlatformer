using UnityEngine;
using PlatformerGame.WorldMechanics;

namespace PlatformerGame.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        [Header("Death Settings")]
        [SerializeField] private float respawnDelay = 1f;
        
        private CheckpointSystem checkpointSystem;
        private bool isDead = false;
        
        private void Start()
        {
            checkpointSystem = GetComponent<CheckpointSystem>();
        }
        
        public void Die()
        {
            if (isDead) return;
            
            isDead = true;
            Debug.Log("Player died!");
            
            // Reset all activated levers
            ResetAllLevers();
            
            // Respawn after delay
            Invoke(nameof(Respawn), respawnDelay);
        }
        
        private void ResetAllLevers()
        {
            CombinationLever[] allLevers = FindObjectsByType<CombinationLever>(FindObjectsSortMode.None);
            foreach (CombinationLever lever in allLevers)
            {
                lever.CheckPlayerDeath(gameObject);
            }
        }
        
        private void Respawn()
        {
            if (checkpointSystem != null)
            {
                transform.position = checkpointSystem.GetRespawnPosition();
            }
            
            isDead = false;
            // Reset player health/state here
        }
    }
}