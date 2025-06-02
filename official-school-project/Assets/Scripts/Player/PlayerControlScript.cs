using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86;

public class PlayerControlScript : MonoBehaviour
{

	#region singleton

	public static PlayerControlScript instance { get; private set; }

	private void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	#endregion


	//variables
	#region variable
	//reference
	private Rigidbody2D rb;
	private PlayerGroundTriggerScript groundTrigger;
	private GameObject fireballMeter;
	//private LogicScript logic;



    //physics
    private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed
	[SerializeField] private float myNormalGravityScale;
	[SerializeField] private float myNormalGravityMaxSpeed;
	private float myFrictionAcceleration;
	private float myAdjustFriction; // when player has acceleration and speed greater than max, apply this force
	[SerializeField] private float myNormalFrictionAcceleration;
	[SerializeField] private float myNormalAdjustFriction;
	private bool isFrictionActive;
	private Coroutine myFrictionLessCoroutine;

	[Header("Move")]
	[SerializeField] private float moveMaxSpeed;
	[SerializeField] private float moveAcceleration; // speed add per second
	public bool isMoving { get; private set; }
	public sbyte moveKeyValue { get; private set; } // also represent move dir
	//[SerializeField] private float moveTurnSpeedScale; i set to 2 before
	private bool isMoveActive;
	private Coroutine moveLessCoroutine;


	[Header("Jump")]
	[SerializeField] private float jumpStrength;
	[SerializeField] private float jumpGravity;
	[SerializeField] private float jumpMinSpeed;
	public bool isJumping { get; private set; }
	//private sbyte jumpKeyValue;
	[SerializeField] private float jumpPreInputTime;
	private Coroutine jumpCoroutine;
	private Coroutine jumpExtraHangTimeCoroutine;
	[SerializeField] private float jumpExtraHangTimeGravityScale;
	private bool isJumpActive;
	private Coroutine jumpLessCoroutine;

	//ground hit
	[Header("Ground Hit")]
	[SerializeField] private float coyoteTime;
	private Coroutine coyoteTimeCoroutine;
	private bool onGround;
	private Coroutine fallDetectCoroutine;
	[SerializeField] private float fallParticleThreshold;
	private float currentMaxFallSpeedInAir;

	//fireball
	[Header("Fireball")]
	[SerializeField] private GameObject fireballPrefab;
	[SerializeField] private float fireballPreInputTime;
	//private sbyte fireballKeyValue;
	//private sbyte fireballMouseValue;
	//private sbyte fireballArrowKeyValue;
	private bool fireballCastByKeyboard;
	[SerializeField] private int fireballMaxCharges;
	private int fireballCurrentCharges;
	private Vector2 fireballDir; // the true dir
	//private Vector2 fireballDirValue; // store input value
	public Vector2 fireballMouseDirValue { get; private set; }
	private sbyte fireballDirLastHorizontal;
	private Coroutine fireballInputCoroutine;
	private bool isFireballActive;
	private Coroutine fireballLessCoroutine;

	private bool fireballPlayerGotten;

	[Header("Fireball Push")]
	[SerializeField] private float fireballPushForceAcceleration;
	[SerializeField] private float fireballPushForceMaxSpeed;
	[SerializeField] private float fireballPushForceDuration;
	[SerializeField] private float fireballPushUpForceScale; // make upward force less
	[SerializeField] private float fireballPushScreenShakeForce;
	private bool isFireballPushForceAdding;
	private float fireballPushForceDurationCounter;
	[SerializeField] private float fireballPushForceHangTimeDuration;
	[SerializeField] private float fireballCastFreezeTime;
	private Coroutine fireballHangTimeCoroutine;
	private bool fireballStartAfterFreezeTimeActive;

	[Header("Fireball Hangtime")]
	[SerializeField] private float fireballHangTimeFrictionScale;
	[SerializeField] private float fireballHangTimeMoveSameDirBoost;
	[SerializeField] private float fireballHangTimeMoveDifferentDirDecrease;
	private sbyte fireballHangTimeMoveBoostDir; // if player try to move this dir, they'll get boost, else decrease

	[Header("Fireball Explode")]
	[SerializeField] private float fireballExplodeForce;
	[SerializeField] private float fireballExplodeHorizontalScale;
	[SerializeField] private float fireballExplodeFreezeTime;
	[SerializeField] private float fireballExplodeMoveSameDirBoost;
	[SerializeField] private float fireballExplodeMoveDifferentDirDecrease;
	[SerializeField] private float fireballExplodeGuaranteeSpeedScale; // ���y�z�����_���t�סA�]�����y�t�׬O�Υ[��, �H�o���n�[���t�׬����
	[SerializeField] private float fireballExplodeMaxSpeedScale; // �Hexplode force�����, x y ���O�B�z
	[SerializeField] private float fireballExplodeExtraPushUpForce;
	[SerializeField] private float fireballExplodeExtraPushUpAngle; //�p�G��V�V�q�������P�����b���ƭȤ����A�|�����@�V�W�t��
	[SerializeField] private float fireballExplodeDuration;
	[SerializeField] private float fireballExplodeGravityScale;
	[SerializeField] private float fireballExplodeFriction;
	[SerializeField] private float fireballExplodeAdjustFriction;
	[SerializeField] private float fireballExplodeMoveAccelerationScale;
	private bool isFireballExplodeForceAdding;
	private Coroutine fireballExplodeCoroutine;

	[Header("Explode shade")]
	[SerializeField] private GameObject fireballExplodeShade;
	[SerializeField] private float fireballExplodeShadeInterval;
	[SerializeField] private int fireballExplodeShadeCount;
	private Coroutine fireballExplodeShadeCoroutine;

	//freeze frame
	private Vector2 freezeVelocity;

