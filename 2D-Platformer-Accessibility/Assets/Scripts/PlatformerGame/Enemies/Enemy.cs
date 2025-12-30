using UnityEngine;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.Enemies
{
public class Enemy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;

    public void TakeDamage()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();

        // Enemy destroyed
        spriteRenderer.enabled = false;
        collider.enabled = false;
    }

    private void AttackPlayer()
    {
        PlayerDeath playerDeath = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeath>();
        playerDeath.Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }
}
}