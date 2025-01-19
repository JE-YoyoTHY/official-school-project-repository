using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86;

public class PlayerControlScript : MonoBehaviour
{
	//variables
	#region variable
	//reference
	private Rigidbody2D rb;
	private PlayerGroundTriggerScript groundTrigger;
	private GameObject fireballMeter;

	//physics
	private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed
	[SerializeField] private float myNormalGravityScale;
	[SerializeField] private float myNormalGravityMaxSpeed;
	[SerializeField] private float myFrictionAcceleration;
	[SerializeField] private float myAdjustFriction; // when player has acceleration and speed greater than max, apply this force
	private bool isFrictionActive;
	private Coroutine myFrictionLessCoroutine;

	[Header("Move")]
	[SerializeField] private float moveMaxSpeed;
	[SerializeField] private float moveAcceleration; // speed add per second
	private bool isMoving;
	private sbyte moveKeyValue;
	[SerializeField] private float moveTurnSpeedScale;


	[Header("Jump")]
	[SerializeField] private float jumpStrength;
	[SerializeField] private float jumpGravity;
	[SerializeField] private float jumpMinSpeed;
	private bool isJumping;
	private sbyte jumpKeyValue;
	[SerializeField] private float jumpPreInputTime;
	private Coroutine jumpCoroutine;
	private Coroutine jumpExtraHangTimeCoroutine;
	[SerializeField] private float jumpExtraHangTimeGravityScale;

	//ground hit
	[Header("Ground Hit")]
	[SerializeField] private float coyoteTime;
	private Coroutine coyoteTimeCoroutine;
	private bool onGround;

	//fireball
	[Header("Fireball")]
	[SerializeField] private GameObject fireballPrefab;
	[SerializeField] private float fireballPreInputTime;
	private sbyte fireballKeyValue;
	[SerializeField] private int fireballMaxCharges;
	private int fireballCurrentCharges;
	private Vector2 fireballDir; // the true dir
	private Vector2 fireballDirValue; // store input value
	private sbyte fireballDirLastHorizontal;
	private Coroutine fireballInputCoroutine;
	[SerializeField] private float fireballPushForceAcceleration;
	[SerializeField] private float fireballPushForceMaxSpeed;
	[SerializeField] private float fireballPushForceDuration;
	[SerializeField] private float fireballPushUpForceScale; // make upward force less
	private bool isFireballPushForceAdding;
	private float fireballPushForceDurationCounter;
	[SerializeField] private float fireballPushForceHangTimeDuration;
	[SerializeField] private float fireballCastFreezeTime;
	private Coroutine fireballHangTimeCoroutine;


	#endregion



	// Start is called before the first frame update
	void Start()
    {
        //init
		rb = GetComponent<Rigidbody2D>();
		groundTrigger = transform.GetChild(0).GetComponent<PlayerGroundTriggerScript>();
		fireballMeter = transform.GetChild(1).gameObject;

		myGravityScale = myNormalGravityScale;
		myGravityMaxSpeed = myNormalGravityMaxSpeed;
		isFrictionActive = true;

		fireballDirLastHorizontal = 1;

    }

    // Update is called once per frame
    void Update()
    {
		groundHitCheckMain();
		myGravityMain();
		moveMain();
		jumpMain();
		fireballMain();

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
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.deltaTime, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

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
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.deltaTime, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
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
                else  // moveKeyValue * rb.velocity.x < 0, 不同方向
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
		if (!isFireballPushForceAdding) return true;
		else return false;
	}

	public void moveInput(InputAction.CallbackContext ctx)
	{
		moveKeyValue = (sbyte)ctx.ReadValue<float>();
	}

	#endregion

	#region natural force, includes gravity and friction

