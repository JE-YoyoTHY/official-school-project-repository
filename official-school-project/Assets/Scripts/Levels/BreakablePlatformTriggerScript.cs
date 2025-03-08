using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformTriggerScript : MonoBehaviour
{

	private void Start()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.right;
	}

	private void Update()
	{
		if(GetComponent<Rigidbody2D>().velocity.x > 0) GetComponent<Rigidbody2D>().velocity -= Vector2.right * Time.deltaTime;
		if (GetComponent<Rigidbody2D>().velocity.x < 0) GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("BreakablePlatform") && collision.transform != transform.parent) 
		{
			transform.parent.GetComponent<BreakablePlatformScript>().tileOnRight = collision.GetComponent<BreakablePlatformScript>();
			transform.parent.GetComponent<BreakablePlatformScript>().tileOnRight.tileOnLeft = transform.parent.GetComponent<BreakablePlatformScript>();
		}
	}



}
