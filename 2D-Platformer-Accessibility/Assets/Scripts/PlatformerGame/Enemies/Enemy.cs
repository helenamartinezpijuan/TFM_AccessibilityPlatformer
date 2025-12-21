using UnityEngine;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.Enemies
{
public class Enemy : MonoBehaviour
{

    public void TakeDamage()
    {
        // Enemy destroyed
        Destroy(this);
    }
}
}