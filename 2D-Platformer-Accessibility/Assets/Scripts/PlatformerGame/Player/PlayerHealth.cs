using UnityEngine;
using System.Collections;

namespace PlatformerGame.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Parameters")]
        [SerializeField] private int maxHealth = 5;
        private int currentHealth;

        [Header("Respawn Parameters")]
        [SerializeField] private CheckpointSystem checkpointSystem;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log("Player Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }

            this.transform.position = checkpointSystem.GetRespawnPosition();
            // Trigger animation or effects when respawning
            // Add music or sound effect here
        }

        private void Die()
        {
            Debug.Log("Player has died.");
            
            // Restart game

        }
    }
}