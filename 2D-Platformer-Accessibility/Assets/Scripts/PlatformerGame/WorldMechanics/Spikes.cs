using UnityEngine;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.WorldMechanics
{
public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                Debug.Log("Spikes damaged player.");
            }
        }
    }
}
}