	//levels
	[Header("Levels")]
	[SerializeField] private GameObject deathSoul;
	[SerializeField] private float deathRespawnDelayTime;
	[SerializeField] private TransitionBlackHole deathHoleScript;
    [SerializeField] private TransitionBlackHole levelTransitionHoleScript;
    [SerializeField] private float playerControlRegainMinHoleRadius;
	private Coroutine deathRespawnDelayCoroutine;
	private Coroutine deathRespawnPlayerControlRegainCoroutine;
	public LevelManagerScript currentLevel { get; private set; }
	public bool isDying { get; private set; } = false;
	//private GameObject currentRespawnPoint; // idk �n��bPlayer�٬Olevel manager
	
	//const layer
	private const int killZoneLayer = 7;
	private const int levelTriggerLayer = 8;
	private const int respawnTriggerLayer = 9;

	//spring
	private bool isControlBySpring;
	private Coroutine springCoroutine;

	//performance
	private bool performanceWithNoGravity;
	private bool performanceWithNoFriction;
	#endregion

	#region public data

	public float m_myNormalGravityScale { get { return myNormalGravityScale; } }
	public float m_myNormalGravityMaxSpeed { get { return myNormalGravityMaxSpeed; } }

	public float m_myNormalFrictionAcceleration { get { return myNormalFrictionAcceleration; } }
	public float m_myNormalAdjustFriction { get { return myNormalAdjustFriction; } }

	public float m_moveMaxSpeed {  get { return moveMaxSpeed; } }
	public float m_moveAcceleration { get { return moveAcceleration; } }
	
	public float m_jumpStrength { get { return jumpStrength; } }
	public float m_jumpGravity { get { return jumpGravity; } }
	public float m_jumpMinSpeed { get { return jumpMinSpeed; } }

	public float m_jumpExtraHangTimeGravityScale { get { return jumpExtraHangTimeGravityScale; } }
	
	public float m_fireballPushForceAcceleration { get { return fireballPushForceAcceleration; } }
	public float m_fireballPushForceMaxSpeed { get { return fireballPushForceMaxSpeed; } }
	public float m_fireballPushForceDuration { get { return fireballPushForceDuration; } }
	public float m_fireballPushUpForceScale { get { return fireballPushUpForceScale; } }
	public float m_fireballPushForceHangTimeDuration { get { return fireballPushForceHangTimeDuration; } }
	
	public float m_fireballHangTimeFrictionScale { get { return fireballHangTimeFrictionScale; } }
	public float m_fireballHangTimeMoveSameDirBoost { get { return fireballHangTimeMoveSameDirBoost; } }
	public float m_fireballHangTimeMoveDifferentDirDecrease { get { return fireballHangTimeMoveDifferentDirDecrease; } }
	
	public float m_fireballExplodeForce { get { return fireballExplodeForce; } }
	public float m_fireballExplodeHorizontalScale { get { return fireballExplodeHorizontalScale; } }
	public float m_fireballExplodeMoveSameDirBoost { get { return fireballExplodeMoveSameDirBoost; } }
	public float m_fireballExplodeMoveDifferentDirDecrease { get { return fireballExplodeMoveDifferentDirDecrease; } }
	public float m_fireballExplodeGuaranteeSpeedScale { get { return fireballExplodeGuaranteeSpeedScale; } }
	public float m_fireballExplodeMaxSpeedScale { get { return fireballExplodeMaxSpeedScale; } }
	public float m_fireballExplodeExtraPushUpForce { get { return fireballExplodeExtraPushUpForce; } }
	public float m_fireballExplodeExtraPushUpAngle { get { return fireballExplodeExtraPushUpAngle; } }
	public float m_fireballExplodeDuration { get { return fireballExplodeDuration; } }
	public float m_fireballExplodeGravityScale { get { return fireballExplodeGravityScale; } }
	public float m_fireballExplodeFriction { get { return fireballExplodeFriction; } }
	public float m_fireballExplodeAdjustFriction { get { return fireballExplodeAdjustFriction; } }
	public float m_fireballExplodeMoveAccelerationScale {  get { return fireballExplodeMoveAccelerationScale; } }
	#endregion



	// Start is called before the first frame update
	void Start()
    {
        //init
		rb = GetComponent<Rigidbody2D>();
		groundTrigger = transform.GetChild(0).GetComponent<PlayerGroundTriggerScript>();
		fireballMeter = transform.GetChild(1).gameObject;
		//logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

		myGravityScale = myNormalGravityScale;
		myGravityMaxSpeed = myNormalGravityMaxSpeed;
		myFrictionAcceleration = myNormalFrictionAcceleration;
		myAdjustFriction = myNormalAdjustFriction;
		isFrictionActive = true;
		isMoveActive = true;
		isJumpActive = true;

		fireballDirLastHorizontal = 1;

		fireballPlayerGotten = false;

    }

