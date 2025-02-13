using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundTriggerScript : MonoBehaviour
{

	public bool isGrounded { get; private set; } = false;
	private const int groundLayer = 6;
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
				collision.gameObject.GetComponent<BreakablePlatformScript>().touchPlayer();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == groundLayer)
		{
			isGrounded = false;
		}
	}

	public void leaveGround()
	{
		isGrounded = false;
	}

	public bool onGround()
	{
		return isGrounded;
	}
}
