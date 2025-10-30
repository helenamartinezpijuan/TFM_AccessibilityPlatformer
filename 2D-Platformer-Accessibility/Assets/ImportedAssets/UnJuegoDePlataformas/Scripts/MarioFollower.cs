using UnityEngine;

public class MarioFollower : MonoBehaviour
{

	public GameObject FollowObject;
	public Vector2 FollowOffset;
	public float speed = 3;
	
	private Vector2 threshold;
	
	private Rigidbody2D mario;
	

    private void Start()
    {
        //offset = Mario.transform.position - transform.position;
		threshold = calculateThreshold();
		mario = FollowObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //transform.position = Mario.position - offset;
		Vector2 follow = FollowObject.transform.position;
		float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x);
		float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y);
		
		Vector3 newPosition = transform.position;
		if(Mathf.Abs(xDifference) >= threshold.x)
		{
			newPosition.x = follow.x;
		}
		
		if(Mathf.Abs(yDifference) >= threshold.y)
		{
			newPosition.y = follow.y;
		}
		
		float moveSpeed = mario.linearVelocity.magnitude > speed ? mario.linearVelocity.magnitude : speed;
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
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Vector2 border = calculateThreshold();
		Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
	}
		
}
