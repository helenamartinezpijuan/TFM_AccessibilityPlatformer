using UnityEngine;
using System.Collections;
using PlatformerGame.Player;
using PlatformerGame.Enemies;

namespace PlatformerGame.WorldMechanics
{
public class FallSpikes : MonoBehaviour
{
    [Header("Enemy Reference")]
    [SerializeField] private Enemy enemy;
    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer renderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemy != null)
            {
                enemy.TakeDamage();
                renderer.enabled = false;
                // Trigger animation of spikes falling
            }
        }
    }
}
}