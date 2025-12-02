using UnityEngine;
using System.Collections;

namespace PlatformerGame.Player
{
    public class CheckpointSystem : MonoBehaviour
    {
        public bool isFirstCheckpoint;
        private Transform currentCheckpoint;

        void Start()
        {
            if (isFirstCheckpoint)
                currentCheckpoint = this.transform;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                UpdateCheckpoint(this.transform);
        }

        void UpdateCheckpoint(Transform checkpoint)
        {
            currentCheckpoint = checkpoint;
        }

        public Vector3 GetRespawnPosition()
        {
            return currentCheckpoint.position;
        }
    }
}