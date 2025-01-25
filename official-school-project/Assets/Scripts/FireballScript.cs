using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
	//variable
	private Rigidbody2D rb;
	private CircleCollider2D coll;
	private PlayerControlScript player;
	private LogicScript logic;

	private Vector2 moveDir;
	[SerializeField] private float moveSpeed;
	//[SerializeField] private float explodeForce;
	[SerializeField] private float explodeRadius;
	//[SerializeField] private float explodeFrictionLessDuration; // or i should take over player's control
	//[SerializeField] private float explodeHorizontalScale;
	//[SerializeField] private float explodeFreezeTime;
	[SerializeField] private float explodeDuration;
	private bool isExploding;
	private bool playerPushed;
	private bool leftPlayer;

	private const int groundLayer = 6;


    // Update is called once per frame
    void Update()
	{
		if (!logic.isFreeze())
		{
			if (!isExploding)
			{
				moveMain();
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
    }

	private void moveMain()
	{
		rb.velocity = moveDir * moveSpeed;
	}

	public void summon(Vector2 localDir)
	{
		moveDir = localDir;
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<CircleCollider2D>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();
		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
		isExploding = false;
		playerPushed = false;
		leftPlayer = false;
	}

	private void explode() // summon a object, if player touch this, they gain velocity
	{
		isExploding = true;
		transform.localScale = new Vector3(explodeRadius / coll.radius, explodeRadius / coll.radius, 1);

		
		StartCoroutine(destroyCoroutine(explodeDuration));
	}

	IEnumerator destroyCoroutine(float t)
	{
		while(t > 0)
		{
			if (!logic.isFreeze())
			{
				t -= Time.deltaTime;
			}
			yield return null;
		}
		Destroy(gameObject);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(!logic.isFreeze())
		{
			if((collision.gameObject.layer == groundLayer || collision.gameObject.tag == "Fireball" || (collision.gameObject.tag == "Player" && leftPlayer)) && !isExploding)
			{
				explode();
			}

			if(collision.gameObject.tag == "Player" && isExploding && !playerPushed)
			{
				Vector3 dis = player.transform.position - transform.position;
				Vector2 localForce = new Vector2(dis.x, dis.y);
				//localForce = localForce.normalized * explodeForce;
				//localForce = new Vector2(localForce.x * explodeHorizontalScale, localForce.y);

				player.fireballExplodeStart(localForce.normalized);
				playerPushed = true;
			}
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			leftPlayer = true;
		}
	}

	public void springPush(Vector2 localDir)
	{
		if(!isExploding)
		{
			moveDir = localDir;
		}
	}
}
