using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformScript : MonoBehaviour
{
	[SerializeField] private float breakTime; // how long it will break after player stands on it
	[SerializeField] private float restoreTime; // how long it will restore after breaks
	private float breakTimeCounter;
	private float restoreTimeCounter;
	private bool isBreaking;
	private bool isRestoreing;

	//private const int groundLayer = 6;

    // Start is called before the first frame update
    void Start()
    {
		restoreAfterBreak();
    }

    // Update is called once per frame
    void Update()
    {
        breakMain();
    }

	/*private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Player" && !isBreaking && !isRestoreing)
		{
			breakPrepare();
		}
	}*/

	public void touchPlayer() // invoked by player ground trigger script to see if player touch it
	{
		if (!isBreaking && !isRestoreing)
		{
			breakPrepare();
		}
	}

	/*private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Fireball")
		{
			FireballScript fb = collision.gameObject.GetComponent<FireballScript>();
			if (!fb.isExploding)
			{
				breakPrepare();
				breakStart();
			}
		}
	}*/

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
					breakStart();
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

	public void breakStart()
	{
		breakTimeCounter = 0;
		isBreaking = true;
		isRestoreing = true;
		//isBreaking = false;
		restoreTimeCounter = restoreTime;

		transform.GetChild(0).localScale = new Vector3(1, 0, 1);

		GetComponent<Collider2D>().enabled = false;
	}

	public void restoreAfterBreak()
	{
		restoreTimeCounter = 0;

		isBreaking = false;
		isRestoreing = false;

		GetComponent<Collider2D>().enabled = true;
		transform.GetChild(0).localScale = new Vector3(1, 1, 1);
	}
}
