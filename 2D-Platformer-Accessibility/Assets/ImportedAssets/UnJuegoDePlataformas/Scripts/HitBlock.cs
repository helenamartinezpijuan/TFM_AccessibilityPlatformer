using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitBlock : MonoBehaviour
{
	public enum Type
    {
        CoinBlock,
        MushroomBlock,
        FlowerBlock,
    }
	
	public Type type;
	
	public MarioController MarioController;
	
	public Animator idleBlockAnimator;
	public Mushroom Mushroom;
	public Flower Flower;
	
	public int powerUp;
	
	public int hits = 1;

    public void Awake()
    {
        // Get the block Animator
        idleBlockAnimator = gameObject.GetComponent<Animator>();
		
		powerUp = MarioController.PowerUp();
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//////////////////////
		// You hit a block! //
		//////////////////////
		if (collision.gameObject.CompareTag("Mario"))
			Hit();
	}
	
	public void Hit()
	{
		hits--;
		
		// Jump!
		idleBlockAnimator.SetBool("Hit", true);
		
		if(type == Type.MushroomBlock && (powerUp == 1 || powerUp == 2))
			type = Type.FlowerBlock;
		
		if(type == Type.CoinBlock)
			Invoke("CheckCoinBlock", 0.3f);
		
		if(type == Type.MushroomBlock)	
			Invoke("CheckMushroomBlock", 0.3f);
		
		if(type == Type.FlowerBlock)
			Invoke("CheckFlowerBlock", 0.3f);
	}
	
	public void CheckCoinBlock()
	{	
		if(hits == 0)
			// Coin!
			idleBlockAnimator.SetBool("Coin", true);

			Invoke("DestroyBlock", 0.2f);
	}
	
	public void CheckMushroomBlock()
	{

		var position = transform.position;
		position.y += 0.8f;

		if(hits == 0)
		{
			// Mushroom!
			Instantiate(Mushroom, position, Quaternion.identity);			
			Invoke("DestroyBlock", 0.2f);
		}
	}
	
	public void CheckFlowerBlock()
	{
		var position = transform.position;
		position.y += 0.8f;
		
		if(hits == 0)
		{
			// Flower!
			Instantiate(Flower, position, Quaternion.identity);	
			Invoke("DestroyBlock", 0.2f);
		}
	}
	
	public void DestroyBlock()
	{	
		idleBlockAnimator.SetBool("Checked", true);
	}
}