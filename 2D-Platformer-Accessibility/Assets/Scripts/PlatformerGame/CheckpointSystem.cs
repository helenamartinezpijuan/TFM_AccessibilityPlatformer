using UnityEngine;
using System.Collections;

namespace PlatformerGame.Player
{
    public class CheckpointSystem : MonoBehaviour
    {
        private Transform currentCheckpoint;

        void Start()
        {
            currentCheckpoint = this.transform;
        }

        public Vector3 GetRespawnPosition()
        {
            return currentCheckpoint.position;
        }
    }
}