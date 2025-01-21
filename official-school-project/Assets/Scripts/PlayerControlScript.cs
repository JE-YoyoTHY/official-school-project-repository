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
	private LogicScript logic;

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
	private sbyte fireballMouseValue;
	private bool fireballCastByKeyboard;
	[SerializeField] private int fireballMaxCharges;
	private int fireballCurrentCharges;
	private Vector2 fireballDir; // the true dir
	private Vector2 fireballDirValue; // store input value
	private Vector2 fireballMouseDirValue;
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

	//freeze frame
	private Vector2 freezeVelocity;

	//levels
	private LevelManagerScript currentLevel;
	private const int killZoneLayer = 7;
	private const int levelTriggerLayer = 8;

	#endregion



	// Start is called before the first frame update
	void Start()
    {
        //init
		rb = GetComponent<Rigidbody2D>();
		groundTrigger = transform.GetChild(0).GetComponent<PlayerGroundTriggerScript>();
		fireballMeter = transform.GetChild(1).gameObject;
		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

		myGravityScale = myNormalGravityScale;
		myGravityMaxSpeed = myNormalGravityMaxSpeed;
		isFrictionActive = true;

		fireballDirLastHorizontal = 1;

    }

    // Update is called once per frame
    void Update()
    {
		inputMain();
		if (!logic.isFreeze())
		{
			groundHitCheckMain();
			myGravityMain();
			moveMain();
			jumpMain();
			fireballMain();

			myFrictionMain();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
		
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
		if (!isFireballPushForceAdding && !logic.isFreeze()) return true;
		else return false;
	}

	/*public void moveInput(InputAction.CallbackContext ctx)
	{
		moveKeyValue = (sbyte)ctx.ReadValue<float>();
	}*/

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
			if (!logic.isFreeze())
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
		/*if(jumpKeyValue == 2)
		{
			jumpKeyValue = 1;
			if(jumpCoroutine != null)
			{
				StopCoroutine(jumpCoroutine);
			}
			jumpCoroutine = StartCoroutine(jumpPreInput(jumpPreInputTime));
		}*/

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
		if (!isJumping && onGround && !isFireballPushForceAdding && !logic.isFreeze()) return true;
		else return false;
	}

	/*public void jumpInput(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			jumpKeyValue = 2;
		}

		if (ctx.canceled)
		{
			jumpKeyValue = -1;
		}
	}*/

	IEnumerator jumpPreInput(float t) //t for time
	{
		while(t > 0)
		{
			//if (!logic.isFreeze())
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
			if (!logic.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		leaveGround();
	}

	#endregion

	#region fireball

	private void fireballMain()
	{
		/*if(fireballKeyValue == 2)
		{
			fireballKeyValue = 1;

			if(fireballInputCoroutine != null)
			{
				StopCoroutine(fireballInputCoroutine);
			}
			fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime));	
		}*/

		fireballPushForceMain();
		fireballChargeMain();
	}

	/* 當玩家按下按鍵後且canCastFireball執行
	 * --進入凍結幀
	 * --凍結幀結束後執行fireballSummon and fireballPushStart
	 */
	private void fireballStart() 
	{
		isFireballPushForceAdding = true;
		fireballDir = fireballCastByKeyboard ? fireballDir : fireballMouseDirValue;

		//freeze frame
		freezeStart(fireballCastFreezeTime);

		//coroutine
		StartCoroutine(fireballStartAfterFreezeTime());
	}

	private void fireballSummon()
	{
		GameObject summonedFireball = null;
		summonedFireball = Instantiate(fireballPrefab, transform.position, transform.rotation);

		if (fireballDir.magnitude > 0) summonedFireball.GetComponent<FireballScript>().summon(fireballDir);
		else summonedFireball.GetComponent<FireballScript>().summon(new Vector2(fireballDirLastHorizontal, 0));

		fireballCurrentCharges--;
		//fireballPushForceStart();
	}

	private bool canCastFireball()
	{
		if (fireballCurrentCharges > 0 && !isFireballPushForceAdding && !logic.isFreeze()) return true;
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
		/*else // direction
		{
			fireballDir = fireballDirValue;
			if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
		}*/
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
		//isFireballPushForceAdding = true;
		fireballPushForceDurationCounter = fireballPushForceDuration;

	}

	private void fireballPushForceEnd()
	{
		isFireballPushForceAdding = false;
		fireballPushForceDurationCounter = 0;
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeCoroutine = StartCoroutine(fireballHangTime(fireballPushForceHangTimeDuration));
	}

	public void fireballExplode(Vector2 localVelocity, float frictionLessDuration, float localFreezeTime)
	{
		fireballPushForceEnd();
		myImpulseAcceleration(localVelocity);

		isFrictionActive = false;
		if(myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);
		myFrictionLessCoroutine = StartCoroutine(myFrictionLess(frictionLessDuration));
		freezeStart(localFreezeTime);
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

	/*public void fireballInput(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			fireballKeyValue = 2;
		}

		if (ctx.canceled)
		{
			fireballKeyValue = -1;
		}
	}*/

	/*public void fireballDirInput(InputAction.CallbackContext ctx)
	{
		//if (!isFireballPushForceAdding) fireballDir = ctx.ReadValue<Vector2>();
		fireballDirValue = ctx.ReadValue<Vector2>();
	}*/

	private void fireballMouseDir()
	{
		//Vector2 screenMid = new Vector2(Screen.width/2, Screen.height/2);
		//Vector2 deltaMousePos = new Vector2((float)Mouse.current.position.ReadValue().x - Screen.width / 2, (float)Mouse.current.position.ReadValue().y - Screen.height / 2);
		Vector3 deltaMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
		float localDeg = Vector2.Angle(Vector2.right, new Vector2(deltaMousePos.x, deltaMousePos.y));
		if (deltaMousePos.y < 0) localDeg = 360 - localDeg;
		
		if (localDeg >= 22.5 && localDeg < 67.5) fireballMouseDirValue = new Vector2(1,1).normalized;
		else if (localDeg >= 67.5 && localDeg < 112.5) fireballMouseDirValue = Vector2.up;
		else if (localDeg >= 112.5 && localDeg < 157.5) fireballMouseDirValue = new Vector2(-1, 1).normalized;
		else if (localDeg >= 157.5 && localDeg < 202.5) fireballMouseDirValue = Vector2.left;
		else if (localDeg >= 202.5 && localDeg < 247.5) fireballMouseDirValue = new Vector2(-1, -1).normalized;
		else if (localDeg >= 247.5 && localDeg < 297.5) fireballMouseDirValue = Vector2.down;
		else if (localDeg >= 297.5 && localDeg < 337.5) fireballMouseDirValue = new Vector2(1, -1).normalized;
		else fireballMouseDirValue = Vector2.right;
		/*float mouseSin = deltaMousePos.normalized.y;
		float mouseCos = deltaMousePos.normalized.x;

		//dir
		/*rightif (mouseCos > 0 && mouseCos > Mathf.Cos(22.5f * Mathf.Deg2Rad)) fireballMouseDirValue = Vector2.right;
		/*right-upelse if (mouseCos > 0 && mouseSin > 0 && mouseCos < Mathf.Cos(22.5f * Mathf.Deg2Rad)) fireballMouseDirValue = new Vector2;
		*/
	}

	IEnumerator fireballPreInput(float t, bool isCastByKeyboard)
	{
		while (t > 0)
		{
			//if (!logic.isFreeze())
				t -= Time.deltaTime;
			
			if (canCastFireball())
			{
				fireballCastByKeyboard = isCastByKeyboard;
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
			if (!logic.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
	}

	IEnumerator fireballStartAfterFreezeTime()
	{
		while(logic.isFreeze()) yield return null;
		fireballSummon();
		fireballPushForceStart();
	}


	#endregion

	#region freeze frame

	private void freezeStart(float t) //fv for freeze velocity
	{
		freezeVelocity = rb.velocity;
		logic.setFreezeTime(t);
	}

	public void freezeEnd()
	{
		rb.velocity = freezeVelocity;
	}


	#endregion

	#region level functions

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == killZoneLayer)
		{
			playerDeath();
		}

		if (collision.gameObject.layer == levelTriggerLayer)
		{
			if (currentLevel != collision.gameObject.transform.parent.gameObject.GetComponent<LevelManagerScript>())
			{
				if (currentLevel != null)
				{
					currentLevel.disableLevel();
				}
				currentLevel = collision.gameObject.transform.parent.gameObject.GetComponent<LevelManagerScript>();
				changeLevel();
			}
		}
	}

	private void changeLevel()
	{
		currentLevel.startLevel();
	}

	private void playerDeath()
	{
		currentLevel.restartLevel();
		rb.velocity = Vector2.zero;

		//reset
		//jump
		if (isJumping) jumpEnd();
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		//fireball
		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		GameObject[] fbs;
		fbs = GameObject.FindGameObjectsWithTag("Fireball");
		foreach (GameObject fb in fbs) Destroy(fb);

		//friction
		if (myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);

		//freeze frame
		if (logic.isFreeze()) logic.setFreezeTime(0);

		//physic state
		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		isFrictionActive = true;
	}

	#endregion


	//inputs region handles inputs function, namely set keyValue 2 -> 1, and trigger pre input
	#region inputs

	private void inputMain()
	{
		//jump
		if (jumpKeyValue == 2)
		{
			jumpKeyValue = 1;
			if (jumpCoroutine != null)
			{
				StopCoroutine(jumpCoroutine);
			}
			jumpCoroutine = StartCoroutine(jumpPreInput(jumpPreInputTime));
		}

		//fireball
		if (fireballKeyValue == 2)
		{
			fireballKeyValue = 1;

			if (fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
			fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime, true));
		}

		if(fireballMouseValue == 2)
		{
			fireballMouseValue = 1;

			if(fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
			fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime, false));
		}

		//fireball dir
		/*player can change fb dir in freeze frame, 
		case 1:玩家一直沒碰方向鍵 -> 玩家最後觸碰的水平方向
		case 2:玩家在凍結幀期間改變方向鍵 -> 玩家新按的方向
		case 3:玩家在凍結幀期間放開方向鍵 -> 玩家最後觸碰的方向
		 */
		if (!isFireballPushForceAdding)
		{
			fireballDir = fireballDirValue;
			//if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
		}
		else
		{
			if (logic.isFreeze() && fireballDirValue.magnitude != 0 && fireballCastByKeyboard) fireballDir = fireballDirValue;
			if (logic.isFreeze() && fireballMouseDirValue.magnitude != 0 && !fireballCastByKeyboard) fireballDir = fireballMouseDirValue;
		}
		if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;

		fireballMouseDir();
	}

	public void moveInput(InputAction.CallbackContext ctx)
	{
		moveKeyValue = (sbyte)ctx.ReadValue<float>();
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

	public void fireballMouseInput(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			fireballMouseValue = 2;
		}

		if (ctx.canceled)
		{
			fireballMouseValue = -1;
		}
	}
	#endregion
}
