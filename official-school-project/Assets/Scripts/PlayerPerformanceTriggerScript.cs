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
	}

	public void disableTrigger()
	{
		performanceTriggerInspectorObject.endEvent.Invoke();
	}

	public void controlMain()
	{
		if (performanceTriggerInspectorObject.action == PerformanceAction.move)
		{
			moveMain();
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


}

[System.Serializable]
public class PerformanceTriggerInspectorObject
{
	public bool isStartPoint;
	public bool isEndPoint;

	[HideInInspector] public PerformanceAction action;
	[HideInInspector] public PlayerPerformanceTriggerScript nextTrigger;

	//move
	[HideInInspector] public float moveSpeed;

	[Header("Invoke Events")]
	public UnityEvent startEvent;
	public UnityEvent endEvent;
}

public enum PerformanceAction
{
	none,
	move
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
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(playerPerformanceTrigger);
		}
	}
}


#endif
