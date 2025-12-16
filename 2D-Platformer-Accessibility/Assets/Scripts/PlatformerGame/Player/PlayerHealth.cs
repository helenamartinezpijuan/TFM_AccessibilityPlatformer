using UnityEngine;
using System.Collections;

namespace PlatformerGame.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Parameters")]
        [SerializeField] private int maxHealth = 4;
        private int currentHealth;

        [Header("Respawn Parameters")]
        [SerializeField] private CheckpointSystem checkpointSystem;

        [Header("UI Elements")]
        [SerializeField] private HealthUI healthUI;

        private void Awake()
        {
            currentHealth = maxHealth;
            healthUI.Initialize(maxHealth);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log("Player Health: " + currentHealth);

            // Update UI
            healthUI.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            this.transform.position = checkpointSystem.GetRespawnPosition();
            // Trigger animation or effects when respawning
            // Add sound effect here
        }

        private void Die()
        {
            Debug.Log("Player has died.");
            
            // Restart game and show game over screen

        }

        public void Heal(int amount)
        {
            int previousHealth = currentHealth;
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthUI.SetHealth(previousHealth);
        }

        // Reset health (for new level/checkpoint)
        public void ResetHealth()
        {
            int previousHealth = currentHealth;
            currentHealth = maxHealth;
            healthUI.SetHealth(previousHealth);
        }
    }
}