using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformerGame.GameManager
{
    public class LevelManager : MonoBehaviour
    {
        void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    }
}