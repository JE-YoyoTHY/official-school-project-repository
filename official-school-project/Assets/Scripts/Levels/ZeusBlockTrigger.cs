using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusBlockTrigger : MonoBehaviour
{
	[SerializeField] private ZeusBlockScript[] objects;
	private bool isTriggered = false;


	private void enterTrigger()
	{
		isTriggered = true;

		foreach (var obj in objects)
		{
			if (obj.blockType == ZeusBlockType.fallingBlock)
			{
				obj.explodeStart();
			}
			else if (obj.blockType == ZeusBlockType.rotatingBlock)
			{
				obj.startRotate();
			}
		}
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == PlayerControlScript.instance.gameObject)
		{
			if(!isTriggered)
				enterTrigger();
		}
	}


}
