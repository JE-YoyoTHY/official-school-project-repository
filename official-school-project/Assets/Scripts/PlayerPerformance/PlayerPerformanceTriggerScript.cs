using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

[ExecuteInEditMode]
public class PlayerPerformanceTriggerScript : MonoBehaviour
{
	//variable
	public PerformanceTriggerInspectorObject performanceTriggerInspectorObject;




    // Update is called once per frame
    void Update()
    {
        if(performanceTriggerInspectorObject.action == PerformanceAction.move)
		{
			DrawMovePath();
		}
		
    }

	#region editor mode method

	private void DrawMovePath()
	{
		if(performanceTriggerInspectorObject.nextTrigger != null)
		{
			//child 0 -> pos
			Debug.DrawLine(transform.GetChild(0).position, performanceTriggerInspectorObject.nextTrigger.transform.GetChild(0).position, Color.green);
		}
	}

	#endregion

	#region basic method

	public void enableTrigger()
	{
		performanceTriggerInspectorObject.startEvent.Invoke();

		if (performanceTriggerInspectorObject.isEndPoint)
		{
			PlayerPerformanceSystemScript.instance.controlEnd();
		}
	}

	public void disableTrigger()
	{
		performanceTriggerInspectorObject.endEvent.Invoke();
	}

	public void enableNextTrigger()
	{
		disableTrigger();
		PlayerPerformanceSystemScript.instance.setController(performanceTriggerInspectorObject.nextTrigger);
	}

	public void controlMain()
	{
		if (performanceTriggerInspectorObject.action == PerformanceAction.move && !LogicScript.instance.isFreeze())
		{
			moveMain();
		}
		if (performanceTriggerInspectorObject.action == PerformanceAction.waitForInput)
		{
			waitForInputMain();
		}
	}

	#endregion

	#region move

	private void moveMain()
	{
		Rigidbody2D playerRB = PlayerControlScript.instance.GetComponent<Rigidbody2D>();
		Vector3 deltaPos = performanceTriggerInspectorObject.nextTrigger.transform.GetChild(0).position - transform.GetChild(0).position;
		int moveDir = 0;
		if (deltaPos.x > 0) moveDir = 1; else if (deltaPos.x < 0) moveDir = -1;
		playerRB.velocity = new Vector2(performanceTriggerInspectorObject.moveSpeed * moveDir, playerRB.velocity.y);
	}

	#endregion

	#region wait for input

	private void waitForInputMain()
	{
		if (performanceTriggerInspectorObject.waitInputAction == PerformanceWaitInputAction.fireball)
		{
			// freeze the game until player press the correct input
			LogicScript.instance.setFreezeTime(1f);

			// end freeze state
			if (InputManagerScript.instance.fireballCastByKeyboardInput == InputState.press
				&& InputManagerScript.instance.fireballDirInput.normalized == performanceTriggerInspectorObject.waitFireballDir.normalized)
			{
				endFireballInputWait(true);
			}
			if (InputManagerScript.instance.fireballCastByMouseInput == InputState.press
				&& PlayerControlScript.instance.fireballMouseDirValue.normalized == performanceTriggerInspectorObject.waitFireballDir.normalized)
			{
				endFireballInputWait(false);
			}

		}
	}

	private void endWaitForInput()
	{
		LogicScript.instance.setFreezeTime(0.01f);
		enableNextTrigger();

	}

	private void endFireballInputWait(bool isCastByKeyboard)
	{
		endWaitForInput();
		PlayerControlScript.instance.fireballStart(isCastByKeyboard);

		InputManagerScript.instance.fireballCastByKeyboardInput = InputState.normal;
		InputManagerScript.instance.fireballCastByMouseInput = InputState.normal;

	}

	#endregion

}

[System.Serializable]
public class PerformanceTriggerInspectorObject
{
	public bool isStartPoint;
	public bool isEndPoint;

	//public bool needGrounded;

	[HideInInspector] public PerformanceAction action;
	[HideInInspector] public PlayerPerformanceTriggerScript nextTrigger;

	[Header("Invoke Events")]
	public UnityEvent startEvent;
	public UnityEvent endEvent;


	//move
	[HideInInspector] public float moveSpeed;

	//wait for inputs
	[HideInInspector] public PerformanceWaitInputAction waitInputAction;
	[HideInInspector] public Vector2 waitFireballDir;
}

public enum PerformanceAction
{
	noControl, //no control for trigger and player, so player will maintain the speed they should have
	move,
	waitForInput,
}

public enum PerformanceWaitInputAction
{
	jump,
	fireball
}

#if UNITY_EDITOR

[CustomEditor(typeof(PlayerPerformanceTriggerScript))]
public class PerformanceScriptEditor : Editor
{
	PlayerPerformanceTriggerScript playerPerformanceTrigger;

	private void OnEnable()
	{
		playerPerformanceTrigger = (PlayerPerformanceTriggerScript)target;	
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (!playerPerformanceTrigger.performanceTriggerInspectorObject.isEndPoint)
		{
			playerPerformanceTrigger.performanceTriggerInspectorObject.nextTrigger =
				EditorGUILayout.ObjectField("Next Object", 
				playerPerformanceTrigger.performanceTriggerInspectorObject.nextTrigger, 
				typeof(PlayerPerformanceTriggerScript), true) as PlayerPerformanceTriggerScript;

			playerPerformanceTrigger.performanceTriggerInspectorObject.action =
				(PerformanceAction)EditorGUILayout.EnumPopup("Player Action", playerPerformanceTrigger.performanceTriggerInspectorObject.action);

			if(playerPerformanceTrigger.performanceTriggerInspectorObject.action == PerformanceAction.move)
			{
				playerPerformanceTrigger.performanceTriggerInspectorObject.moveSpeed =
					EditorGUILayout.FloatField("Move Speed", playerPerformanceTrigger.performanceTriggerInspectorObject.moveSpeed);
			}

			if (playerPerformanceTrigger.performanceTriggerInspectorObject.action == PerformanceAction.waitForInput)
			{
				playerPerformanceTrigger.performanceTriggerInspectorObject.waitInputAction =
				(PerformanceWaitInputAction)EditorGUILayout.EnumPopup("Wait for input", playerPerformanceTrigger.performanceTriggerInspectorObject.waitInputAction);

				if (playerPerformanceTrigger.performanceTriggerInspectorObject.waitInputAction == PerformanceWaitInputAction.fireball)
					playerPerformanceTrigger.performanceTriggerInspectorObject.waitFireballDir =
						EditorGUILayout.Vector2Field("Wait fireball direction (Not normalized)", playerPerformanceTrigger.performanceTriggerInspectorObject.waitFireballDir);
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(playerPerformanceTrigger);
		}
	}
}


#endif
