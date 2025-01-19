using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
	//variable
	private Rigidbody2D rb;
	private CircleCollider2D coll;
	private PlayerControlScript player;

	private Vector2 moveDir;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float explodeForce;
	[SerializeField] private float explodeRadius;
	[SerializeField] private float explodeFrictionLessDuration; // or i should take over player's control
	[SerializeField] private float explodeHorizontalScale;
	[SerializeField] private float explodeFreezeTime;
	[SerializeField] private float explodeDuration;
	private bool isExploding;
	private bool playerPushed;

	private const int groundLayer = 6;


    // Update is called once per frame
    void Update()
    {
		if (!isExploding)
		{
			moveMain();
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
		isExploding = false;
		playerPushed = false;
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
			t -= Time.deltaTime;
			yield return null;
		}
		Destroy(gameObject);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.layer == groundLayer && !isExploding)
		{
			explode();
		}

		if(collision.gameObject.tag == "Player" && isExploding && !playerPushed)
		{
			Vector3 dis = player.transform.position - transform.position;
			Vector2 localForce = new Vector2(dis.x, dis.y);
			localForce = localForce.normalized * explodeForce;
			localForce = new Vector2(localForce.x * explodeHorizontalScale, localForce.y);

			player.fireballExplode(localForce, explodeFrictionLessDuration);
			playerPushed = true;
		}
	}
}
