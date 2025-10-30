using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mushroom : MonoBehaviour
{	
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Mario"))
		{
			Destroy(gameObject);
		}
	}

}