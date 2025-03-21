using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
	//variable
	private Rigidbody2D rb;
	private CircleCollider2D coll;
	//private PlayerControlScript player;
	//private LogicScript logic;

	private Vector2 moveDir;
	private float moveSpeed;
	[SerializeField] private float normalMoveSpeed;
	[SerializeField] private float fireballFriction; // reduce its speed to normal when speed is greater than normal, for example : spring
	//[SerializeField] private float explodeForce;
	[SerializeField] private float explodeRadius;
	//[SerializeField] private float explodeFrictionLessDuration; // or i should take over player's control
	//[SerializeField] private float explodeHorizontalScale;
	//[SerializeField] private float explodeFreezeTime;
	[SerializeField] private float explodeDuration;
	[SerializeField] private float hitPlayerSpeedScale; // i want to add fb's velocity * this scale to player when player is hit by fb
	public bool isExploding { get; private set; }
	private bool playerPushed;
	private bool leftPlayer;

	private const int groundLayer = 6;

	//screen shake
	[Header("Screen shake")]
	[SerializeField] private float impulseForce;

	//raycast for prevent collision being igorend when moving too fast
	//the formula is : distance = (current speed - min speed) * scale
	//[SerializeField] private float raycastMinSpeed;
	//[SerializeField] private float exceedSpeedToDistanceScale;

	[Header("Collision setting")]
	[SerializeField] private float leavePlayerTime;

	//particle
	private ParticleSystem movingParticle;
	//[SerializeField] private ParticleSystem explosionParticle;
	//private ParticleSystem explosionParticleInstance;


    // Update is called once per frame
    void Update()
	{
		if (!LogicScript.instance.isFreeze() && !isExploding)
		{
			//if (!isExploding)
			//{
				moveMain();
			//}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
    }

	private void moveMain()
	{
		if(moveSpeed > normalMoveSpeed) //apply friction
		{
			moveSpeed = Mathf.Max(moveSpeed - fireballFriction * Time.deltaTime, normalMoveSpeed);
		}

		rb.velocity = moveDir * moveSpeed;

		/*if(moveSpeed > raycastMinSpeed)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir, (moveSpeed - raycastMinSpeed) * exceedSpeedToDistanceScale);
			if (hit)
			{
				//Debug.Log(hit.collider.name);
				if(hit.collider.gameObject.tag == "KillFireballWithoutExplode")
				{
					Destroy(gameObject);
				}

				if(hit.collider.gameObject.layer == groundLayer || (hit.collider.gameObject.tag == "Fireball" && hit.collider.gameObject != this.gameObject))
				{
					if (hit.collider.gameObject.tag == "BreakablePlatform")
					{
						hit.collider.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
					}

					explode();
				}

				if (hit.collider.gameObject.tag == "Player" && leftPlayer)
				{
					explodePushPlayer();
					explode();
				}
			}
			//Debug.DrawRay(transform.position, moveDir * (moveSpeed - raycastMinSpeed) * exceedSpeedToDistanceScale);
		}*/

		if (leavePlayerTime >= 0) leavePlayerTime -= Time.deltaTime;
		if (leavePlayerTime < 0)
		{
			leftPlayer = true;
			myIgnoreCollision(false);
		}
	}

	public void summon(Vector2 localDir)
	{
		moveDir = localDir;
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<CircleCollider2D>();
		//player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();
		//logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
		isExploding = false;
		playerPushed = false;
		leftPlayer = false;

		myIgnoreCollision(true);


		moveSpeed = normalMoveSpeed;
		


		//particle
		movingParticle = transform.GetChild(0).GetComponent<ParticleSystem>(); // child 0 -> particle system
		movingParticle.transform.rotation = Quaternion.FromToRotation(Vector3.left, moveDir * -1);
	}

	private void explode()
	{
		isExploding = true;
		transform.localScale = new Vector3(explodeRadius / coll.radius, explodeRadius / coll.radius, 1);

		rb.velocity = Vector2.zero;
		//StopAllCoroutines();
		StartCoroutine(destroyCoroutine(explodeDuration));

		//particle
		//explosionParticleInstance = Instantiate(explosionParticle, transform.position, Quaternion.identity);
		GetComponent<ParticleCommonScript>().emitParticle();


		//collision
		myIgnoreCollision(false);
		coll.isTrigger = true;

	}

	private void explodePushPlayer()
	{
		Vector3 dis = PlayerControlScript.instance.transform.position - transform.position;
		Vector2 pushDir = new Vector2(dis.x, dis.y).normalized;
		//localForce = localForce.normalized * explodeForce;
		//localForce = new Vector2(localForce.x * explodeHorizontalScale, localForce.y);


		PlayerControlScript.instance.fireballExplodeStart(pushDir, rb.velocity * hitPlayerSpeedScale);
		playerPushed = true;


		//screen shake
		CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();

		impulseSource.m_DefaultVelocity = pushDir;
		impulseSource.GenerateImpulseWithForce(impulseForce);
	}

	IEnumerator destroyCoroutine(float t)
	{
		while(t > 0)
		{
			if (!LogicScript.instance.isFreeze())
			{
				t -= Time.deltaTime;
			}
			yield return null;
		}
		Destroy(gameObject);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(!LogicScript.instance.isFreeze())
		{
		//	if(collision.CompareTag("KillFireballWithoutExplode"))
		//	{
		//		Destroy(gameObject);
		//		return ;
		//	}


			/* currently, one way platform is using tilemap, 
			 * and my solution to make fb pass through is
			 * when it touch one way platform and move upwards
			 */
			//if((collision.gameObject.layer == groundLayer || collision.CompareTag("Fireball") /*|| (collision.gameObject.tag == "Player" && leftPlayer)*/) && !isExploding)
			//{
			//	if(collision.CompareTag("BreakablePlatform"))
			//	{
			//		collision.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
			//	}

			//	// if is passing one way platform -> not explode
			//	if(!(collision.CompareTag("OneWayPlatform") && moveDir.y > 0))
			//	{
			//		explode();
			//	}

			//}

			/* if player was hit by fb
			 * i want to give player an extra force, that push player toward the direction which fireball goes
			 */
			//if(collision.CompareTag("Player") && leftPlayer && !isExploding)
			//{
			//	explodePushPlayer();
			//	explode();
			//}

			if(collision.CompareTag("Player") && isExploding && !playerPushed)
			{
				/*Vector3 dis = player.transform.position - transform.position;
				Vector2 localForce = new Vector2(dis.x, dis.y).normalized;
				//localForce = localForce.normalized * explodeForce;
				//localForce = new Vector2(localForce.x * explodeHorizontalScale, localForce.y);

				player.fireballExplodeStart(localForce);
				playerPushed = true;
				CinemachineImpulseSource impulseSource = GetComponent<CinemachineImpulseSource>();
				impulseSource.m_DefaultVelocity = localForce;
				impulseSource.GenerateImpulseWithForce(impulseForce);*/
			if (rb.velocity.magnitude != 0) rb.velocity = Vector2.zero;
				explodePushPlayer();
			}
		}

	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (!LogicScript.instance.isFreeze())
		{
			if (collision.gameObject.tag == "KillFireballWithoutExplode")
			{
				Destroy(gameObject);
				return;
			}


			/* currently, one way platform is using tilemap, 
			 * and my solution to make fb pass through is
			 * when it touch one way platform and move upwards
			 */
			if ((collision.gameObject.layer == groundLayer || collision.gameObject.tag == "Fireball" /*|| (collision.gameObject.tag == "Player" && leftPlayer)*/) && !isExploding)
			{
				if (collision.gameObject.tag == "BreakablePlatform")
				{
					collision.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
				}

				// if is passing one way platform -> not explode
				if (!(collision.gameObject.tag == "OneWayPlatform" && moveDir.y > 0))
				{
					explode();
				}

			}

			/* if player was hit by fb
			 * i want to give player an extra force, that push player toward the direction which fireball goes
			 */
			if (collision.gameObject.tag == "Player" && leftPlayer && !isExploding)
			{
				explodePushPlayer();
				explode();
			}

			if (collision.gameObject.tag == "Player" && isExploding && !playerPushed)
			{
				if (rb.velocity.magnitude != 0) rb.velocity = Vector2.zero;
				explodePushPlayer();
			}
		}
	}


	public void springPush(Vector2 localDir, float localForce)
	{
		if(!isExploding)
		{
			moveDir = localDir;
			moveSpeed = moveSpeed * localForce;
		}

		leftPlayer = true;
		myIgnoreCollision(false);
	}

	private void myIgnoreCollision(bool ignore)
	{
		//ignore collision
		Collider2D[] playerColls = PlayerControlScript.instance.GetComponents<Collider2D>();
		foreach (Collider2D playerColl in playerColls)
		{
			Physics2D.IgnoreCollision(coll, playerColl, ignore);
		}
	}

}
