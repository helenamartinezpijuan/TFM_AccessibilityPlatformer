using UnityEngine;
using UnityEngine.SceneManagement;
using PlatformerGame.Managers;

namespace PlatformerGame.Player
{
    public class PreviousLevel : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Going to previous Scene");
                SceneTransitionManager.Instance?.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                
            }
        }
    }
}