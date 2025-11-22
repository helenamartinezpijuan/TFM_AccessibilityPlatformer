using UnityEngine;

namespace PlatformerGame
{
    public class CameraFollowPixelPerfect : MonoBehaviour
    {
        public Transform target;
        public Vector2 offset = Vector2.zero;
        public float smoothTime = 0.3f;
        
        private Vector3 velocity = Vector3.zero;

        void LateUpdate()
        {
            if (target == null) return;
            
            Vector3 targetPosition = target.position + (Vector3)offset;
            targetPosition.z = transform.position.z;
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}

