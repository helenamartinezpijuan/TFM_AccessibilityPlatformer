using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformerGame.Player
{
    public class EndLevel : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Level Completed!");
                // Level completion logic here (load next level, show UI, etc.)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                
            }
        }
    }
}