    // Update is called once per frame
    void Update()
    {
		inputMain();
		if (!LogicScript.instance.isFreeze())
		{
			groundHitCheckMain();
			//myGravityMain();
			moveMain();
			jumpMain();
			fireballMain();

			//myFrictionMain();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
		
    }

	//gravity use fixed update can make jump height more constant
	private void FixedUpdate()
	{
		if (!LogicScript.instance.isFreeze())
		{
			myFrictionMain();
			myGravityMain();

			addMoveForce();
			addFireballPushForce();
			
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

			if(rb.velocity.x > localMaxVelocity.x && isFrictionActive)
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

	private void moveMain()
	{
		//if(moveKeyValue != 0)
		//{
		//	if (canMove())
		//	{
		//		isMoving = true;
		//		//normal
		//		if (fireballHangTimeMoveBoostDir == 0 && !isFireballExplodeForceAdding)
		//		{
		//			if (moveKeyValue * rb.velocity.x >= 0) // same direction
		//			{
		//				myAcceleration(new Vector2(moveAcceleration * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
		//			}
		//			else  // moveKeyValue * rb.velocity.x < 0, ���P��V
		//			{
		//				/* inspired by celeste, that player is hard to turn around in air
		//				 * and i dont want player is harder to stop when they try to turn than stop moving
		//				 * so i canceled turn speed scale, and add friction acceleration instead
		//				 * but that only happen when player is on ground
		//				 */
		//				//myAcceleration(new Vector2(moveAcceleration * moveKeyValue * moveTurnSpeedScale, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
		//				if(onGround) myAcceleration(new Vector2((moveAcceleration + myFrictionAcceleration) * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
		//				else myAcceleration(new Vector2(moveAcceleration * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
		//			}
		//		}
		//		else if (fireballHangTimeMoveBoostDir != 0)// fireball hang time boost
		//		{
		//			if(moveKeyValue * fireballHangTimeMoveBoostDir > 0) // same dir
		//			{
		//				myAcceleration(new Vector2(moveAcceleration * moveKeyValue * fireballHangTimeMoveSameDirBoost, 0), new Vector2(moveMaxSpeed * moveKeyValue * fireballHangTimeMoveSameDirBoost, 0));
		//			}
		//			else
		//			{
		//				myAcceleration(new Vector2(moveAcceleration * moveKeyValue * fireballHangTimeMoveDifferentDirDecrease, 0), new Vector2(moveMaxSpeed * moveKeyValue * fireballHangTimeMoveDifferentDirDecrease, 0));
		//			}
		//		}
		//		else if (isFireballExplodeForceAdding)
		//		{
		//			myAcceleration(new Vector2(moveAcceleration * moveKeyValue * fireballExplodeMoveAccelerationScale, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
		//		}
		//	}
		//}

		if(moveKeyValue == 0 || !canMove())
		{
			isMoving = false;
		}
	}

	private void addMoveForce()
	{
		if (moveKeyValue != 0)
		{
			if (canMove())
			{
				isMoving = true;
				//normal
				if (fireballHangTimeMoveBoostDir == 0 && !isFireballExplodeForceAdding)
				{
					if (moveKeyValue * rb.velocity.x >= 0) // same direction
					{
						myAccelerationWithFixedDeltatime(new Vector2(moveAcceleration * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
					}
					else  // moveKeyValue * rb.velocity.x < 0, ���P��V
					{
						/* inspired by celeste, that player is hard to turn around in air
						 * and i dont want player is harder to stop when they try to turn than stop moving
						 * so i canceled turn speed scale, and add friction acceleration instead
						 * but that only happen when player is on ground
						 */
						//myAcceleration(new Vector2(moveAcceleration * moveKeyValue * moveTurnSpeedScale, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
						if (onGround) myAccelerationWithFixedDeltatime(new Vector2((moveAcceleration + myFrictionAcceleration) * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
						else myAccelerationWithFixedDeltatime(new Vector2(moveAcceleration * moveKeyValue, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
					}
				}
				else if (fireballHangTimeMoveBoostDir != 0)// fireball hang time boost
				{
					if (moveKeyValue * fireballHangTimeMoveBoostDir > 0) // same dir
					{
						myAccelerationWithFixedDeltatime(new Vector2(moveAcceleration * moveKeyValue * fireballHangTimeMoveSameDirBoost, 0), new Vector2(moveMaxSpeed * moveKeyValue * fireballHangTimeMoveSameDirBoost, 0));
					}
					else
					{
						myAccelerationWithFixedDeltatime(new Vector2(moveAcceleration * moveKeyValue * fireballHangTimeMoveDifferentDirDecrease, 0), new Vector2(moveMaxSpeed * moveKeyValue * fireballHangTimeMoveDifferentDirDecrease, 0));
					}
				}
				else if (isFireballExplodeForceAdding)
				{
					myAccelerationWithFixedDeltatime(new Vector2(moveAcceleration * moveKeyValue * fireballExplodeMoveAccelerationScale, 0), new Vector2(moveMaxSpeed * moveKeyValue, 0));
				}
			}
		}
	}

	private bool canMove()
	{
		if (!isFireballPushForceAdding && !LogicScript.instance.isFreeze() && isMoveActive && !isControlBySpring && !PlayerPerformanceSystemScript.instance.isBeingControl) return true;
		else return false;
	}

	IEnumerator moveLess(float t)
	{
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		isMoveActive = true;
	}

	#endregion

	#region natural force, includes gravity and friction

	//friction
	private void myFrictionMain() // horizontal
	{
		if(!isMoving && isFrictionActive && !(isFireballPushForceAdding && fireballDir.x != 0) && !(PlayerPerformanceSystemScript.instance.isBeingControl && performanceWithNoFriction) /*&& !isControlBySpring*/)
		{
			if(rb.velocity.x < 0)
			{
				myAccelerationWithFixedDeltatime(new Vector2(myFrictionAcceleration * 1, 0), Vector2.zero);
			}

			if (rb.velocity.x > 0)
			{
				myAccelerationWithFixedDeltatime(new Vector2(myFrictionAcceleration * -1, 0), Vector2.zero);
			}
		}
	}

	IEnumerator myFrictionLess(float t) // t for time
	{
		while(t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		isFrictionActive = true;
	}


	//gravity
	private void myGravityMain()
	{
		if(!isJumping && !isFireballPushForceAdding && !isFireballExplodeForceAdding && !isControlBySpring)
		{
			if (rb.velocity.y < jumpMinSpeed && rb.velocity.y > -jumpMinSpeed)
				mySetGravity(jumpGravity * jumpExtraHangTimeGravityScale, myNormalGravityMaxSpeed);
			else if(rb.velocity.y < -jumpMinSpeed)
				mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		}

		if(!performanceWithNoGravity)
			myAccelerationWithFixedDeltatime(Vector2.down * myGravityScale, Vector2.down * myGravityMaxSpeed);


		/*if (!performanceWithNoGravity)
		{
			if (rb.velocity.y > -myGravityMaxSpeed)
			{
				rb.AddForce(new Vector2(0, Mathf.Max(-myGravityScale * Time.fixedDeltaTime, -myGravityMaxSpeed - rb.velocity.y)) * rb.mass, ForceMode2D.Impulse);
			}

			if (rb.velocity.y < -myGravityMaxSpeed && isFrictionActive)
			{
				rb.AddForce(Mathf.Min(myAdjustFriction * Time.fixedDeltaTime, -myGravityMaxSpeed - rb.velocity.y) * Vector2.up * rb.mass, ForceMode2D.Impulse);
			}
		}*/
		
		

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
		//if(jumpKeyValue == -1)
		if (InputManagerScript.instance.jumpInput == InputState.release)
		{
			if (isJumping)
			{
				jumpEnd();
			}
		}
	}

	private void jumpStart()
	{
		print("jump sfx");
		SFXManager.playSFXOneShot(SFXManager.SFXType.Jump);
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		isJumping = true;
		if(isFireballExplodeForceAdding)
		{
			if(rb.velocity.y < jumpStrength)
			{
				mySetVy(jumpStrength);
			}
		}
		else
		{
			mySetVy(jumpStrength);
			mySetGravity(jumpGravity, myGravityMaxSpeed);
		}

		
		leaveGround(true);
		currentMaxFallSpeedInAir = 0;

		//particle - jump
		GetComponent<ParticleCommonScript>().emitParticle();
	}

	private void jumpEnd()
	{
		if (isFireballExplodeForceAdding)
		{
			if(rb.velocity.y < jumpMinSpeed) mySetVy(jumpMinSpeed);
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

		if(jumpExtraHangTimeCoroutine != null)
		{
			StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		jumpExtraHangTimeCoroutine = StartCoroutine(jumpExtraHangTime());
	}

	private bool canJump()
	{
		if (!isJumping && onGround && !isFireballPushForceAdding && !LogicScript.instance.isFreeze() && isJumpActive && !isControlBySpring && !PlayerPerformanceSystemScript.instance.isBeingControl) return true;
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
			//if (!LogicScript.instance.isFreeze())
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

	IEnumerator jumpLess(float t)
	{
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		isJumpActive = true;
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

		//if (onGround)
		//{
		//	if(fallDetectCoroutine != null)
		//		StopCoroutine(fallDetectCoroutine);
		//	fallDetectCoroutine = null;
		//}

		if (!onGround && fallDetectCoroutine == null)
		{
			fallDetectCoroutine = StartCoroutine(fallDetect());
		}

	}

	private void leaveGround(bool byJump)
	{
		groundTrigger.leaveGround(byJump);
		onGround = false;

		if (coyoteTimeCoroutine != null)
		{
			StopCoroutine(coyoteTimeCoroutine);
			coyoteTimeCoroutine = null;
		}

        if (fallDetectCoroutine != null)
		{
			StopCoroutine(fallDetectCoroutine);
			fallDetectCoroutine = null;

            if (currentMaxFallSpeedInAir < fallParticleThreshold)
                GetComponent<ParticleCommonScript>().emitParticleWithIndex(0); // 0 for fall in particle common script
        }
            
        fallDetectCoroutine = StartCoroutine(fallDetect());

    }

	IEnumerator leaveGroundDelay(float t) // t for time, this function implement the coyote time
	{
		while(t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		leaveGround(false);
	}

	IEnumerator fallDetect()
	{
		//float vy1 = rb.velocity.y;
		//float vy2 = 0;
        //print(vy + "fall speed");
		currentMaxFallSpeedInAir = 0;
        while (!onGround)
		{
			//vy2 = vy1;
			//vy1 = rb.velocity.y;

			currentMaxFallSpeedInAir = Mathf.Min(currentMaxFallSpeedInAir, rb.velocity.y);
            yield return null;
		}

		//print(currentMaxFallSpeedInAir + "fall speed");

		if (currentMaxFallSpeedInAir < fallParticleThreshold)
			GetComponent<ParticleCommonScript>().emitParticleWithIndex(0); // 0 for fall in particle common script

		fallDetectCoroutine = null;
	}

	#endregion

	#region fireball

	private void fireballMain()
	{

		fireballPushForceMain();
		fireballChargeMain();
	}

	/* �����a���U�����BcanCastFireball����
	 * --�i�J�ᵲ�V
	 * --�ᵲ�V���������fireballSummon and fireballPushStart
	 */
	public void fireballStart(bool isCastByKeyboard) 
	{
		print("shoot fireball sfx");
		SFXManager.playSFXOneShot(SFXManager.SFXType.ShootFireball);
		fireballCastByKeyboard = isCastByKeyboard;

		isFireballPushForceAdding = true;
		fireballDir = fireballCastByKeyboard ? fireballDir : fireballMouseDirValue;
		//print(fireballDir);
		//freeze frame
		//freezeStart(fireballCastFreezeTime);
		LogicScript.instance.setFreezeTime(fireballCastFreezeTime);

		//coroutine
		//StartCoroutine(fireballStartAfterFreezeTime());
		fireballStartAfterFreezeTimeActive = true;
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
		if (fireballCurrentCharges > 0 && !isFireballPushForceAdding && !LogicScript.instance.isFreeze() && !PlayerPerformanceSystemScript.instance.isBeingControl && fireballPlayerGotten && isFireballActive) return true;
		else return false;
	}


	private void fireballPushForceMain()
	{
		if (fireballStartAfterFreezeTimeActive)
		{
			fireballSummon();
			fireballPushForceStart();
			fireballStartAfterFreezeTimeActive = false;
		}

		//if (isFireballPushForceAdding)
		//{
		//	if(fireballDir.magnitude == 0)
		//	{
		//		myAcceleration(new Vector2(fireballPushForceAcceleration * fireballDirLastHorizontal * -1,0), new Vector2(fireballPushForceMaxSpeed * fireballDirLastHorizontal * -1, 0));
		//	}
  //          else
  //          {
		//		Vector2 pushDir = fireballDir.normalized;
		//		if (pushDir.y < 0) pushDir = new Vector2(fireballDir.x, fireballDir.y * fireballPushUpForceScale);
		//		myAcceleration(pushDir * fireballPushForceAcceleration * -1, pushDir * fireballPushForceMaxSpeed * -1);
  //          }
		//	fireballPushForceDurationCounter -= Time.deltaTime;

		//	if (fireballPushForceDurationCounter < 0)
		//	{
		//		fireballPushForceEnd();
		//	}
        //}
		/*else // direction
		{
			fireballDir = fireballDirValue;
			if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
		}*/
	}

	private void addFireballPushForce()
	{
		if (isFireballPushForceAdding)
		{
			if (fireballDir.magnitude == 0)
			{
				myAccelerationWithFixedDeltatime(new Vector2(fireballPushForceAcceleration * fireballDirLastHorizontal * -1, 0), new Vector2(fireballPushForceMaxSpeed * fireballDirLastHorizontal * -1, 0));
			}
			else
			{
				Vector2 pushDir = fireballDir.normalized;
				if (pushDir.y < 0) pushDir = new Vector2(fireballDir.x, fireballDir.y * fireballPushUpForceScale);
				myAccelerationWithFixedDeltatime(pushDir * fireballPushForceAcceleration * -1, pushDir * fireballPushForceMaxSpeed * -1);
			}
			fireballPushForceDurationCounter -= Time.fixedDeltaTime;

			if (fireballPushForceDurationCounter < 0)
			{
				fireballPushForceEnd();
			}
		}
	}

	/* friction:
	 * when pushing, adjust friction is set to normal
	 * during hangtime, both friction value is multiplied by a hangtime multiplier
	 */
	private void fireballPushForceStart()
	{
		isMoving = false;

		if (isJumping)
		{
			jumpEnd();
			if(jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);
		}
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
        if (isFireballExplodeForceAdding) fireballExplodeEnd();
        springEnd();
		mySetGravity(0, myGravityMaxSpeed);
		
		//set player's velocity, currently, i want to set player vy to 0. vx to 0 if there's no horizontal force
		/*if(rb.velocity.y < 0)*/ mySetVy(0);
		if (fireballDir.x == 0) mySetVx(0);

		//isFireballPushForceAdding = true;
		fireballPushForceDurationCounter = fireballPushForceDuration;

		isFrictionActive = true; isMoveActive = true; isJumpActive = true;
		if (myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);
		if (moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
		if (jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);

		//move boost , only horizontal has now
		if (fireballDir.x > 0 && fireballDir.y == 0) fireballHangTimeMoveBoostDir = -1;
		else if (fireballDir.x < 0 && fireballDir.y == 0) fireballHangTimeMoveBoostDir = 1;
		else fireballHangTimeMoveBoostDir = 0;

		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);

		CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();

		impulseSource.m_DefaultVelocity = fireballDir;
		impulseSource.GenerateImpulseWithForce(fireballPushScreenShakeForce);

		//fall particle
		currentMaxFallSpeedInAir = 0;

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
		print("fireball explode sfx");
		SFXManager.playSFXOneShot(SFXManager.SFXType.FireballExplode);
		//freezeStart(fireballExplodeFreezeTime);
		LogicScript.instance.setFreezeTime(fireballExplodeFreezeTime);

		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		fireballExplodeCoroutine = StartCoroutine(fireballExplode(localVelocity, fireballVelocity));

		//shade
		if (fireballExplodeShadeCoroutine != null) StopCoroutine(fireballExplodeShadeCoroutine);
		fireballExplodeShadeCoroutine = StartCoroutine(fireballExplodeShadeMain());
	}

	//public void fireballExplode(Vector2 localVelocity/*, float frictionLessDuration, float localFreezeTime*/)
	IEnumerator fireballExplode(Vector2 localVelocity, Vector2 fireballVelocity) // will end fireball push force and enter hangtime
	{
		//coroutine phase 1 -> wait until freezeEnd
			while(LogicScript.instance.isFreeze()) yield return null;


		//coroutine phase 2 -> apply explode force and do some setup
		
			fireballPushForceEnd();
			if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
			fireballHangTimeMoveBoostDir = 0;

			//fall particle
			currentMaxFallSpeedInAir = 0;

			//if hit by fireball, reset vy if its below 0
			if (fireballVelocity.magnitude > 0 && rb.velocity.y < 0)
			{
				mySetVy(0);
			}

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

			if (moveKeyValue * localVelocity.x > 0) localVelocity = new Vector2(localVelocity.x * fireballExplodeMoveSameDirBoost, localVelocity.y);
			else if (moveKeyValue * localVelocity.x < 0) localVelocity = new Vector2(localVelocity.x * fireballExplodeMoveDifferentDirDecrease, localVelocity.y);

	
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
			//isMoving = false;
			//isMoveActive = false; isJumpActive = false;
			//if(moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
			//moveLessCoroutine = StartCoroutine(moveLess(fireballExplodeDuration));
			if(jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);
			jumpLessCoroutine = StartCoroutine(jumpLess(fireballExplodeDuration));

			mySetGravity(fireballExplodeGravityScale, myNormalGravityMaxSpeed);
			mySetFriction(fireballExplodeFriction, fireballExplodeAdjustFriction);
			if (localVelocity.y > 0) leaveGround(false); // for cancel coyote time


		//coroutine phase 3 -> wait until duration end
			float t = fireballExplodeDuration;
			while(t > 0)
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

		if (moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
		if (jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);
		isMoveActive = true; isJumpActive = true;

		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);

		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);

		//shade
		if (fireballExplodeShadeCoroutine != null) StopCoroutine(fireballExplodeShadeCoroutine);
	}

	private IEnumerator fireballExplodeShadeMain()
	{
		for(int i = 0; i < fireballExplodeShadeCount; i++){
			
			GameObject explodeShade = Instantiate(fireballExplodeShade, transform.position, transform.rotation);
			explodeShade.GetComponent<ExplodeShadeScript>().summon();	
			
			float t = fireballExplodeShadeInterval;
			while (t > 0){
				if(!LogicScript.instance.isFreeze()){
					t -= Time.deltaTime;
				}
				yield return null;
			}


		}
	}

	//i tried to write this part in fireballExplode
	/*IEnumerator fireballExplodeControlless(float t)
	{

	}*/

	private void fireballChargeMain()
	{
		//recharge
		if (onGround && fireballChargeNeeded() && !isFireballPushForceAdding && !isFireballExplodeForceAdding)
		{
			fireballCurrentCharges = fireballMaxCharges;
		}

		//display
		fireballMeter.transform.localScale = new Vector3(1, (float)fireballCurrentCharges / fireballMaxCharges, 1);

		//light, child 2 -> light
		if(transform.childCount > 2)
		{
			if (fireballCurrentCharges == fireballMaxCharges && fireballPlayerGotten) transform.GetChild(2).gameObject.SetActive(true);
			else transform.GetChild(2).gameObject.SetActive(false);
		}

		//indicator
		if (fireballCurrentCharges == fireballMaxCharges && fireballPlayerGotten && !isDying) PlayerFireballCountIndicatorScript.instance.setIndicatorActiveState(true);
		else PlayerFireballCountIndicatorScript.instance.setIndicatorActiveState(false);

    }

	public bool fireballChargeNeeded()
	{
		if (fireballCurrentCharges < fireballMaxCharges) return true;
		else return false;
	}

	public void fireballChargeGain(int localCharges)
	{
		fireballCurrentCharges += localCharges;
		fireballCurrentCharges = (fireballCurrentCharges > fireballMaxCharges) ? fireballMaxCharges : fireballCurrentCharges;
	}

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
			//if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			
			if (canCastFireball())
			{
				//fireballCastByKeyboard = isCastByKeyboard;
				fireballStart(isCastByKeyboard);
				yield break;
			}
			
			yield return null;
		}
	}

	IEnumerator fireballHangTime(float t) // after fireball pushforce ends, player will enter this stage, which they will have no gravity
	{
		while(t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		mySetGravity(myNormalGravityScale, myGravityMaxSpeed);
		fireballHangTimeMoveBoostDir = 0;
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
	}

	IEnumerator fireballLess(float t)
	{
		while (t > 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			yield return null;
		}
		isFireballActive = true;
	}

	/*IEnumerator fireballStartAfterFreezeTime()
	{
		while(LogicScript.instance.isFreeze()) yield return null;
		fireballSummon();
		fireballPushForceStart();
	}*/

	[ContextMenu("Get Fireball Ability")]
	public void fireballPlayerGetAbility()
	{
		fireballPlayerGotten = true;
	}

	#endregion

	#region freeze frame

	public void freezeStart()
	{
		freezeVelocity = rb.velocity;
		rb.velocity = Vector2.zero;
		//LogicScript.instance.setFreezeTime(t);
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
			if (collision.CompareTag("DeadZoneSpike"))
			{
				DeadZoneScript deadZone = collision.GetComponent<DeadZoneScript>();
				if (deadZone.noDirection || (deadZone.deadZoneDirection.x * rb.velocity.x <= 0 || deadZone.deadZoneDirection.y * rb.velocity.y <= 0))
				{
					if(deathRespawnDelayCoroutine == null)
					playerDeathDelayStart();
				}
			}
			else
			{
				if(deathRespawnDelayCoroutine == null)
				playerDeathDelayStart();
			}
			
		}

		if (collision.gameObject.layer == levelTriggerLayer)
		{
			/*if (currentLevel != collision.gameObject.transform.parent.parent.parent.gameObject.GetComponent<LevelManagerScript>())
			{
				if (currentLevel != null)
				{
					currentLevel.disableLevel();
				}
				currentLevel = collision.gameObject.transform.parent.parent.parent.gameObject.GetComponent<LevelManagerScript>();
				changeLevel();
			}*/
			if (collision.gameObject.name == "EnterTrigger")
			{
				if (currentLevel != null)
				{
					currentLevel.disableLevel();
				}
				currentLevel = collision.gameObject.transform.parent.parent.parent.gameObject.GetComponent<LevelManagerScript>();
				changeLevel();
			}

			if (collision.gameObject.name == "ExitTrigger")
			{
				if (currentLevel != null)
				{
					currentLevel.disableLevel();
				}

				LevelManagerScript levelManager = collision.gameObject.transform.parent.parent.parent.gameObject.GetComponent<LevelManagerScript>();

                if (levelManager.nextLevel == null)
				{
                    GameObject[] levelManagerScripts = GameObject.FindGameObjectsWithTag("LevelManager");
                    foreach (var levelManagerScript in levelManagerScripts)
                    {
                        if (levelManagerScript.GetComponent<LevelManagerScript>().levelNumber.x == levelManager.levelNumber.x + 1 &&
                            levelManagerScript.GetComponent<LevelManagerScript>().levelNumber.y == 1)
                        {
                            currentLevel = levelManagerScript.GetComponent<LevelManagerScript>();

                        }
                    }
                }
				else
				{
					currentLevel = levelManager.nextLevel;
				}

				//currentLevel = collision.gameObject.transform.parent.parent.parent.gameObject.GetComponent<LevelManagerScript>().nextLevel;
				changeLevel();

               
            }
		}

		if (collision.gameObject.layer == respawnTriggerLayer)
		{
			/*if(currentRespawnPoint != collision.gameObject.transform.GetChild(0).gameObject)
			{
				currentRespawnPoint = collision.gameObject.transform.GetChild(0).gameObject;
			}*/
			currentLevel.swapRespawnPoint(collision.gameObject.transform.GetChild(0).gameObject);
		}
	}

	private void changeLevel()
	{
		rb.velocity = Vector2.zero;
		currentLevel.startLevel();

		levelTransitionHoleScript.transform.parent.gameObject.SetActive(true);
		levelTransitionHoleScript.closeHole();
	}

	//reset state, call when hole close finish
	public void playerDeath()
	{
		rb.velocity = Vector2.zero;
		//transform.position = currentRespawnPoint.transform.position;
		currentLevel.restartLevel();
		//currentRespawnPoint.transform.parent.GetComponent<RespawnPointScript>().changeCameraAfterRespawn();

		//reset
		//jump
		if (isJumping) jumpEnd();
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		//fireball
		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (isFireballExplodeForceAdding) fireballExplodeEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		GameObject[] fbs;
		fbs = GameObject.FindGameObjectsWithTag("Fireball");
		foreach (GameObject fb in fbs) Destroy(fb);

		GameObject[] explosionShades = GameObject.FindGameObjectsWithTag("ExplosionShade");
		foreach(GameObject exp_shade in explosionShades) Destroy(exp_shade);

		//friction
		if (myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);

		//freeze frame
		if (LogicScript.instance.isFreeze()) LogicScript.instance.setFreezeTime(0);

		//physic state
		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
		fireballHangTimeMoveBoostDir = 0;
		//isFrictionActive = true; isMoveActive = true; isJumpActive = true; isFireballActive = true;

		//coroutine
		if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
		if(fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
		if(fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		if (moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
		if (jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);
		if (fireballLessCoroutine != null) StopCoroutine(fireballLessCoroutine);
        fallDetectCoroutine = null;
        StopAllCoroutines();


		PlayerPerformanceSystemScript.instance.controlEnd();


        //death soul
        GameObject soulInstance = Instantiate(deathSoul, transform.position, Quaternion.identity);
        soulInstance.GetComponent<PlayDeathSoulScript>().summon(false);
    }

	//call when hole close finish, called at the same time as player death,
	//but these two must be separate because enter new level dont call player death
	//regain player control when whole is opening -> call playerRespawnControlRegain coroutine
	public void playerRespawn()
	{
		deathRespawnDelayCoroutine = null;
        if (deathRespawnPlayerControlRegainCoroutine != null) StopCoroutine(deathRespawnPlayerControlRegainCoroutine);
		deathRespawnPlayerControlRegainCoroutine = StartCoroutine(playerRespawnControlRegain());

        //PlayerAnims.instance.GetComponent<SpriteRenderer>().enabled = true;
    }

	//call when player touch killzone, the actual death start
	private void playerDeathDelayStart()
	{
		if(deathRespawnDelayCoroutine != null) StopCoroutine(deathRespawnDelayCoroutine);
		deathRespawnDelayCoroutine = StartCoroutine(playerDeathDelayMain(deathRespawnDelayTime));

		isDying = true;

		//particle - player death
		GetComponent<ParticleCommonScript>().emitParticleWithIndex(1); // 1 for player death

		//death soul
		GameObject soulInstance = Instantiate(deathSoul, transform.position, Quaternion.identity);
		soulInstance.GetComponent<PlayDeathSoulScript>().summon(true);

		//player sprite
		PlayerAnims.instance.GetComponent<SpriteRenderer>().enabled = false;
	}

	//wait for a short time before the hole close, to play animation
	private IEnumerator playerDeathDelayMain(float t)
	{
		//initial
		isMoveActive = false;
		isJumpActive = false;
		isFrictionActive = false;
		isFireballActive = false;
		mySetGravity(0, 0);


		while(t >= 0)
		{
			rb.velocity = Vector2.zero;
			

			t -= Time.deltaTime;
			yield return null;
		}

		deathHoleScript.transform.parent.gameObject.SetActive(true);
		deathHoleScript.closeHole();

	}

	//to give player control while the hole is opening
	private IEnumerator playerRespawnControlRegain()
	{
		while (deathHoleScript.holeRect.sizeDelta.x < playerControlRegainMinHoleRadius)
		{
			yield return null;
		}

        isFrictionActive = true; isMoveActive = true; isJumpActive = true; isFireballActive = true;
		isDying = false; //currently is only to prevent getting wrong camera if player dies in swap trigger
    }


	#endregion

	#region level objects


	/*if player's move ability is removed, 
	 * during the remove time
	 * player cant move and will have a special gravity and friction
	 */
	public void springPush(Vector2 localForce, bool isControlRemoved, float springDuration, float springGravityScale, float springFriction/*, Vector3 springPos, Vector3 springTargetPos*/, float springFireballLessTime)
	{
		//reset player state
		isMoving = false;
		if (isJumping) jumpEnd();
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		
		if (isFireballExplodeForceAdding) fireballExplodeEnd();

		if (myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);

		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
		fireballHangTimeMoveBoostDir = 0;
		isFrictionActive = true;

		isFireballActive = false;
		if (fireballLessCoroutine != null) StopCoroutine(fireballLessCoroutine);
		fireballLessCoroutine = StartCoroutine(fireballLess(springFireballLessTime));

        //fall particle
        currentMaxFallSpeedInAir = 0;

        //push
        rb.velocity = localForce;
		fireballChargeGain(3);
		//transform.position = localPos;

		//hang time, maybe i need its own hang time duration
		/*mySetGravity(0, myGravityMaxSpeed);
		if (localForce.x > 0) fireballHangTimeMoveBoostDir = 1;
		else if (localForce.x < 0) fireballHangTimeMoveBoostDir = -1;
		else fireballHangTimeMoveBoostDir = 0;
		mySetFriction(myNormalFrictionAcceleration * fireballHangTimeFrictionScale, myNormalAdjustFriction * fireballHangTimeFrictionScale);

		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);
		fireballHangTimeCoroutine = StartCoroutine(fireballHangTime(fireballPushForceHangTimeDuration));*/

		if(isControlRemoved)
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

		while(springDuration > 0)
		{
			if(!LogicScript.instance.isFreeze())
				springDuration -= Time.deltaTime;

			if (localForce.x > 0 && rb.velocity.x < moveMaxSpeed) mySetVx(moveMaxSpeed);
			if (localForce.x < 0 && rb.velocity.x > moveMaxSpeed * -1) mySetVx(moveMaxSpeed * -1);

			yield return null;
		}

		//Vector3 deltaPos = springPos - transform.position;

		/*while (deltaPos.magnitude < (springTargetPos - springPos).magnitude)
		{
			deltaPos = springPos - transform.position;
			//rb.velocity = localForce;
			yield return null;
		}*/
		springEnd();
	}

	private void springEnd()
	{
		isControlBySpring = false;
		if(springCoroutine != null) StopCoroutine(springCoroutine);
		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
		//rb.velocity = rb.velocity / 2;
		//if (rb.velocity.x < 0 && rb.velocity.x < moveMaxSpeed * -1) mySetVx(moveMaxSpeed * -1);
		//if (rb.velocity.x > 0 && rb.velocity.x > moveMaxSpeed * 1) mySetVx(moveMaxSpeed);

		//jumpEnd();
	}

	#endregion

	#region performance

	public void performanceStart()
	{		
		//jump
		if (isJumping) jumpEnd();
		if (jumpExtraHangTimeCoroutine != null) StopCoroutine(jumpExtraHangTimeCoroutine);

		//fireball
		if (isFireballPushForceAdding) fireballPushForceEnd();
		if (isFireballExplodeForceAdding) fireballExplodeEnd();
		if (fireballHangTimeCoroutine != null) StopCoroutine(fireballHangTimeCoroutine);

		//friction
		if (myFrictionLessCoroutine != null) StopCoroutine(myFrictionLessCoroutine);

		//freeze frame
		//if (LogicScript.instance.isFreeze()) LogicScript.instance.setFreezeTime(0);

		//physic state
		mySetGravity(myNormalGravityScale, myNormalGravityMaxSpeed);
		mySetFriction(myNormalFrictionAcceleration, myNormalAdjustFriction);
		fireballHangTimeMoveBoostDir = 0;
		isFrictionActive = true; isMoveActive = true; isJumpActive = true;

		//coroutine
		if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
		if (fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
		if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		//if (fireballExplodeCoroutine != null) StopCoroutine(fireballExplodeCoroutine);
		if (moveLessCoroutine != null) StopCoroutine(moveLessCoroutine);
		if (jumpLessCoroutine != null) StopCoroutine(jumpLessCoroutine);
		if (fireballLessCoroutine != null) StopCoroutine(fireballLessCoroutine);
	}

	public void performanceGravity(bool localPerformanceWithNoGravity)
	{
		performanceWithNoGravity = localPerformanceWithNoGravity;
		if(performanceWithNoGravity)
			rb.velocity = Vector2.zero;
	}

	public void performanceFriction(bool localPerformanceWithNoFriction)
	{
		performanceWithNoFriction = localPerformanceWithNoFriction;
	}

	#endregion


	//inputs region handles inputs function, namely set keyValue 2 -> 1, and trigger pre input
	#region inputs

	private void inputMain()
	{
		//move
		moveKeyValue = (sbyte)InputManagerScript.instance.moveInput;

		//those function which will modify the value of Input Manager should only be executed when not beingControl
		if (!PlayerPerformanceSystemScript.instance.isBeingControl)
		{
			//jump
			if (InputManagerScript.instance.jumpInput == InputState.press)
			{
				InputManagerScript.instance.jumpInput = InputState.hold;
				if (jumpCoroutine != null)
				{
					StopCoroutine(jumpCoroutine);
				}
				jumpCoroutine = StartCoroutine(jumpPreInput(jumpPreInputTime));
			}

			//fireball
			if (InputManagerScript.instance.fireballCastByKeyboardInput == InputState.press)
			{
				InputManagerScript.instance.fireballCastByKeyboardInput = InputState.hold;
				if (fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
				fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime, true));
			}
			if (InputManagerScript.instance.fireballCastByMouseInput == InputState.press)
			{
				InputManagerScript.instance.fireballCastByMouseInput = InputState.hold;
				if (fireballInputCoroutine != null) StopCoroutine(fireballInputCoroutine);
				fireballInputCoroutine = StartCoroutine(fireballPreInput(fireballPreInputTime, false));
			}
		}
		

		//fireball dir
		//player can change fb dir in freeze frame,
		//case 1:
		//	���a�@���S�I��V��->���a�̫�Ĳ�I��������V
		//case 2:
		//	���a�b�ᵲ�V�������ܤ�V��->���a�s������V
		//case 3:
		//	���a�b�ᵲ�V������}��V��->���a�̫�Ĳ�I����V
		if (!isFireballPushForceAdding)
		{
			fireballDir = InputManagerScript.instance.fireballDirInput;
			//if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
		}
		else
		{
			if (LogicScript.instance.isFreeze() && InputManagerScript.instance.fireballDirInput.magnitude != 0 && fireballCastByKeyboard && !PlayerPerformanceSystemScript.instance.isBeingControl) fireballDir = InputManagerScript.instance.fireballDirInput;
			if (LogicScript.instance.isFreeze() && fireballMouseDirValue.magnitude != 0 && !fireballCastByKeyboard && !PlayerPerformanceSystemScript.instance.isBeingControl) fireballDir = fireballMouseDirValue;
		}
		if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;

			fireballMouseDir();

			//legacy
			/*
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
			player can change fb dir in freeze frame, 
			case 1:���a�@���S�I��V�� -> ���a�̫�Ĳ�I��������V
			case 2:���a�b�ᵲ�V�������ܤ�V�� -> ���a�s������V
			case 3:���a�b�ᵲ�V������}��V�� -> ���a�̫�Ĳ�I����V

			if (!isFireballPushForceAdding)
			{
				fireballDir = fireballDirValue;
				//if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;
			}
			else
			{
				if (LogicScript.instance.isFreeze() && fireballDirValue.magnitude != 0 && fireballCastByKeyboard) fireballDir = fireballDirValue;
				if (LogicScript.instance.isFreeze() && fireballMouseDirValue.magnitude != 0 && !fireballCastByKeyboard) fireballDir = fireballMouseDirValue;
			}
			if (fireballDir.x != 0) fireballDirLastHorizontal = (sbyte)fireballDir.x;

			fireballMouseDir();*/
		}

	/*public void moveInput(InputAction.CallbackContext ctx)
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
	}*/
	#endregion


	#region debug function

	public void testLevel(LevelManagerScript levelToBeTest)
	{
		currentLevel = levelToBeTest;
		changeLevel();
	}

	#endregion
}


