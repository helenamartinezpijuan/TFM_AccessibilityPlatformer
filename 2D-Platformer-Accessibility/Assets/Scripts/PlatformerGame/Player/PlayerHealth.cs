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
        [SerializeField] private float respawnDelay = 1f;

        [Header("UI Elements")]
        [SerializeField] private HealthUI healthUI;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip damageSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip respawnSound;
        [SerializeField] private AudioClip healSound;
        [Range(0f, 1f)] [SerializeField] private float soundVolume = 0.7f;
        private AudioSource audioSource;

        [Header("Animation References")]
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private ParticleSystem damageParticles;
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private ParticleSystem respawnParticles;

        // Animation parameter names (use the same as in your Animator controller)
        private const string DAMAGE_TRIGGER = "TakeDamage";
        private const string DEATH_TRIGGER = "Die";
        private const string RESPAWN_TRIGGER = "Respawn";

        // Public property for other scripts to check health
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsFullHealth => currentHealth == maxHealth;
        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maxHealth;
            
            // Initialize audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0.7f; // 2D with some spatial awareness
            }

            // Initialize UI
            if (healthUI != null)
            {
                healthUI.Initialize(maxHealth);
            }

            // Animator
            if (playerAnimator == null)
            {
                playerAnimator = GetComponent<Animator>();
            }
        }

        public void TakeDamage(int damage)
        {
            // Check if player is already dead
            if (currentHealth <= 0) return;

            // Apply damage
            int previousHealth = currentHealth;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            Debug.Log($"Player Health: {currentHealth} (Took {damage} damage)");

            // Update UI
            if (healthUI != null)
            {
                healthUI.SetHealth(currentHealth);
            }

            // Play damage sound
            if (damageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(damageSound, soundVolume);
            }

            // Trigger damage animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(DAMAGE_TRIGGER);
            }

            // Play damage particles
            if (damageParticles != null)
            {
                damageParticles.Play();
            }

            // Check for death
            if (currentHealth <= 0)
            {
                StartCoroutine(DieSequence());
            }
            else
            {
                StartCoroutine(RespawnAfterDelay(0.2f));
            }
        }

        private IEnumerator DieSequence()
        {
            // Play death sound
            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound, soundVolume);
            }

            // Trigger death animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(DEATH_TRIGGER);
            }

            // Play death particles
            if (deathParticles != null)
            {
                deathParticles.Play();
            }

            Debug.Log("Player has died.");

            // Disable player controls temporarily
            DisablePlayerControls();

            // Wait for death animation
            yield return new WaitForSeconds(respawnDelay);

            // Respawn at checkpoint
            StartCoroutine(RespawnSequence());
        }

        private IEnumerator RespawnSequence()
        {
            // Play respawn sound
            if (respawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(respawnSound, soundVolume);
            }

            // Play respawn particles at checkpoint
            if (respawnParticles != null)
            {
                respawnParticles.transform.position = checkpointSystem.GetRespawnPosition();
                respawnParticles.Play();
            }

            // Move player to checkpoint
            transform.position = checkpointSystem.GetRespawnPosition();

            // Trigger respawn animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(RESPAWN_TRIGGER);
            }

            // Reset health for respawn
            ResetHealth();

            // Re-enable player controls
            EnablePlayerControls();

            yield return null;
        }

        private IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // Optional: Add knockback or visual feedback here
            transform.position = checkpointSystem.GetRespawnPosition();
            
            // Play respawn effect for non-lethal damage too
            if (respawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(respawnSound, soundVolume * 0.5f);
            }
        }

        public void Heal(int amount)
        {
            int previousHealth = currentHealth;
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Update UI
            if (healthUI != null)
            {
                healthUI.SetHealth(currentHealth);
            }

            // Play heal sound
            if (healSound != null && audioSource != null && amount > 0)
            {
                audioSource.PlayOneShot(healSound, soundVolume);
            }

            // Optional: Play heal particles
            Debug.Log($"Player healed: {previousHealth} -> {currentHealth}");
        }

        public void ResetHealth()
        {
            int previousHealth = currentHealth;
            currentHealth = maxHealth;
            
            if (healthUI != null)
            {
                healthUI.SetHealth(currentHealth);
            }
            
            Debug.Log($"Health reset to max: {currentHealth}");
        }

        private void DisablePlayerControls()
        {
            // Disable movement
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Disable collider temporarily
            /*var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }*/
        }

        private void EnablePlayerControls()
        {
            // Re-enable movement and other player scripts
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

            // Re-enable collider
            /*var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }*/
        }
    }
}