using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShadeControlTriggerScript : MonoBehaviour
{
	private TutorialShadeScript currentShade;
	private Collider2D coll;

	[SerializeField] private TutorialShadeAction action;

	[Header("move")]
	[SerializeField] private sbyte moveDir;


	[Header("Fireball")]
	[SerializeField] private Vector2 fireballDir;

	[Header("End and Start")]
	[SerializeField] private float endWaitTime;
	[SerializeField] private float endDisappearTime;
	[SerializeField] private float startWaitTime;
	[SerializeField] private GameObject startPoint;
	private Coroutine waitCoroutine;
	//private bool isWaiting;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();

		myIgnoreCollision(true);
    }

    // Update is called once per frame
    void Update()
    {
  //      if(currentShade != null)
		//{
		//	stayTrigger();
		//}
    }

	#region basic

	private void enterTrigger(TutorialShadeScript shade)
	{
		currentShade = shade;

		//if (action == TutorialShadeAction.move)
		//{
		//	currentShade.setMoveDir(moveDir);
		//}

		if (action == TutorialShadeAction.jump)
		{
			currentShade.jumpStart();
		}

		if (action == TutorialShadeAction.castFireball)
		{
			currentShade.fireballStart(fireballDir.normalized);
		}

		if (action == TutorialShadeAction.end)
		{
			if(waitCoroutine != null) StopCoroutine(waitCoroutine);
			waitCoroutine = StartCoroutine(endTutorial());
		}
	}

	private void stayTrigger()
	{
		if (action == TutorialShadeAction.move)
		{
			if(!currentShade.isWaiting)
				currentShade.setMoveDir(moveDir);
			else currentShade.setMoveDir(0);
		}
	}

	private void exitTrigger()
	{

		if (action == TutorialShadeAction.move)
		{
			currentShade.setMoveDir(0);
		}

		//currentShade = null;
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


	#endregion

	#region start and end tutorial


	private IEnumerator endTutorial()
	{
		currentShade.isWaiting = true;

		//end wait
		float t = endWaitTime;
        while (t >= 0)
        {
            if(!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;

			yield return null;
        }

		//disappear
		currentShade.GetComponent<SpriteRenderer>().enabled = false;
		t = endDisappearTime;

		while (t >= 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;

			yield return null;
		}

		//start wait
		t = startWaitTime;
		currentShade.GetComponent<SpriteRenderer>().enabled = true;
		//teleport
		currentShade.transform.position = startPoint.transform.position;

		while (t >= 0)
		{
			if (!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;

			yield return null;
		}

		currentShade.isWaiting = false;

	}

	#endregion

	#region on trigger
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("TutorialShade"))
			enterTrigger(collision.GetComponent<TutorialShadeScript>());
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		stayTrigger();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("TutorialShade"))
			exitTrigger();
	}
	#endregion
}

public enum TutorialShadeAction
{
	move,
	jump,
	castFireball,
	end
}
