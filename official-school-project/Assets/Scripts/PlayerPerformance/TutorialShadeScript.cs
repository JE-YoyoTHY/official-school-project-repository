using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShadeScript : MonoBehaviour
{

	//variable
	private Rigidbody2D rb;
	private Collider2D coll;


	//physic
	private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed
	//private float PlayerControlScript.instance.m_myNormalGravityScale;
	//private float PlayerControlScript.instance.m_myNormalGravityMaxSpeed;
	private float myFrictionAcceleration;
	private float myAdjustFriction; // when player has acceleration and speed greater than max, apply this force
	//private float PlayerControlScript.instance.m_myNormalFrictionAcceleration;
	//private float PlayerControlScript.instance.m_myNormalAdjustFriction;
	private bool isFrictionActive;

	//move
	//private float PlayerControlScript.instance.m_moveMaxSpeed;
	//private float PlayerControlScript.instance.m_moveAcceleration;
	public bool isMoving {  get; private set; }
	public sbyte moveDir {  get; private set; }
	private bool isMoveActive;

	//jump
	//private float PlayerControlScript.instance.m_jumpStrength;
	//private float PlayerControlScript.instance.m_jumpStrength;
	//private float PlayerControlScript.instance.m_jumpMinSpeed;
	public bool isJumping {  get; private set; }
	private Coroutine jumpExtraHangTimeCoroutine;
	//private float PlayerControlScript.instance.m_jumpExtraHangTimeGravityScale;
	//private bool isJumpActive;

	//fireball basic
	[SerializeField] private GameObject fireballPrefab;
	private Vector2 fireballDir;
	//private bool isFireballActive;

	//fireball push
	//private float PlayerControlScript.instance.m_fireballPushForceAcceleration;
	//private float PlayerControlScript.instance.m_fireballPushForceMaxSpeed;
	//private float fireballPushForceDuration;
	//private float PlayerControlScript.instance.m_fireballPushUpForceScale;
	private bool isFireballPushForceAdding;
	private float fireballPushForceDurationCounter;
	//private float PlayerControlScript.instance.m_fireballPushForceHangTimeDuration;
	private Coroutine fireballHangTimeCoroutine;

	//fireball hangtime
	//private float PlayerControlScript.instance.m_fireballHangTimeFrictionScale;
	//private float PlayerControlScript.instance.m_fireballHangTimeMoveSameDirBoost;
	//private float fireballHangTimeMoveDifferentDirDecrease;
	private sbyte fireballHangTimeMoveBoostDir; // if player try to move this dir, they'll get boost, else decrease

	//fireball explode
	//private float PlayerControlScript.instance.m_fireballExplodeForce;
	//private float PlayerControlScript.instance.m_fireballExplodeHorizontalScale;
	//private float PlayerControlScript.instance.m_fireballExplodeMoveSameDirBoost;
	//private float PlayerControlScript.instance.m_fireballExplodeMoveDifferentDirDecrease;
	//private float PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale; // ���y�z�����_���t�סA�]�����y�t�׬O�Υ[��, �H�o���n�[���t�׬����
	//private float PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale; // �Hexplode force�����, x y ���O�B�z
	//private float PlayerControlScript.instance.m_fireballExplodeExtraPushUpForce;
	//private float PlayerControlScript.instance.m_fireballExplodeExtraPushUpAngle; //�p�G��V�V�q�������P�����b���ƭȤ����A�|�����@�V�W�t��
	//private float PlayerControlScript.instance.m_fireballExplodeDuration;
	//private float PlayerControlScript.instance.m_fireballExplodeGravityScale;
	//private float PlayerControlScript.instance.m_fireballExplodeFriction;
	//private float fireballExplodeAdjustFriction;
	//private float fireballExplodeMoveAccelerationScale;
	private bool isFireballExplodeForceAdding;
	private Coroutine fireballExplodeCoroutine;

	//freeze velocity
	private Vector2 freezeVelocity;

	//spring
	private bool isControlBySpring;
	private Coroutine springCoroutine;


	//Control
	public GameObject startPoint;
	public bool isWaiting;
	public bool tutorialStarted = false;
	private TutorialShadeControlTriggerScript currentController;
	[SerializeField] private TutorialShadeControlTriggerScript firstController;


	/*private void Awake()
	{
		//init
		rb = GetComponent<Rigidbody2D>();
		myGravityScale = PlayerControlScript.instance.m_myNormalGravityScale;
		myGravityMaxSpeed = PlayerControlScript.instance.m_myNormalGravityMaxSpeed;
		myFrictionAcceleration = PlayerControlScript.instance.m_myNormalFrictionAcceleration;
		myAdjustFriction = PlayerControlScript.instance.m_myNormalAdjustFriction;
		isFrictionActive = true;
		isMoveActive = true;
		isJumpActive = true;
	}*/


	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
		myGravityScale = PlayerControlScript.instance.m_myNormalGravityScale;
		myGravityMaxSpeed = PlayerControlScript.instance.m_myNormalGravityMaxSpeed;
		myFrictionAcceleration = PlayerControlScript.instance.m_myNormalFrictionAcceleration;
		myAdjustFriction = PlayerControlScript.instance.m_myNormalAdjustFriction;
		isFrictionActive = true;
		isMoveActive = true;
		//isJumpActive = true;

		myIgnoreCollision();

		//controller
		//currentController = firstController;
		//currentController.enterTrigger(this);
	}

    // Update is called once per frame
    void Update()
    {
		if (!LogicScript.instance.isFreeze())
		{
			moveMain();
			jumpMain();
			//fireballMain();

			//myGravityMain();
			//myFrictionMain();

			if (currentController != null)
			{
				currentController.stayTrigger();

				//print(currentController);
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void FixedUpdate()
	{
		if (!LogicScript.instance.isFreeze())
		{
			myFrictionMain();
			myGravityMain();

			addMoveForce();
			fireballMain();
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

	private void myIgnoreCollision()
	{
		//ignore collision
		Collider2D[] playerColls = PlayerControlScript.instance.GetComponents<Collider2D>();
		foreach (Collider2D playerColl in playerColls)
		{
			Physics2D.IgnoreCollision(coll, playerColl, true);
		}

	}

	#endregion

	#region natural force

	//friction
	private void myFrictionMain() // horizontal
	{
		if (!isMoving && isFrictionActive && !(isFireballPushForceAdding && fireballDir.x != 0))
		{
			if (rb.velocity.x < 0)
			{
				myAccelerationWithFixedDeltatime(new Vector2(myFrictionAcceleration * 1, 0), Vector2.zero);
			}

			if (rb.velocity.x > 0)
			{
				myAccelerationWithFixedDeltatime(new Vector2(myFrictionAcceleration * -1, 0), Vector2.zero);
			}
		}
	}


	//gravity
	private void myGravityMain()
	{
		if (!isJumping && !isFireballPushForceAdding && !isFireballExplodeForceAdding && !isControlBySpring)
		{
			if (rb.velocity.y < PlayerControlScript.instance.m_jumpMinSpeed && rb.velocity.y > -PlayerControlScript.instance.m_jumpMinSpeed)
				mySetGravity(PlayerControlScript.instance.m_jumpGravity * PlayerControlScript.instance.m_jumpExtraHangTimeGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
			else if (rb.velocity.y < -PlayerControlScript.instance.m_jumpMinSpeed)
				mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		}
		myAccelerationWithFixedDeltatime(Vector2.down * myGravityScale, Vector2.down * myGravityMaxSpeed);

	}

	#endregion



	#region move
	// set move dir to start move
	public void setMoveDir(sbyte dir)
	{
		moveDir = dir;
	}

	private void moveMain() //set move dir
	{
		//if (moveDir != 0)
		//{
		//	if (canMove())
		//	{
		//		isMoving = true;

		//		//normal
		//		if (fireballHangTimeMoveBoostDir == 0 && !isFireballExplodeForceAdding)
		//		{
		//			if (moveDir * rb.velocity.x >= 0) // same direction
		//			{
		//				myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
		//			}
		//			else  // moveDir * rb.velocity.x < 0, ���P��V
		//			{
		//				/* inspired by celeste, that player is hard to turn around in air
		//				 * and i dont want player is harder to stop when they try to turn than stop moving
		//				 * so i canceled turn speed scale, and add friction acceleration instead
		//				 * but that only happen when player is on ground
		//				 */
		//				//myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * moveTurnSpeedScale, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
		//				/*if (onGround)*/ myAcceleration(new Vector2((PlayerControlScript.instance.m_moveAcceleration + myFrictionAcceleration) * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
		//				//else myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
		//			}
		//		}
		//		else if (fireballHangTimeMoveBoostDir != 0)// fireball hang time boost
		//		{
		//			if (moveDir * fireballHangTimeMoveBoostDir > 0) // same dir
		//			{
		//				myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveSameDirBoost, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveSameDirBoost, 0));
		//			}
		//			else
		//			{
		//				myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveDifferentDirDecrease, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveDifferentDirDecrease, 0));
		//			}
		//		}
		//		else if (isFireballExplodeForceAdding)
		//		{
		//			myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballExplodeMoveAccelerationScale, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
		//		}
		//	}
		//}

		if(moveDir == 0 || !canMove())
		{
			isMoving = false;
		}
	}

	private void addMoveForce()
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
						myAccelerationWithFixedDeltatime(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
					}
					else  // moveDir * rb.velocity.x < 0, ���P��V
					{
						/* inspired by celeste, that player is hard to turn around in air
						 * and i dont want player is harder to stop when they try to turn than stop moving
						 * so i canceled turn speed scale, and add friction acceleration instead
						 * but that only happen when player is on ground
						 */
						//myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * moveTurnSpeedScale, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
						/*if (onGround)*/
						myAccelerationWithFixedDeltatime(new Vector2((PlayerControlScript.instance.m_moveAcceleration + myFrictionAcceleration) * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
						//else myAcceleration(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
					}
				}
				else if (fireballHangTimeMoveBoostDir != 0)// fireball hang time boost
				{
					if (moveDir * fireballHangTimeMoveBoostDir > 0) // same dir
					{
						myAccelerationWithFixedDeltatime(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveSameDirBoost, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveSameDirBoost, 0));
					}
					else
					{
						myAccelerationWithFixedDeltatime(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveDifferentDirDecrease, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir * PlayerControlScript.instance.m_fireballHangTimeMoveDifferentDirDecrease, 0));
					}
				}
				else if (isFireballExplodeForceAdding)
				{
					myAccelerationWithFixedDeltatime(new Vector2(PlayerControlScript.instance.m_moveAcceleration * moveDir * PlayerControlScript.instance.m_fireballExplodeMoveAccelerationScale, 0), new Vector2(PlayerControlScript.instance.m_moveMaxSpeed * moveDir, 0));
				}
			}
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
			if (rb.velocity.y < PlayerControlScript.instance.m_jumpMinSpeed)
			{
				jumpEnd();
			}
		}
	}

	public void jumpStart() // call this
	{
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		isJumping = true;
		mySetVy(PlayerControlScript.instance.m_jumpStrength);
		mySetGravity(PlayerControlScript.instance.m_jumpGravity, myGravityMaxSpeed);
	}

	private void jumpEnd()
	{
		if (isFireballExplodeForceAdding)
		{
			if (rb.velocity.y < PlayerControlScript.instance.m_jumpMinSpeed) mySetVy(PlayerControlScript.instance.m_jumpMinSpeed);
		}
		else
		{
			mySetGravity(PlayerControlScript.instance.m_jumpStrength * PlayerControlScript.instance.m_jumpExtraHangTimeGravityScale, myGravityMaxSpeed);
			mySetVy(PlayerControlScript.instance.m_jumpMinSpeed);
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
		while (rb.velocity.y > PlayerControlScript.instance.m_jumpMinSpeed * -1)
		{
			yield return null;
		}
		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, myGravityMaxSpeed);
	}


	#endregion

	#region fireball
	//call fireball start to shoot fireball
	private void fireballMain()
	{
		fireballPushForceMain();
	}

	public void fireballStart(Vector2 dir) // call this
	{
		fireballDir = dir.normalized;

		isFireballPushForceAdding = true;
		fireballSummon();
		fireballPushForceStart();
	}

	private void fireballSummon()
	{
		GameObject summonedFireball =  Instantiate(fireballPrefab, transform.position, transform.rotation);
		summonedFireball.GetComponent<TutorialFireballScript>().summon(fireballDir, this);
		
	}

	private void fireballPushForceMain()
	{

		if (isFireballPushForceAdding)
		{
			
			Vector2 pushDir = fireballDir.normalized;
			if (pushDir.y < 0) pushDir = new Vector2(fireballDir.x, fireballDir.y * PlayerControlScript.instance.m_fireballPushUpForceScale);
			myAccelerationWithFixedDeltatime(pushDir * PlayerControlScript.instance.m_fireballPushForceAcceleration * -1, pushDir * PlayerControlScript.instance.m_fireballPushForceMaxSpeed * -1);
			
			fireballPushForceDurationCounter -= Time.fixedDeltaTime;

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
		fireballPushForceDurationCounter = PlayerControlScript.instance.m_fireballPushForceDuration;

		isFrictionActive = true; isMoveActive = true; //isJumpActive = true;

		//move boost
		if (fireballDir.x > 0 && fireballDir.y == 0) fireballHangTimeMoveBoostDir = -1;
		else if (fireballDir.x < 0 && fireballDir.y == 0) fireballHangTimeMoveBoostDir = 1;
		else fireballHangTimeMoveBoostDir = 0;

		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);
	}

	private void fireballPushForceEnd()
	{
		isFireballPushForceAdding = false;
		fireballPushForceDurationCounter = 0;
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration * PlayerControlScript.instance.m_fireballHangTimeFrictionScale, PlayerControlScript.instance.m_myNormalAdjustFriction * PlayerControlScript.instance.m_fireballHangTimeFrictionScale);
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeCoroutine = StartCoroutine(fireballHangTime(PlayerControlScript.instance.m_fireballPushForceHangTimeDuration));
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
		 * and because of this, jump will be disabled while explode _duration
		 */
		bool addPushUpForce = (localVelocity.y >= Mathf.Sin(0 - PlayerControlScript.instance.m_fireballExplodeExtraPushUpAngle * Mathf.Deg2Rad) && localVelocity.y <= Mathf.Sin(PlayerControlScript.instance.m_fireballExplodeExtraPushUpAngle * Mathf.Deg2Rad)) ? true : false;

		/* if player try to move to the same dir of the explode dir -> increase the speed
		 * if player try to move to the opposite dir of the explode fir -> decrease the speed
		 * else, the speed is multiplied by 1
		 */
		localVelocity = localVelocity * PlayerControlScript.instance.m_fireballExplodeForce;
		localVelocity = new Vector2(localVelocity.x * PlayerControlScript.instance.m_fireballExplodeHorizontalScale, localVelocity.y);

		if (moveDir * localVelocity.x > 0) localVelocity = new Vector2(localVelocity.x * PlayerControlScript.instance.m_fireballExplodeMoveSameDirBoost, localVelocity.y);
		else if (moveDir * localVelocity.x < 0) localVelocity = new Vector2(localVelocity.x * PlayerControlScript.instance.m_fireballExplodeMoveDifferentDirDecrease, localVelocity.y);


		/* add fireball's Velocity if player was hit by fireball, 
		 * this line of code's position will affect how player's action affect fb's velocity boost,
		 * currently, i decide to add fb velocity last, so it wont be affected by horizontal boost and move same dir boost
		 */
		//localVelocity = localVelocity + fireballVelocity; // i move it after max speed so that it can actually work

		//push up force
		if (addPushUpForce) localVelocity += Vector2.up * PlayerControlScript.instance.m_fireballExplodeExtraPushUpForce;

		myImpulseAcceleration(localVelocity);
		//rb.velocity = localVelocity;

		//guarantee speed
		if ((localVelocity.x > 0 && rb.velocity.x < localVelocity.x * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale) || (localVelocity.x < 0 && rb.velocity.x > localVelocity.x * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale)) rb.velocity = new Vector2(localVelocity.x * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale, rb.velocity.y);
		if ((localVelocity.y > 0 && rb.velocity.y < localVelocity.y * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale) || (localVelocity.y < 0 && rb.velocity.y > localVelocity.y * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale)) rb.velocity = new Vector2(rb.velocity.x, localVelocity.y * PlayerControlScript.instance.m_fireballExplodeGuaranteeSpeedScale);
		//max speed
		if (localVelocity.x > 0 && rb.velocity.x > PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * PlayerControlScript.instance.m_fireballExplodeHorizontalScale) mySetVx(PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * PlayerControlScript.instance.m_fireballExplodeHorizontalScale);
		if (localVelocity.x < 0 && rb.velocity.x < PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * PlayerControlScript.instance.m_fireballExplodeHorizontalScale * -1) mySetVx(PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * PlayerControlScript.instance.m_fireballExplodeHorizontalScale * -1);
		if (localVelocity.y > 0 && rb.velocity.y > PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale) mySetVy(PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale);
		if (localVelocity.y < 0 && rb.velocity.y < PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * -1) mySetVy(PlayerControlScript.instance.m_fireballExplodeForce * PlayerControlScript.instance.m_fireballExplodeMaxSpeedScale * -1);

		//if hit by fireball (or fireball velocity = 0)
		//print(fireballVelocity + " <-fb ; local -> " + localVelocity);
		//localVelocity = localVelocity + fireballVelocity;
		myImpulseAcceleration(fireballVelocity);

		isFireballExplodeForceAdding = true;

		mySetGravity(PlayerControlScript.instance.m_fireballExplodeGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		mySetFriction(PlayerControlScript.instance.m_fireballExplodeFriction, PlayerControlScript.instance.m_fireballExplodeAdjustFriction);


		//coroutine phase 3 -> wait until _duration end
		float t = PlayerControlScript.instance.m_fireballExplodeDuration;
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

		isMoveActive = true; //isJumpActive = true;

		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);

		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);
	}

	IEnumerator fireballHangTime(float t) // after fireball pushforce ends, player will enter this stage, which they will have no gravity
	{
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, myGravityMaxSpeed);
		fireballHangTimeMoveBoostDir = 0;
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);
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


		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);
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
		mySetGravity(springGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		//mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration * 0, PlayerControlScript.instance.m_myNormalAdjustFriction);
		mySetFriction(springFriction, PlayerControlScript.instance.m_myNormalAdjustFriction);
		rb.velocity = localForce;

		while (springDuration > 0)
		{
			if (!LogicScript.instance.isFreeze())
				springDuration -= Time.deltaTime;

			if (localForce.x > 0 && rb.velocity.x < PlayerControlScript.instance.m_moveMaxSpeed) mySetVx(PlayerControlScript.instance.m_moveMaxSpeed);
			if (localForce.x < 0 && rb.velocity.x > PlayerControlScript.instance.m_moveMaxSpeed * -1) mySetVx(PlayerControlScript.instance.m_moveMaxSpeed * -1);

			yield return null;
		}

		springEnd();
	}

	private void springEnd()
	{
		isControlBySpring = false;
		if (springCoroutine != null) StopCoroutine(springCoroutine);
		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);

	}

	#endregion

	#region performance

	private void OnTriggerEnter2D(Collider2D collision)
	{
		TutorialShadeControlTriggerScript tutorialTrigger = collision.GetComponent<TutorialShadeControlTriggerScript>();
		if (tutorialTrigger != null)
		{
			/*if(tutorialTrigger.action == TutorialShadeAction.move)
			{
				if (currentController == null)
				{
					currentController = tutorialTrigger;
				}
				else if(tutorialTrigger.previousMoveTrigger == currentController)
				{
					currentController = tutorialTrigger;
				}
			}*/


			if (tutorialTrigger.previousMoveTrigger == currentController)
			{
				//print("shade enter tutorial");
				/*if(tutorialTrigger.action == TutorialShadeAction.move)
					currentController = tutorialTrigger;
				else tutorialTrigger.enterTrigger(this);*/
				tutorialTrigger.enterTrigger(this);

				//print(tutorialTrigger.action == TutorialShadeAction.move);
				//print(tutorialTrigger.action);

				if (tutorialTrigger.action == TutorialShadeAction.move)
				{
					currentController = tutorialTrigger;
					//print("action = move");
				}
					
			}

			if (currentController == null && tutorialTrigger.action == TutorialShadeAction.move)
			{
				currentController = tutorialTrigger;
				//print(currentController.gameObject);
			}
		}
	}

	public void resetTutorial(){
		transform.position = startPoint.transform.position;
		//isWaiting = true;
		tutorialStarted = true;
		currentController = firstController;
		currentController.enterTrigger(this);


		//reset

		rb.velocity = Vector2.zero;

		//jump
		if (isJumping) jumpEnd();
		//if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		//fireball
		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (isFireballExplodeForceAdding) fireballExplodeEnd();
		//if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		GameObject[] fbs;
		fbs = GameObject.FindGameObjectsWithTag("TutorialShadeFireball");
		foreach (GameObject fb in fbs) Destroy(fb);


		//physic state
		mySetGravity(PlayerControlScript.instance.m_myNormalGravityScale, PlayerControlScript.instance.m_myNormalGravityMaxSpeed);
		mySetFriction(PlayerControlScript.instance.m_myNormalFrictionAcceleration, PlayerControlScript.instance.m_myNormalAdjustFriction);
		fireballHangTimeMoveBoostDir = 0;
		//isFrictionActive = true; isMoveActive = true; isJumpActive = true; isFireballActive = true;

		//coroutine
		//if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
		//if (fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
		//if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		//if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		//if (moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
		//if (jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);
		//if (fireballLessCoroutine != null) StopCoroutine(fireballLessCoroutine);
		StopAllCoroutines();
		isFrictionActive = true; isMoveActive = true;// isJumpActive = true; isFireballActive = true;
	}

	// public void startLoop(){
	// 	isWaiting = false;
	// }

	#endregion


}
