using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;
using Cinemachine;
using JetBrains.Annotations;

//[ExecuteInEditMode]
public class PlayerPerformanceTriggerScript : MonoBehaviour
{
	//variable
	public PerformanceTriggerInspectorObject performanceTriggerInspectorObject;

	private Rigidbody2D playerRB;
	private CinemachineImpulseSource impulseSource;

	public sbyte moveDir { get; private set; }

	private void Start()
	{
		//playerRB = PlayerControlScript.instance.GetComponent<Rigidbody2D>();
		impulseSource = GetComponent<CinemachineImpulseSource>();
	}

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void OnDrawGizmos()
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
		playerRB = PlayerControlScript.instance.GetComponent<Rigidbody2D>();
		impulseSource = GetComponent<CinemachineImpulseSource>();

		performanceTriggerInspectorObject.startEvent.Invoke();

		if (performanceTriggerInspectorObject.isEndPoint)
		{
			PlayerPerformanceSystemScript.instance.controlEnd();
		}

		//gravity and friction
		PlayerControlScript.instance.performanceGravity(performanceTriggerInspectorObject.performWithNoGravity);
		PlayerControlScript.instance.performanceFriction(performanceTriggerInspectorObject.performWithNoFriction);

		//trigger action if needed
		if (performanceTriggerInspectorObject.action == PerformanceAction.waitForSecond)
		{
			StartCoroutine(waitForSecondMain());
		}

		if (performanceTriggerInspectorObject.action == PerformanceAction.move)
		{
			PlayerControlScript.instance.performanceFriction(true);
		}

		
	}

	public void disableTrigger()
	{
		performanceTriggerInspectorObject.endEvent.Invoke();

		//gravity and friction
		PlayerControlScript.instance.performanceGravity(performanceTriggerInspectorObject.performWithNoGravity);
		PlayerControlScript.instance.performanceFriction(performanceTriggerInspectorObject.performWithNoFriction);
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

    #region Instruction UI
    public void showInstructionUIFromPerformanceTrigger(int instructionTypeInt)
    {
		InstructionUI.InstructionTypeEnum instructionType = (InstructionUI.InstructionTypeEnum)instructionTypeInt;
        InstructionUIManager.instance.showInstructionUI(instructionType);
    }

    public void disappearInstructionUIFromPerformanceTrigger()
    {
        InstructionUIManager.instance.disappearInstructionUI();
    }
    #endregion

    #endregion

    #region move

    private void moveMain()
	{
		Rigidbody2D playerRB = PlayerControlScript.instance.GetComponent<Rigidbody2D>();
		//Vector3 deltaPos = performanceTriggerInspectorObject.nextTrigger.transform.GetChild(0).position - transform.GetChild(0).position;
		Vector3 deltaPos = performanceTriggerInspectorObject.nextTrigger.transform.GetChild(0).position - PlayerControlScript.instance.transform.position;
		moveDir = 0;
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


	#region wait for sec

	//private void waitForSecondMain()
	//{
	//	if (!LogicScript.instance.isFreeze())
	//	{
	//		enableNextTrigger();
	//	}
	//}

	//private void waitForSecondStart()
	//{
	//	LogicScript.instance.setFreezeTime(performanceTriggerInspectorObject.waitTime);
	//}

	private IEnumerator waitForSecondMain()
	{
		float t = performanceTriggerInspectorObject.waitTime;
		//PlayerControlScript.instance.performanceGravity(performanceTriggerInspectorObject.waitWithNoGravity);

		while (t > 0)
		{
			t -= Time.deltaTime;

			//playerRB.velocity = Vector2.zero;

			if (performanceTriggerInspectorObject.waitWithScreenShake)
				CameraShakeManagerScript.instance.cameraShakeWithProfileWithRandomDirection(performanceTriggerInspectorObject.screenShakeProfile, impulseSource);

			yield return null;
		}

		//PlayerControlScript.instance.performanceGravity(false);
		enableNextTrigger();
	}

	#endregion

	#region secondary function for unity event

	public void jump()
	{
		playerRB.velocity = new Vector2(playerRB.velocity.x, performanceTriggerInspectorObject.jumpStrength);
	}

	public void screenShake()
	{
        impulseSource = GetComponent<CinemachineImpulseSource>();
        CameraShakeManagerScript.instance.cameraShakeWithProfileWithRandomDirection(performanceTriggerInspectorObject.screenShakeProfile, impulseSource);
	}

	public void performanceTileDestroy(string name)
	{
		LogicScript.instance.performanceTileDestroy(name);
	}

	public void enablePlayerFireball(bool isSettingActive)
	{
		PlayerControlScript.instance.fireballPlayerGetAbility(isSettingActive);
	}

	public void comicStart(string id)
	{
		LogicScript.instance.comicStart(id);
	}

	public void swapCamera()
	{
		LevelManagerScript tmpLevel = transform.parent.parent.parent.parent.GetComponent<LevelManagerScript>();
		tmpLevel.swapCamera(performanceTriggerInspectorObject.virtualCamera, performanceTriggerInspectorObject.camBlendTime);
	}

	public void gridColor()
	{
		LogicScript.instance.gridColor();
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

	[Header("Secondary Functions")]
	public float jumpStrength;
	public ScreenShakeProfile screenShakeProfile;
	public bool performWithNoGravity;
	public bool performWithNoFriction;
	public CinemachineVirtualCamera virtualCamera;
	public float camBlendTime;


	//move
	[HideInInspector] public float moveSpeed;

	//wait for inputs
	[HideInInspector] public PerformanceWaitInputAction waitInputAction;
	[HideInInspector] public Vector2 waitFireballDir;

	//wait for second
	[HideInInspector] public float waitTime;
	//[HideInInspector] public bool waitWithNoGravity;
	[HideInInspector] public bool waitWithScreenShake;



}

public enum PerformanceAction
{
	noControl, //no control for trigger and player, so player will maintain the speed they should have
	move,
	waitForInput,
	waitForSecond
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

			if (playerPerformanceTrigger.performanceTriggerInspectorObject.action == PerformanceAction.waitForSecond)
			{
				playerPerformanceTrigger.performanceTriggerInspectorObject.waitTime =
					EditorGUILayout.FloatField("Wait Time", playerPerformanceTrigger.performanceTriggerInspectorObject.waitTime);
				//playerPerformanceTrigger.performanceTriggerInspectorObject.waitWithNoGravity =
					//EditorGUILayout.Toggle("Wait With No Gravity", playerPerformanceTrigger.performanceTriggerInspectorObject.waitWithNoGravity);
				playerPerformanceTrigger.performanceTriggerInspectorObject.waitWithScreenShake =
					EditorGUILayout.Toggle("Wait With Screen Shake", playerPerformanceTrigger.performanceTriggerInspectorObject.waitWithScreenShake);
			}
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(playerPerformanceTrigger);
		}
	}
}


#endif
