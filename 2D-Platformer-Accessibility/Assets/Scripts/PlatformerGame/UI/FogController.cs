using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace PlatformerGame.WorldMechanics
{
    public class FogController : MonoBehaviour
    {
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = false;
            }
        }
    }
}