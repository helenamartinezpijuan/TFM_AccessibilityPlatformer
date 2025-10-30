using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
	public MarioController MarioController;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Mario"))
			MarioController.Death();
	}
	
}