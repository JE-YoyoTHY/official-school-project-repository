using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86;

public class PlayerControlScript : MonoBehaviour
{
	//variables
	#region variable
	//reference
	private Rigidbody2D rb;

	//physics
	private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed
	[SerializeField] private float myNormalGravityScale;
	[SerializeField] private float myNormalGravityMaxSpeed;
	[SerializeField] private float myFrictionAcceleration;
	[SerializeField] private float myAdjustFriction; // when player has acceleration and speed greater than max, apply this force
	private bool isFrictionActive;
	private Coroutine myFrictionLessCoroutine;

	[Header("move")]
	[SerializeField] private float moveMaxSpeed;
	[SerializeField] private float moveAcceleration; // speed add per second
	private bool isMoving;
	private sbyte moveKeyValue;
	[SerializeField] private float moveTurnSpeedScale;


	#endregion



	// Start is called before the first frame update
	void Start()
    {
        //init
		rb = GetComponent<Rigidbody2D>();
		myGravityScale = myNormalGravityScale;
		myGravityMaxSpeed = myNormalGravityMaxSpeed;
		isFrictionActive = true;

    }

    // Update is called once per frame
    void Update()
    {
		myGravityMain();
		moveMain();

		myFrictionMain();
    }

	#region physic function


	private void myAcceleration(Vector2 localAcceleration, Vector2 localMaxVelocity)
	{
		// x
		if(localAcceleration.x > 0)
		{
			if(rb.velocity.x < localMaxVelocity.x)
			{	
				rb.AddForce(new Vector2(Mathf.Min(localAcceleration.x * Time.deltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if(rb.velocity.x > localMaxVelocity.x)
			{
				rb.AddForce(Mathf.Max(myAdjustFriction * Time.deltaTime * -1, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

			}
		}
		else if(localAcceleration.x < 0)
		{
			if (rb.velocity.x > localMaxVelocity.x)
			{
				rb.AddForce(new Vector2(Mathf.Max(localAcceleration.x * Time.deltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.x < localMaxVelocity.x)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.deltaTime * -1, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

			}
		}

		// y
		if (localAcceleration.y > 0)
		{
			if (rb.velocity.y < localMaxVelocity.y)
			{
				rb.AddForce(new Vector2(0, Mathf.Min(localAcceleration.y * Time.deltaTime, localMaxVelocity.y - rb.velocity.y)) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.y > localMaxVelocity.y)
			{
				rb.AddForce(Mathf.Max(myAdjustFriction * Time.deltaTime * -1, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}
		else if (localAcceleration.y < 0)
		{
			if (rb.velocity.y > localMaxVelocity.y)
			{
				rb.AddForce(new Vector2(0, Mathf.Max(localAcceleration.y * Time.deltaTime, localMaxVelocity.y - rb.velocity.y)) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.y < localMaxVelocity.y)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.deltaTime * -1, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}
	}

	private void myImpulseAcceleration(Vector2 localImpulseAcceleration)
	{
		rb.AddForce(localImpulseAcceleration * rb.mass, ForceMode2D.Impulse);
	}

	private void mySetVx(float localVx)
	{
		rb.velocity = new Vector2(localVx, rb.velocity.y);
	}
	private void mySetVy(float localVy)
	{
		rb.velocity = new Vector2(rb.velocity.x, localVy);
	}

	private void mySetGravity(float localGravityScale, float localMaxGravity)
	{
		myGravityScale = localGravityScale;
		myGravityMaxSpeed = localMaxGravity;
	}

	#endregion


	#region move

	private void moveMain()
	{
		if(moveKeyValue != 0)
		{
			if (canMove())
			{
				isMoving = true;
				
				if(moveKeyValue * rb.velocity.x >= 0) // same direction
				{
					myAcceleration(new Vector2(moveAcceleration * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
				}
				else if(moveKeyValue * rb.velocity.x < 0)
				{
					myAcceleration(new Vector2(moveAcceleration * moveKeyValue * moveTurnSpeedScale, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
				}
			}
		}

		if(moveKeyValue == 0)
		{
			isMoving = false;
		}
	}

	private bool canMove()
	{
		if (true) return true;
		else return false;
	}

	public void moveInput(InputAction.CallbackContext ctx)
	{
		moveKeyValue = (sbyte)ctx.ReadValue<float>();
	}

	#endregion

	#region natural force includes gravity and friction

	//friction
	private void myFrictionMain() // horizontal
	{
		if(!isMoving && isFrictionActive)
		{
			if(rb.velocity.x < 0)
			{
				myAcceleration(new Vector2(myFrictionAcceleration * 1, 0), Vector2.zero);
			}

			if (rb.velocity.x > 0)
			{
				myAcceleration(new Vector2(myFrictionAcceleration * -1, 0), Vector2.zero);
			}
		}
	}

	IEnumerator myFrictionLess(float t) // t for time
	{
		while(t > 0)
		{
			t -= Time.deltaTime;
			yield return null;
		}
		isFrictionActive = true;
	}


	//gravity
	private void myGravityMain()
	{
		myAcceleration(Vector2.down * myGravityScale, Vector2.down * myGravityMaxSpeed);
	}

	#endregion
}
