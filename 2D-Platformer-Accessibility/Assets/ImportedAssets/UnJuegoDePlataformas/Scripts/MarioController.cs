using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarioController : MonoBehaviour
{
	
	//////////////////////
	// Let's get moving //
	//////////////////////
    public float Speed = 1;
	public float ButtonTime = 0.5f;
	public float JumpHeight = 5;
	public float CancelRate = 100;
	
	///////////////////////
	// Let's get jumping //
	///////////////////////
	private float jumpTime;
	private bool jumping;
	private bool jumpCancelled;
	
	////////////////////////////
	// Keep yourself grounded //
	////////////////////////////
	public LayerMask groundLayer;
	private float distanceToGround;
	
	////////////////////////
	// Show me your moves //
	////////////////////////
	public SpriteRenderer CurrentSprite;
	
	public Sprite SmallBoy;
	public Sprite BigBoy;
	public Sprite FlowerBoy;
	
	public Sprite[] WalkRight_small;
	public Sprite[] WalkRight_big;
	public Sprite[] WalkRight_flower;
	public Sprite[] WalkLeft_small;
	public Sprite[] WalkLeft_big;
	public Sprite[] WalkLeft_flower;
	
	public Sprite Jump_small;
	public Sprite Jump_big;
	public Sprite Jump_flower;

	
	private int i = 0;
	
	///////////////////////
	// Bricks and blocks //
	///////////////////////
	public HitBrick HitBrick;
	public HitBlock HitBlock;
	
	/////////////
	// Goomba! //
	/////////////
	public Goomba Goomba;
	
	///////////////
	// Mushroom! //
	///////////////
	public Mushroom Mushroom;
	
	/////////////
	// Flower! //
	/////////////
	public Flower Flower;
	public int powerUp = 0;

	////////////////
	// Win or die //
	////////////////
	public Sprite DeadBoy;
    public Action OnKilled;
    public Action OnReachedEndOfLevel;

	///////////////////////
	// It's a'me, Mario! //
	///////////////////////
    private Rigidbody2D mario;


/////////////////////
/////// AWAKE ///////
/////////////////////

    private void Awake()
    {
		// Mario
        mario = GetComponent<Rigidbody2D>();
		// Default sprite
		CurrentSprite = gameObject.GetComponent<SpriteRenderer>();
		
		CurrentSprite.sprite = SmallBoy;
    }

////////////////////////
/////// MOVEMENT ///////
////////////////////////

    public void Update()
    {
    
		////////////////
		// Move right //
		////////////////
		if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
			ChangeWalkRightSprite();
        }

		///////////////
		// Move left //
		///////////////
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
			ChangeWalkLeftSprite();
        }

		///////////
		// Jump! //
		///////////
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {	
			Jump();
		}
		
    }
	
	private void FixedUpdate()
    {
        if(jumpCancelled && jumping && mario.linearVelocity.y > 0)
        {
            mario.AddForce(Vector2.down * CancelRate);
        }
    }

	// Right!
    private void MoveRight()
    {
		transform.Translate(Vector2.right * Speed * Time.deltaTime);
	}
	// Left!
    private void MoveLeft()
    {
		transform.Translate(Vector2.left * Speed * Time.deltaTime);
	}
	// Jump!
	private void Jump()
	{
		if (IsGrounded() == true)
		{
			float jumpForce = Mathf.Sqrt(JumpHeight * -2 * (Physics2D.gravity.y * mario.gravityScale));
			mario.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			
			jumping = true;
			jumpCancelled = false;
			jumpTime = 0;
			
			if(jumping)
			{
				jumpTime += Time.deltaTime;
				
				if(PowerUp() == 0)
				{
					CurrentSprite.sprite = Jump_small;
				}
				if(PowerUp() == 1)
				{
					CurrentSprite.sprite = Jump_big;
				}
				if(PowerUp() == 2)
				{
					CurrentSprite.sprite = Jump_flower;
				}

				if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
				{
				
					jumpCancelled = true;
				}
				if (jumpTime > ButtonTime)
				{
					jumping = false;
				}
			}
		}
	}

//////////////////////////
/////// ANIMATION ///////
/////////////////////////

	private void ChangeWalkRightSprite()
	{
		if(PowerUp() == 0)
		{
			CurrentSprite.sprite = WalkRight_small[i];
			if(i<2)
				i++;
			else
				i--;
		}
		
		if(PowerUp() == 1)
		{
			CurrentSprite.sprite = WalkRight_big[i];
			if(i<2)
				i++;
			else
				i--;
		}
		
		if(PowerUp() == 2)
		{
			CurrentSprite.sprite = WalkRight_flower[i];
			if(i<2)
				i++;
			else
				i--;
		}
	}
	
	private void ChangeWalkLeftSprite()
	{
		if(PowerUp() == 0)
		{
			CurrentSprite.sprite = WalkLeft_small[i];
			if(i<2)
				i++;
			else
				i--;
		}
		
		if(PowerUp() == 1)
		{
			CurrentSprite.sprite = WalkLeft_big[i];
			if(i<2)
				i++;
			else
				i--;
		}
		
		if(PowerUp() == 2)
		{
			CurrentSprite.sprite = WalkLeft_flower[i];
			if(i<2)
				i++;
			else
				i--;
		}
	}


///////////////////////
/////// PHYSICS ///////
///////////////////////

	private bool IsGrounded()
	{
		distanceToGround = 2.0f;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
		
		if(hit.collider != null)
		{
			return true;
		}
		return false;
	}

////////////////////////
/////// POWERUPS ///////
////////////////////////
	
	public int PowerUp()
	{
		return powerUp;
	}

///////////////////////
/////// ENEMIES ///////
///////////////////////
	
	public void Death()
	{
		if (PowerUp() == 0)
		{
			CurrentSprite.sprite = DeadBoy;
			OnKilled?.Invoke();
		}
		
		if (PowerUp() == 1)
		{
			CurrentSprite.sprite = SmallBoy;
			powerUp = 0;
		}
		
		if (PowerUp() == 2)
		{
			CurrentSprite.sprite = BigBoy;
			powerUp = 1;
		}
	}
		

		
//////////////////////////
/////// ON TRIGGER ///////
//////////////////////////

    private void OnTriggerEnter2D(Collider2D other)
    {
		///////////////////////////
		// You reached the flag! //
		///////////////////////////
        if (other.gameObject.CompareTag("EndOfLevel"))
            OnReachedEndOfLevel?.Invoke();
		
		///////////////////////////
		// You found a mushroom! //
		///////////////////////////
		if(other.gameObject.CompareTag("Mushroom"))
		{
			powerUp = 1;
		}
		
		/////////////////////////
		// You found a flower! //
		/////////////////////////
		if (other.gameObject.CompareTag("Flower"))
		{
			powerUp = 2;
		}
					
    }
	
}
