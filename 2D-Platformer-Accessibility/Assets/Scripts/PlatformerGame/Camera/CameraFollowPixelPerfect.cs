using UnityEngine;

namespace PlatformerGame
{/*
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
    }*/

public class CameraFollowPixelPerfect : MonoBehaviour
{
    [Header("Target")]
        public Transform target;
        public Vector2 offset = Vector2.zero;
        public float smoothTime = 0.3f;
        
        [Header("Pixel Perfect Settings")]
        public int pixelsPerUnit = 18;
        public int baseResolutionHeight = 135; // This controls how "zoomed in" the camera is
        
        private Vector3 velocity = Vector3.zero;
        private Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
            SetupPixelPerfectCamera();
            
            // Snap to target immediately
            if (target != null)
            {
                Vector3 targetPosition = target.position + (Vector3)offset;
                targetPosition.z = transform.position.z;
                transform.position = targetPosition;
            }
        }

        void LateUpdate()
        {
            if (target == null) return;
            
            Vector3 targetPosition = target.position + (Vector3)offset;
            targetPosition.z = transform.position.z;
            
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref velocity, 
                smoothTime
            );
            
            // Apply pixel snapping to avoid sub-pixel movement
            SnapToPixelGrid();
        }

        private void SetupPixelPerfectCamera()
        {
            if (cam == null) return;
            
            // Calculate orthographic size for pixel perfection
            // This ensures each pixel aligns perfectly with world units
            float unitsPerPixel = 1f / pixelsPerUnit;
            cam.orthographicSize = (baseResolutionHeight * 0.5f) / pixelsPerUnit;
            
            Debug.Log($"Camera setup: {baseResolutionHeight}px height = {cam.orthographicSize} ortho size");
            Debug.Log($"Each pixel = {unitsPerPixel} world units");
        }

        private void SnapToPixelGrid()
        {
            float unitsPerPixel = 1f / pixelsPerUnit;
            
            Vector3 snappedPosition = transform.position;
            snappedPosition.x = Mathf.Round(transform.position.x / unitsPerPixel) * unitsPerPixel;
            snappedPosition.y = Mathf.Round(transform.position.y / unitsPerPixel) * unitsPerPixel;
            
            transform.position = snappedPosition;
        }
    }
}