	//friction
	private void myFrictionMain() // horizontal
	{
		if(!isMoving && isFrictionActive && !isFireballPushForceAdding)
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

	#region jump

	private void jumpMain()
	{
		if(jumpKeyValue == 2)
		{
			jumpKeyValue = 1;
			if(jumpCoroutine != null)
			{
				StopCoroutine(jumpCoroutine);
			}
			jumpCoroutine = StartCoroutine(jumpPreInput(jumpPreInputTime));
		}

		if (isJumping)
		{
			if(rb.velocity.y < jumpMinSpeed)
			{
				jumpEnd();
			}
		}

		//release jump button
		if(jumpKeyValue == -1)
		{
			if (isJumping)
			{
				jumpEnd();
			}
		}
	}

	private void jumpStart()
	{
		isJumping = true;
		mySetVy(jumpStrength);
		mySetGravity(jumpGravity, myGravityMaxSpeed);
		leaveGround();
	}

	private void jumpEnd()
	{
		mySetGravity(jumpGravity * jumpExtraHangTimeGravityScale, myGravityMaxSpeed);
		mySetVy(jumpMinSpeed);

		if (isJumping)
		{
			isJumping = false;
		}

		if(jumpExtraHangTimeCoroutine != null)
		{
			StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		jumpExtraHangTimeCoroutine = StartCoroutine(jumpExtraHangTime());
	}

	private bool canJump()
	{
		if (!isJumping && onGround && !isFireballPushForceAdding) return true;
		else return false;
	}

	public void jumpInput(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			jumpKeyValue = 2;
		}

		if (ctx.canceled)
		{
			jumpKeyValue = -1;
		}
	}

	IEnumerator jumpPreInput(float t) //t for time
	{
		while(t > 0)
		{
			t -= Time.deltaTime;

			if(canJump())
			{
				jumpStart();
				yield break;
			}

			yield return null;
		}
	}

	IEnumerator jumpExtraHangTime()
	{
		while(rb.velocity.y > jumpMinSpeed * -1)
		{
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
	}

	#endregion

	#region ground hit check

	private void groundHitCheckMain()
	{
		if (groundTrigger.onGround())
		{
			onGround = true;
			if(coyoteTimeCoroutine != null)
			{
				StopCoroutine(coyoteTimeCoroutine);
				coyoteTimeCoroutine = null;
			}
		}
		else
		{
			if(onGround && coyoteTimeCoroutine == null)
			{
				coyoteTimeCoroutine = StartCoroutine(leaveGroundDelay(coyoteTime));
			}
		}
	}

	private void leaveGround()
	{
		groundTrigger.leaveGround();
		onGround = false;

		if (coyoteTimeCoroutine != null)
		{
			StopCoroutine(coyoteTimeCoroutine);
			coyoteTimeCoroutine = null;
		}
	}

	IEnumerator leaveGroundDelay(float t) // t for time, this function implement the coyote time
	{
		while(t > 0)
		{
			t -= Time.deltaTime;
			yield return null;
		}
		leaveGround();
	}

	#endregion

	#region fireball

	private void fireballMain()
	{
		if(fireballKeyValue == 2)
		{
			fireballKeyValue = 1;

			if(fireballInputCoroutine != null)
			{
				StopCoroutine(fireballInputCoroutine);
			}
			fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime));	
		}

		fireballPushForceMain();
		fireballChargeMain();
	}

	private void fireballStart()
	{
		GameObject summonedFireball = null;
		summonedFireball = Instantiate(fireballPrefab, transform.position, transform.rotation);
		
		if(fireballDir.magnitude > 0) summonedFireball.GetComponent<FireballScript>().summon(fireballDir);
		else summonedFireball.GetComponent<FireballScript>().summon(new Vector2(fireballDirLastHorizontal, 0));
		
		fireballCurrentCharges--;
		fireballPushForceStart();
	}

	private bool canCastFireball()
	{
		if (fireballCurrentCharges > 0 && !isFireballPushForceAdding) return true;
		else return false;
	}

	private void fireballPushForceMain()
	{
		if (isFireballPushForceAdding)
		{
			if(fireballDir.magnitude == 0)
			{
				myAcceleration(new Vector2(fireballPushForceAcceleration * fireballDirLastHorizontal * -1,0), new Vector2(fireballPushForceMaxSpeed * fireballDirLastHorizontal * -1, 0));
			}
            else
            {
				Vector2 pushDir = fireballDir.normalized;
				if (pushDir.y < 0) pushDir = new Vector2(fireballDir.x, fireballDir.y * fireballPushUpForceScale);
				myAcceleration(pushDir * fireballPushForceAcceleration * -1, pushDir * fireballPushForceMaxSpeed * -1);
            }
			fireballPushForceDurationCounter -= Time.deltaTime;

			if (fireballPushForceDurationCounter < 0)
			{
				fireballPushForceEnd();
			}
        }
		else // direction
		{
			fireballDir = fireballDirValue;
			if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
		}
	}

	private void fireballPushForceStart()
	{
		if (isJumping)
		{
			jumpEnd();
			if(jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		mySetGravity(0, myGravityMaxSpeed);
		mySetVy(0);
		isFireballPushForceAdding = true;
		fireballPushForceDurationCounter = fireballPushForceDuration;

		//freeze frame
	}

	private void fireballPushForceEnd()
	{
		isFireballPushForceAdding = false;
		fireballPushForceDurationCounter = 0;
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeCoroutine = StartCoroutine(fireballHangTime(fireballPushForceHangTimeDuration));
	}

	public void fireballExplode(Vector2 localVelocity, float frictionLessDuration)
	{
		fireballPushForceEnd();
		myImpulseAcceleration(localVelocity);

		isFrictionActive = false;
		if(myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);
		myFrictionLessCoroutine = StartCoroutine(myFrictionLess(frictionLessDuration));
	}

	private void fireballChargeMain()
	{
		//recharge
		if (onGround)
		{
			fireballCurrentCharges = fireballMaxCharges;
		}

		//display
		fireballMeter.transform.localScale = new Vector3(1, (float)fireballCurrentCharges / fireballMaxCharges, 1);
	}

	public void fireballInput(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			fireballKeyValue = 2;
		}

		if (ctx.canceled)
		{
			fireballKeyValue = -1;
		}
	}

	public void fireballDirInput(InputAction.CallbackContext ctx)
	{
		//if (!isFireballPushForceAdding) fireballDir = ctx.ReadValue<Vector2>();
		fireballDirValue = ctx.ReadValue<Vector2>();
	}

	IEnumerator fireballPreInput(float t)
	{
		while (t > 0)
		{
			t -= Time.deltaTime;
			
			if (canCastFireball())
			{
				fireballStart();
				yield break;
			}
			
			yield return null;
		}
	}

	IEnumerator fireballHangTime(float t) // after fireball pushforce ends, player will enter this stage, which they will have no gravity
	{
		while(t > 0)
		{
			t -= Time.deltaTime;
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
	}



	#endregion


}
