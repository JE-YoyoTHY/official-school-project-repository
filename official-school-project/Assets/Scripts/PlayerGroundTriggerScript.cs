using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundTriggerScript : MonoBehaviour
{

	private bool isGrounded;
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
		if(collision.gameObject.layer == 6)
		{
			isGrounded = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 6)
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
