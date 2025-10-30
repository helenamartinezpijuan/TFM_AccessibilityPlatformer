using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitCoinBlock : MonoBehaviour
{
	
	public Animator idleBlockAnimator;
	
	public int hits = 1;

    public void Start()
    {
        // Get the Animator
        idleBlockAnimator = gameObject.GetComponent<Animator>();
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// check if Mario hits the block
		if (collision.gameObject.CompareTag("Mario"))
			Hit();
	}
	
	public void Hit()
	{
		hits--;
		
		// Jump!
		idleBlockAnimator.SetBool("Hit", true);
		
		Invoke("CheckHits", 0.01f);
	}
	
	public void CheckCoinBlockHits()
	{	
		if(hits <= 0)
			// Coin!
			idleBlockAnimator.SetBool("Coin", true);

			Invoke("DestroyBlock", 0.2f);
	}
	
	public void DestroyCoinBlock()
	{
		idleBlockAnimator.SetBool("Checked", true);
	}
}