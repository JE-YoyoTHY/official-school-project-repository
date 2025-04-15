using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformTriggerScript : MonoBehaviour
{
	private bool merged = false;


	private void Start()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.right;
		StartCoroutine(merge());
	}

	private void Update()
	{
		if(GetComponent<Rigidbody2D>().velocity.x > 0) GetComponent<Rigidbody2D>().velocity -= Vector2.right * Time.deltaTime;
		if (GetComponent<Rigidbody2D>().velocity.x < 0) GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("BreakablePlatform") && collision.transform != transform.parent && !merged) 
		{
			transform.parent.GetComponent<BreakablePlatformScript>().tileOnRight = collision.GetComponent<BreakablePlatformScript>();
			transform.parent.GetComponent<BreakablePlatformScript>().tileOnRight.tileOnLeft = transform.parent.GetComponent<BreakablePlatformScript>();
		}
	}

	private IEnumerator merge()
	{
		yield return new WaitForSeconds(0.5f);
		merged = true;
	}

}
