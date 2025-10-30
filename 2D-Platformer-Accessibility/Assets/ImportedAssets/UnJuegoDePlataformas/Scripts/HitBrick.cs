using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBrick : MonoBehaviour
{
	
	public GameObject BreakBits;
	public GameObject BrickCollider;
	
	public MarioController Mario;
	
	public Animator brickAnimator;

	public SpriteRenderer SpriteRenderer;
	
	// In case we want to activate a multiple coin brick/block... which I didn't have time for -_-
	public int hits = 1;

	public void Awake()
	{
		//Get brick Animator
		brickAnimator = gameObject.GetComponent<Animator>();
	}
	
	public void OnCollisionEnter2D(Collision2D collision)
	{
		//////////////////////
		// You hit a brick! //
		//////////////////////
		if (collision.gameObject.CompareTag("Mario"))
		{
			if(Mario.PowerUp() == 1 || Mario.PowerUp() == 2)
				Break();
			else
				// You are too small to break the brick
				Hit();
		}
	}
	
	public void Hit()
	{
		brickAnimator.SetTrigger("Hit");
	}
	
	public void Break()
	{
		BrickCollider.SetActive(false);
			
		BreakBits.SetActive(true);
		SpriteRenderer.enabled = false;
			
		Invoke("DestroyBrick", 0.5f);
	}
		
	public void DestroyBrick()
	{
		Destroy(gameObject);
	}
}