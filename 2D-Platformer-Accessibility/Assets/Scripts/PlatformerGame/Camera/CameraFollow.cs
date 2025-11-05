using UnityEngine;

namespace PlatformerGame
{
    public class CameraFollow : MonoBehaviour
    {
        public GameObject FollowObject;
        public Vector2 FollowOffset;
        public float speed = 3;

        private Vector2 threshold;

        private Rigidbody2D player;


        private void Start()
        {
            //offset = player.transform.position - transform.position;
            threshold = calculateThreshold();
            player = FollowObject.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 follow = FollowObject.transform.position;
            float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);
            float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);

            Vector3 newPosition = transform.position;
            if (Mathf.Abs(xDifference) >= threshold.x)
            {
                newPosition.x = follow.x;
            }

            if (Mathf.Abs(yDifference) >= threshold.y)
            {
                newPosition.y = follow.y;
            }

            float moveSpeed = player.linearVelocity.magnitude > speed ? player.linearVelocity.magnitude : speed;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
        }

        private Vector3 calculateThreshold()
        {
            Rect aspect = Camera.main.pixelRect;
            Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
            t.x -= FollowOffset.x;
            t.y -= FollowOffset.y;
            return t;
        }
    }
	
}