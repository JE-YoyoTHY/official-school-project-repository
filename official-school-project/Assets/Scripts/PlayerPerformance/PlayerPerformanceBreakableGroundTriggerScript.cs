using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformanceBreakablePlatformTriggerScript : MonoBehaviour
{
	private bool merged = false;


	private void Start()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.right;
		StartCoroutine(merge());
	}

	private void Update()
	{
		if (GetComponent<Rigidbody2D>().velocity.x > 0) GetComponent<Rigidbody2D>().velocity -= Vector2.right * Time.deltaTime;
		if (GetComponent<Rigidbody2D>().velocity.x < 0) GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("PerformanceTile") && collision.transform != transform.parent && !merged)
		{
			transform.parent.GetComponent<PlayerPerformanceBreakableGround>().tileOnRight = collision.GetComponent<PlayerPerformanceBreakableGround>();
			transform.parent.GetComponent<PlayerPerformanceBreakableGround>().tileOnRight.tileOnLeft = transform.parent.GetComponent<PlayerPerformanceBreakableGround>();
		}
	}

	private IEnumerator merge()
	{
		yield return new WaitForSeconds(0.5f);
		merged = true;
	}
}
