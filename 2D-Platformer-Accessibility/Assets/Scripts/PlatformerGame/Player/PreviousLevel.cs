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
                Debug.Log("Level Completed!");
                SceneTransitionManager.Instance?.LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex - 1);
                
            }
        }
    }
}