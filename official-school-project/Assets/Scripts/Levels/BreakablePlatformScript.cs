using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformScript : MonoBehaviour
{
	[SerializeField] private float breakTime; // how long it will break after player stands on it
	[SerializeField] private float restoreTime; // how long it will restore after breaks
	[SerializeField] private float playerJumpBreakTime; // if player jump on this tile, decrease the lifespan

	private float breakTimeCounter;
	private float restoreTimeCounter;
	private bool isBreaking;
	private bool isRestoreing;
	private bool playerJumpThisFrame;

	//make breakable platform use tile to set
	public BreakablePlatformScript tileOnLeft;
	public BreakablePlatformScript tileOnRight;
	//private LayerMask groundLayer;
	//private bool initialized = false;

	//private const int groundLayer = 6;

    // Start is called before the first frame update
    void Start()
	{
		//StartCoroutine(startInitialization());
		restoreAfterBreak();
    }

    // Update is called once per frame
    void Update()
    {
		//if(!initialized) startInitialization();

		breakMain();
    }

	private void LateUpdate()
	{
		playerJumpThisFrame = false;
	}

	//private void startInitialization()
	//{
	//	initialized = true;

	//	//yield return new WaitForSeconds(1);
	//	gameObject.tag = "Untagged";
	//	RaycastHit2D raycastForLeftTile = Physics2D.BoxCast(gameObject.GetComponent<Collider2D>().bounds.center + new Vector3(-2f, 0.5f, 0), Vector2.one * 0.5f, 0f, Vector2.left, 0f, groundLayer);
	//	if(raycastForLeftTile) tileOnLeft = (raycastForLeftTile.collider.CompareTag("BreakablePlatform")) ? raycastForLeftTile.transform.GetComponent<BreakablePlatformScript>() : null;
	//	RaycastHit2D raycastForRightTile = Physics2D.BoxCast(gameObject.GetComponent<Collider2D>().bounds.center, Vector2.one, 0f, Vector2.left, 0f, groundLayer);
	//	//tileOnLeft = (raycastForLeftTile.collider.CompareTag("BreakablePlatform")) ? raycastForLeftTile.transform.GetComponent<BreakablePlatformScript>() : null;
	//	if(raycastForRightTile)tileOnRight = (raycastForRightTile.collider.CompareTag("BreakablePlatform")) ? raycastForRightTile.transform.GetComponent<BreakablePlatformScript>() : null;
	//	gameObject.tag = "BreakablePlatform";
	//}


	/*private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Player" && !isBreaking && !isRestoreing)
		{
			breakPrepare();
		}
	}*/

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("BreakablePlatform"))
		{
			print("s;fja;sldfja");

			tileOnRight = collision.GetComponent<BreakablePlatformScript>();
			tileOnRight.tileOnLeft = this;
		}
	}

	public void touchPlayer() // invoked by player ground trigger script to see if player touch it
	{
		if (!isBreaking && !isRestoreing)
		{
			//traversalThroughAdjacentTiles(this);
			breakPrepare();
		}
	}

	public void traversalThroughAdjacentTiles(BreakablePlatformScript lastTile)
	{
		if (!isBreaking && !isRestoreing)
		{
			if (tileOnLeft != lastTile && tileOnLeft != null) tileOnLeft.traversalThroughAdjacentTiles(this);
			if (tileOnRight != lastTile && tileOnRight != null) tileOnRight.traversalThroughAdjacentTiles(this);

			breakPrepare();
			
		}



		//touchPlayer();
	}

	private void breakMain()
	{
		if(isBreaking)
		{
			if(breakTimeCounter > 0)
			{
				breakTimeCounter -= Time.deltaTime;

				//change indicator
				transform.GetChild(0).localScale = new Vector3(1, breakTimeCounter / breakTime, 1);
			}
			else
			{
				if(!isRestoreing)
				{
					breakStart(false, null);
				}
			}
		}

		if (isRestoreing)
		{
			if(restoreTimeCounter > 0)
			{
				restoreTimeCounter -= Time.deltaTime;
			}
			else
			{
				//restoreTimeCounter = 0;
				restoreAfterBreak();
			}
		}
	}

	private void breakPrepare()
	{
		isBreaking = true;
		breakTimeCounter = breakTime;
	}

	public void breakStart(bool byFireball, BreakablePlatformScript lastTile)
	{
		breakTimeCounter = 0;
		isBreaking = true;
		isRestoreing = true;
		//isBreaking = false;
		restoreTimeCounter = restoreTime;

		transform.GetChild(0).localScale = new Vector3(1, 0, 1);

		GetComponent<Collider2D>().enabled = false;

		if (byFireball)
		{
			if (tileOnLeft != lastTile && tileOnLeft != null) tileOnLeft.breakStart(byFireball, this);
			if (tileOnRight != lastTile && tileOnRight != null) tileOnRight.breakStart(byFireball, this);
		}
	}

	public void restoreAfterBreak()
	{
		restoreTimeCounter = 0;

		isBreaking = false;
		isRestoreing = false;

		GetComponent<Collider2D>().enabled = true;
		transform.GetChild(0).localScale = new Vector3(1, 1, 1);
	}

	public void playerJumpOnThisTraversal(BreakablePlatformScript lastTile)
	{
		if (!playerJumpThisFrame)
		{
			breakTimeCounter -= playerJumpBreakTime;
			playerJumpThisFrame = true;

			if (tileOnLeft != lastTile && tileOnLeft != null) tileOnLeft.playerJumpOnThisTraversal(this);
			if (tileOnRight != lastTile && tileOnRight != null) tileOnRight.playerJumpOnThisTraversal(this);
		}
	}
}
