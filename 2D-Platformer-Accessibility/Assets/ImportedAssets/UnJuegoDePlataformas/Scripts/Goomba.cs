using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
	
	public MarioController Mario;
	
	public SpriteRenderer spriteRenderer;
	public Sprite HitSprite;
	
	public float jumpForce = 2f;


	private void Awake()
	{	
		// Default goomba sprite
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		//////////////////////////
		// You killed a goomba! //
		//////////////////////////
		if (other.gameObject.CompareTag("Mario"))
			KillGoomba();
	}
		
	private void KillGoomba()
	{
		spriteRenderer.sprite = HitSprite;
		Destroy(transform.parent.gameObject);
	}

}