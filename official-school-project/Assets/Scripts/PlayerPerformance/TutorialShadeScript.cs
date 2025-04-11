using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShadeScript : MonoBehaviour
{
	//variable
	private Rigidbody2D rb;


	//physic
	private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed
	private float myNormalGravityScale;
	private float myNormalGravityMaxSpeed;
	private float myFrictionAcceleration;
	private float myAdjustFriction; // when player has acceleration and speed greater than max, apply this force
	private float myNormalFrictionAcceleration;
	private float myNormalAdjustFriction;
	private bool isFrictionActive;

	//move
	private float moveMaxSpeed;
	private float moveAcceleration;
	private bool isMoving;
	private sbyte moveDir;
	private bool isMoveActive;

	//jump
	private float jumpStrength;
	private float jumpGravity;
	private float jumpMinSpeed;
	private bool isJumping;
	private Coroutine jumpExtraHangTimeCoroutine;
	private float jumpExtraHangTimeGravityScale;
	private bool isJumpActive;

	//fireball basic
	private GameObject fireballPrefab;
	private Vector2 fireballDir;
	private bool isFireballActive;

	//fireball push
	private float fireballPushForceAcceleration;
	private float fireballPushForceMaxSpeed;
	private float fireballPushForceDuration;
	private float fireballPushUpForceScale;
	private bool isFireballPushForceAdding;
	private float fireballPushForceDurationCounter;
	private float fireballPushForceHangTimeDuration;
	private Coroutine fireballHangTimeCoroutine;

	//fireball hangtime
	private float fireballHangTimeFrictionScale;
	private float fireballHangTimeMoveSameDirBoost;
	private float fireballHangTimeMoveDifferentDirDecrease;
	private sbyte fireballHangTimeMoveBoostDir; // if player try to move this dir, they'll get boost, else decrease

	//fireball explode
	private float fireballExplodeForce;
	private float fireballExplodeHorizontalScale;
	private float fireballExplodeMoveSameDirBoost;
	private float fireballExplodeMoveDifferentDirDecrease;
	private float fireballExplodeGuaranteeSpeedScale; // 火球爆炸的寶底速度，因為火球速度是用加的, 以這次要加的速度為基準
	private float fireballExplodeMaxSpeedScale; // 以explode force為基準, x y 分別處理
	private float fireballExplodeExtraPushUpForce;
	private float fireballExplodeExtraPushUpAngle; //如果方向向量的俯角與仰角在此數值之間，會給予一向上速度
	private float fireballExplodeDuration;
	private float fireballExplodeGravityScale;
	private float fireballExplodeFriction;
	private float fireballExplodeAdjustFriction;
	private float fireballExplodeMoveAccelerationScale;
	private bool isFireballExplodeForceAdding;
	private Coroutine fireballExplodeCoroutine;

	//freeze velocity
	private Vector2 freezeVelocity;

	//spring
	private bool isControlBySpring;
	private Coroutine springCoroutine;





	private void Awake()
	{
		//init
		rb = GetComponent<Rigidbody2D>();
		myGravityScale = myNormalGravityScale;
		myGravityMaxSpeed = myNormalGravityMaxSpeed;
		myFrictionAcceleration = myNormalFrictionAcceleration;
		myAdjustFriction = myNormalAdjustFriction;
		isFrictionActive = true;
		isMoveActive = true;
		isJumpActive = true;
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!LogicScript.instance.isFreeze())
		{
			moveMain();
		}
    }


	#region physics
	private void myAcceleration(Vector2 localAcceleration, Vector2 localMaxVelocity)
	{
		// x
		if (localAcceleration.x > 0)
		{
			if (rb.velocity.x < localMaxVelocity.x)
			{
				rb.AddForce(new Vector2(Mathf.Min(localAcceleration.x * Time.deltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.x > localMaxVelocity.x && isFrictionActive)
			{
				rb.AddForce(Mathf.Max(myAdjustFriction * Time.deltaTime * -1, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

			}
		}
		else if (localAcceleration.x < 0)
		{
			if (rb.velocity.x > localMaxVelocity.x)
			{
				rb.AddForce(new Vector2(Mathf.Max(localAcceleration.x * Time.deltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.x < localMaxVelocity.x && isFrictionActive)
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

			if (rb.velocity.y > localMaxVelocity.y && isFrictionActive)
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

			if (rb.velocity.y < localMaxVelocity.y && isFrictionActive)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.deltaTime, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}

		//idk if it is needed, but if not, camera might move down even player stands still
		//if (groundTrigger.onGround() && rb.velocity.y < 0)
		//	mySetVy(0);
	}

	private void myAccelerationWithFixedDeltatime(Vector2 localAcceleration, Vector2 localMaxVelocity)
	{
		// x
		if (localAcceleration.x > 0)
		{
			if (rb.velocity.x < localMaxVelocity.x)
			{
				rb.AddForce(new Vector2(Mathf.Min(localAcceleration.x * Time.fixedDeltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.x > localMaxVelocity.x && isFrictionActive)
			{
				rb.AddForce(Mathf.Max(myAdjustFriction * Time.fixedDeltaTime * -1, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

			}
		}
		else if (localAcceleration.x < 0)
		{
			if (rb.velocity.x > localMaxVelocity.x)
			{
				rb.AddForce(new Vector2(Mathf.Max(localAcceleration.x * Time.fixedDeltaTime, localMaxVelocity.x - rb.velocity.x), 0) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.x < localMaxVelocity.x && isFrictionActive)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.fixedDeltaTime, localMaxVelocity.x - rb.velocity.x) * Vector2.right * rb.mass, ForceMode2D.Impulse);

			}
		}

		// y
		if (localAcceleration.y > 0)
		{
			if (rb.velocity.y < localMaxVelocity.y)
			{
				rb.AddForce(new Vector2(0, Mathf.Min(localAcceleration.y * Time.fixedDeltaTime, localMaxVelocity.y - rb.velocity.y)) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.y > localMaxVelocity.y && isFrictionActive)
			{
				rb.AddForce(Mathf.Max(myAdjustFriction * Time.fixedDeltaTime * -1, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}
		else if (localAcceleration.y < 0)
		{
			if (rb.velocity.y > localMaxVelocity.y)
			{
				rb.AddForce(new Vector2(0, Mathf.Max(localAcceleration.y * Time.fixedDeltaTime, localMaxVelocity.y - rb.velocity.y)) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.y < localMaxVelocity.y && isFrictionActive)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.fixedDeltaTime, localMaxVelocity.y - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}

		//idk if it is needed, but if not, camera might move down even player stands still
		//if (groundTrigger.onGround() && rb.velocity.y < 0)
		//	mySetVy(0);
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

	private void mySetFriction(float localFriction, float localAdjustFriction)
	{
		myFrictionAcceleration = localFriction;
		myAdjustFriction = localAdjustFriction;
	}

	#endregion


	#region move
	// set move dir to start move

	private void moveMain() //set move dir
	{
		if (moveDir != 0)
		{
			if (canMove())
			{
				isMoving = true;

				//normal
				if (fireballHangTimeMoveBoostDir == 0 && !isFireballExplodeForceAdding)
				{
					if (moveDir * rb.velocity.x >= 0) // same direction
					{
						myAcceleration(new Vector2(moveAcceleration * moveDir, 0), new Vector2(moveMaxSpeed * moveDir, 0));
					}
					else  // moveDir * rb.velocity.x < 0, 不同方向
					{
						/* inspired by celeste, that player is hard to turn around in air
						 * and i dont want player is harder to stop when they try to turn than stop moving
						 * so i canceled turn speed scale, and add friction acceleration instead
						 * but that only happen when player is on ground
						 */
						//myAcceleration(new Vector2(moveAcceleration * moveDir * moveTurnSpeedScale, 0), new Vector2(moveMaxSpeed * moveDir, 0));
						/*if (onGround)*/ myAcceleration(new Vector2((moveAcceleration + myFrictionAcceleration) * moveDir, 0), new Vector2(moveMaxSpeed * moveDir, 0));
						//else myAcceleration(new Vector2(moveAcceleration * moveDir, 0), new Vector2(moveMaxSpeed * moveDir, 0));
					}
				}
				else if (fireballHangTimeMoveBoostDir != 0)// fireball hang time boost
				{
					if (moveDir * fireballHangTimeMoveBoostDir > 0) // same dir
					{
						myAcceleration(new Vector2(moveAcceleration * moveDir * fireballHangTimeMoveSameDirBoost, 0), new Vector2(moveMaxSpeed * moveDir * fireballHangTimeMoveSameDirBoost, 0));
					}
					else
					{
						myAcceleration(new Vector2(moveAcceleration * moveDir * fireballHangTimeMoveDifferentDirDecrease, 0), new Vector2(moveMaxSpeed * moveDir * fireballHangTimeMoveDifferentDirDecrease, 0));
					}
				}
				else if (isFireballExplodeForceAdding)
				{
					myAcceleration(new Vector2(moveAcceleration * moveDir * fireballExplodeMoveAccelerationScale, 0), new Vector2(moveMaxSpeed * moveDir, 0));
				}
			}
		}

		if(moveDir != 0 || !canMove())
		{
			isMoving = false;
		}
	}

	private bool canMove()
	{
		if (!isFireballPushForceAdding && !LogicScript.instance.isFreeze() && isMoveActive && !isControlBySpring) return true;
		else return false;
	}


	#endregion

	#region jump
	//call jump start to jump
	private void jumpMain()
	{
		if (isJumping)
		{
			if (rb.velocity.y < jumpMinSpeed)
			{
				jumpEnd();
			}
		}
	}

	private void jumpStart() // call this
	{
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		isJumping = true;
		mySetVy(jumpStrength);
		mySetGravity(jumpGravity, myGravityMaxSpeed);
	}

	private void jumpEnd()
	{
		if (isFireballExplodeForceAdding)
		{
			if (rb.velocity.y < jumpMinSpeed) mySetVy(jumpMinSpeed);
		}
		else
		{
			mySetGravity(jumpGravity * jumpExtraHangTimeGravityScale, myGravityMaxSpeed);
			mySetVy(jumpMinSpeed);
		}

		if (isJumping)
		{
			isJumping = false;
		}

		if (jumpExtraHangTimeCoroutine != null)
		{
			StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		jumpExtraHangTimeCoroutine = StartCoroutine(jumpExtraHangTime());
	}


	IEnumerator jumpExtraHangTime()
	{
		while (rb.velocity.y > jumpMinSpeed * -1)
		{
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
	}


	#endregion

	#region fireball
	//call fireball start to shoot fireball
	private void fireballMain()
	{
		fireballPushForceMain();
	}

	private void fireballStart() // call this
	{
		isFireballPushForceAdding = true;
		fireballSummon();
		fireballPushForceStart();
	}

	private void fireballSummon()
	{
		GameObject summonedFireball =  Instantiate(fireballPrefab, transform.position, transform.rotation);
		summonedFireball.GetComponent<FireballScript>().summon(fireballDir);
		
	}

	private void fireballPushForceMain()
	{

		if (isFireballPushForceAdding)
		{
			
			Vector2 pushDir = fireballDir.normalized;
			if (pushDir.y < 0) pushDir = new Vector2(fireballDir.x, fireballDir.y * fireballPushUpForceScale);
			myAcceleration(pushDir * fireballPushForceAcceleration * -1, pushDir * fireballPushForceMaxSpeed * -1);
			
			fireballPushForceDurationCounter -= Time.deltaTime;

			if (fireballPushForceDurationCounter < 0)
			{
				fireballPushForceEnd();
			}
		}
	}

	private void fireballPushForceStart()
	{
		isMoving = false;

		if (isJumping)
		{
			jumpEnd();
			if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		springEnd();
		mySetGravity(0, myGravityMaxSpeed);

		//set player's velocity, currently, i want to set player vy to 0. vx to 0 if there's no horizontal force
		/*if(rb.velocity.y < 0)*/
		mySetVy(0);
		if (fireballDir.x == 0) mySetVx(0);

		//isFireballPushForceAdding = true;
		fireballPushForceDurationCounter = fireballPushForceDuration;

		isFrictionActive = true; isMoveActive = true; isJumpActive = true;

		//move boost
		if (fireballDir.x > 0) fireballHangTimeMoveBoostDir = -1;
		else if (fireballDir.x < 0) fireballHangTimeMoveBoostDir = 1;
		else fireballHangTimeMoveBoostDir = 0;

		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
	}

	private void fireballPushForceEnd()
	{
		isFireballPushForceAdding = false;
		fireballPushForceDurationCounter = 0;
		mySetFriction(myNormalFrictionAcceleration * fireballHangTimeFrictionScale, myNormalAdjustFriction * fireballHangTimeFrictionScale);
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeCoroutine = StartCoroutine(fireballHangTime(fireballPushForceHangTimeDuration));
	}

	public void fireballExplodeStart(Vector2 localVelocity, Vector2 fireballVelocity)
	{
		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		fireballExplodeCoroutine = StartCoroutine(fireballExplode(localVelocity, fireballVelocity));
	}

	IEnumerator fireballExplode(Vector2 localVelocity, Vector2 fireballVelocity) // will end fireball push force and enter hangtime
	{


		//coroutine phase 2 -> apply explode force and do some setup

		fireballPushForceEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeMoveBoostDir = 0;


		/* extra push up force for horizontal only
		 * and because of this, jump will be disabled while explode duration
		 */
		bool addPushUpForce = (localVelocity.y >= Mathf.Sin(0 - fireballExplodeExtraPushUpAngle * Mathf.Deg2Rad) && localVelocity.y <= Mathf.Sin(fireballExplodeExtraPushUpAngle * Mathf.Deg2Rad)) ? true : false;

		/* if player try to move to the same dir of the explode dir -> increase the speed
		 * if player try to move to the opposite dir of the explode fir -> decrease the speed
		 * else, the speed is multiplied by 1
		 */
		localVelocity = localVelocity * fireballExplodeForce;
		localVelocity = new Vector2(localVelocity.x * fireballExplodeHorizontalScale, localVelocity.y);

		if (moveDir * localVelocity.x > 0) localVelocity = new Vector2(localVelocity.x * fireballExplodeMoveSameDirBoost, localVelocity.y);
		else if (moveDir * localVelocity.x < 0) localVelocity = new Vector2(localVelocity.x * fireballExplodeMoveDifferentDirDecrease, localVelocity.y);


		/* add fireball's Velocity if player was hit by fireball, 
		 * this line of code's position will affect how player's action affect fb's velocity boost,
		 * currently, i decide to add fb velocity last, so it wont be affected by horizontal boost and move same dir boost
		 */
		//localVelocity = localVelocity + fireballVelocity; // i move it after max speed so that it can actually work

		//push up force
		if (addPushUpForce) localVelocity += Vector2.up * fireballExplodeExtraPushUpForce;

		myImpulseAcceleration(localVelocity);
		//rb.velocity = localVelocity;

		//guarantee speed
		if ((localVelocity.x > 0 && rb.velocity.x < localVelocity.x * fireballExplodeGuaranteeSpeedScale) || (localVelocity.x < 0 && rb.velocity.x > localVelocity.x * fireballExplodeGuaranteeSpeedScale)) rb.velocity = new Vector2(localVelocity.x * fireballExplodeGuaranteeSpeedScale, rb.velocity.y);
		if ((localVelocity.y > 0 && rb.velocity.y < localVelocity.y * fireballExplodeGuaranteeSpeedScale) || (localVelocity.y < 0 && rb.velocity.y > localVelocity.y * fireballExplodeGuaranteeSpeedScale)) rb.velocity = new Vector2(rb.velocity.x, localVelocity.y * fireballExplodeGuaranteeSpeedScale);
		//max speed
		if (localVelocity.x > 0 && rb.velocity.x > fireballExplodeForce * fireballExplodeMaxSpeedScale * fireballExplodeHorizontalScale) mySetVx(fireballExplodeForce * fireballExplodeMaxSpeedScale * fireballExplodeHorizontalScale);
		if (localVelocity.x < 0 && rb.velocity.x < fireballExplodeForce * fireballExplodeMaxSpeedScale * fireballExplodeHorizontalScale * -1) mySetVx(fireballExplodeForce * fireballExplodeMaxSpeedScale * fireballExplodeHorizontalScale * -1);
		if (localVelocity.y > 0 && rb.velocity.y > fireballExplodeForce * fireballExplodeMaxSpeedScale) mySetVy(fireballExplodeForce * fireballExplodeMaxSpeedScale);
		if (localVelocity.y < 0 && rb.velocity.y < fireballExplodeForce * fireballExplodeMaxSpeedScale * -1) mySetVy(fireballExplodeForce * fireballExplodeMaxSpeedScale * -1);

		//if hit by fireball (or fireball velocity = 0)
		//print(fireballVelocity + " <-fb ; local -> " + localVelocity);
		//localVelocity = localVelocity + fireballVelocity;
		myImpulseAcceleration(fireballVelocity);

		isFireballExplodeForceAdding = true;

		mySetGravity(fireballExplodeGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(fireballExplodeFriction, fireballExplodeAdjustFriction);


		//coroutine phase 3 -> wait until duration end
		float t = fireballExplodeDuration;
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}

		//coroutine phase 4 -> end
		fireballExplodeEnd();

	}

	private void fireballExplodeEnd()
	{
		isFireballExplodeForceAdding = false;

		isMoveActive = true; isJumpActive = true;

		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);

		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
	}

	IEnumerator fireballHangTime(float t) // after fireball pushforce ends, player will enter this stage, which they will have no gravity
	{
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
		fireballHangTimeMoveBoostDir = 0;
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
	}

	#endregion

	#region freeze frame

	public void freezeStart()
	{
		freezeVelocity = rb.velocity;
		rb.velocity = Vector2.zero;
		//logic.setFreezeTime(t);
	}

	public void freezeEnd()
	{
		rb.velocity = freezeVelocity;
	}


	#endregion

	#region level objects


	/*if player's move ability is removed, 
	 * during the remove time
	 * player cant move and will have a special gravity and friction
	 */
	public void springPush(Vector2 localForce, bool isControlRemoved, float springDuration, float springGravityScale, float springFriction/*, Vector3 springPos, Vector3 springTargetPos*/)
	{
		//reset player state
		isMoving = false;
		if (isJumping) jumpEnd();
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);

		if (isFireballExplodeForceAdding) fireballExplodeEnd();


		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
		fireballHangTimeMoveBoostDir = 0;
		isFrictionActive = true;

		//push
		rb.velocity = localForce;

		if (isControlRemoved)
		{
			if (springCoroutine != null) StopCoroutine(springCoroutine);
			springCoroutine = StartCoroutine(springControlless(localForce, springDuration, springGravityScale, springFriction));
		}
	}

	IEnumerator springControlless(Vector2 localForce, float springDuration, float springGravityScale, float springFriction /*Vector3 springPos, Vector3 springTargetPos*/)
	{
		isControlBySpring = true;
		mySetGravity(springGravityScale, myNormalGravityMaxSpeed);
		//mySetFriction(myNormalFrictionAcceleration * 0, myNormalAdjustFriction);
		mySetFriction(springFriction, myNormalAdjustFriction);
		rb.velocity = localForce;

		while (springDuration > 0)
		{
			if (!LogicScript.instance.isFreeze())
				springDuration -= Time.deltaTime;

			if (localForce.x > 0 && rb.velocity.x < moveMaxSpeed) mySetVx(moveMaxSpeed);
			if (localForce.x < 0 && rb.velocity.x > moveMaxSpeed * -1) mySetVx(moveMaxSpeed * -1);

			yield return null;
		}

		springEnd();
	}

	private void springEnd()
	{
		isControlBySpring = false;
		if (springCoroutine != null) StopCoroutine(springCoroutine);
		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);

	}

	#endregion
}
