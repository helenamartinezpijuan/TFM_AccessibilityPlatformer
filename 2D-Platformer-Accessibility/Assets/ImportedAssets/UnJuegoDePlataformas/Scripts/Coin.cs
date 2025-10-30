using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
	
	public static int coins = 0;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Mario"))
		{
			coins++;			
			Destroy(gameObject);
		}
	}
	
	public string CoinsCollected()
	{
		return coins.ToString("000");
	}
	
}
