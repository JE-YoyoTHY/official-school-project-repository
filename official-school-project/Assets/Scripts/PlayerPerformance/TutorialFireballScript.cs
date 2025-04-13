using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFireballScript : MonoBehaviour
{
	//variable
	private Rigidbody2D rb;
	private CircleCollider2D coll;
	private TutorialShadeScript tutorialShade;

	private Vector2 moveDir;
	private float moveSpeed;
	[SerializeField] private float normalMoveSpeed;
	[SerializeField] private float fireballFriction; // reduce its speed to normal when speed is greater than normal, for example : spring
													 //[SerializeField] private float explodeForce;
	[SerializeField] private float explodeRadius;
	[SerializeField] private float explodeDuration;
	
	public bool isExploding { get; private set; }
	private bool playerPushed;
	private bool leftPlayer;

	/* i want to add this vector2 to player if they are hit, 
	 * in place of hit player speed scale, 
	 * because physic collision make it hard to control the speed
	 * and this value is set by spring when pushed
	 */
	private Vector2 hitPlayerSpeedModifier;

	private const int groundLayer = 6;

	[Header("Collision setting")]
	[SerializeField] private float leavePlayerTime;


	// Update is called once per frame
	void Update()
	{
		if (!LogicScript.instance.isFreeze() && !isExploding)
		{
			moveMain();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void moveMain()
	{
		if (moveSpeed > normalMoveSpeed) //apply friction
		{
			moveSpeed = Mathf.Max(moveSpeed - fireballFriction * Time.deltaTime, normalMoveSpeed);
		}

		rb.velocity = moveDir * moveSpeed;

		

		if (leavePlayerTime >= 0) leavePlayerTime -= Time.deltaTime;
		if (leavePlayerTime < 0)
		{
			leftPlayer = true;
			myIgnoreCollision(false);
		}
	}

	public void summon(Vector2 localDir, TutorialShadeScript shade)
	{
		//init
		moveDir = localDir;
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<CircleCollider2D>();
		tutorialShade = shade;

		isExploding = false;
		playerPushed = false;
		leftPlayer = false;

		hitPlayerSpeedModifier = Vector2.zero;

		myIgnoreCollision(true);


		moveSpeed = normalMoveSpeed;
	}

	private void explode()
	{
		isExploding = true;
		transform.localScale = new Vector3(explodeRadius / coll.radius, explodeRadius / coll.radius, 1);

		rb.velocity = Vector2.zero;
		hitPlayerSpeedModifier = Vector2.zero;
		//StopAllCoroutines();
		StartCoroutine(destroyCoroutine(explodeDuration));


		//collision
		myIgnoreCollision(false);
		coll.isTrigger = true;

	}

	private void explodePushPlayer(GameObject shade)
	{
		Vector3 dis = shade.transform.position - transform.position;
		Vector2 pushDir = new Vector2(dis.x, dis.y).normalized;

		shade.GetComponent<TutorialShadeScript>().fireballExplodeStart(pushDir, hitPlayerSpeedModifier);
		playerPushed = true;

	}

	IEnumerator destroyCoroutine(float t)
	{
		while (t > 0)
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
		if (!LogicScript.instance.isFreeze())
		{
			if (collision.CompareTag("TutorialShade") && isExploding && !playerPushed)
			{
				if (rb.velocity.magnitude != 0) rb.velocity = Vector2.zero;
				explodePushPlayer(collision.gameObject);
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
				// if is passing one way platform -> not explode
				if (!(collision.gameObject.tag == "OneWayPlatform" && moveDir.y > 0))
				{
					explode();
				}

			}

			/* if player was hit by fb
			 * i want to give player an extra force, that push player toward the direction which fireball goes
			 */
			if (collision.gameObject.tag == "TutorialShade" && leftPlayer && !isExploding)
			{
				explodePushPlayer(collision.gameObject);
				explode();
			}

			if (collision.gameObject.tag == "TutorialShade" && isExploding && !playerPushed)
			{
				if (rb.velocity.magnitude != 0) rb.velocity = Vector2.zero;
				explodePushPlayer(collision.gameObject);
			}
		}
	}


	public void springPush(Vector2 localDir, float localForce, Vector2 localHitPlayerSpeedModifier)
	{
		if (!isExploding)
		{
			moveDir = localDir;
			moveSpeed = moveSpeed * localForce;
			hitPlayerSpeedModifier = localHitPlayerSpeedModifier;
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
			Physics2D.IgnoreCollision(coll, playerColl, true);
		}

		GameObject[] fbs = GameObject.FindGameObjectsWithTag("Fireball");
		foreach (GameObject fb in fbs)
		{
			Physics2D.IgnoreCollision(coll, fb.GetComponent<Collider2D>(), true);
		}

		Collider2D[] shadeColls = tutorialShade.GetComponents<Collider2D>();
		foreach (Collider2D shadeColl in shadeColls)
		{
			Physics2D.IgnoreCollision(coll, shadeColl, ignore);
		}
	}
}
