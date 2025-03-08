using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundTriggerScript : MonoBehaviour
{

	public bool isGrounded { get; private set; } = false;
	private const int groundLayer = 6;

	private BreakablePlatformScript currentBreakablePlatform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.layer == groundLayer)
		{
			isGrounded = true;

			if(collision.gameObject.tag == "BreakablePlatform")
			{
				currentBreakablePlatform = collision.gameObject.GetComponent<BreakablePlatformScript>();
				currentBreakablePlatform.traversalThroughAdjacentTiles(null);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == groundLayer)
		{
			if (collision.CompareTag("BreakablePlatform")) currentBreakablePlatform = null;

			isGrounded = false;
		}
	}

	public void leaveGround(bool byJump)
	{
		if(isGrounded && byJump && currentBreakablePlatform != null)
		{
			currentBreakablePlatform.playerJumpOnThisTraversal(null);
		}


		isGrounded = false;
	}

	public bool onGround()
	{
		return isGrounded;
	}
}
