using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GateScript : MonoBehaviour
{
	private GameObject gateSprite;
	private GameObject lockedPos;
	private GameObject openedPos;

	private int currentKeyCount; //while max key count = child 3(keys) . childcount
	private bool gateOpened;

	[SerializeField] private float howLongToOpen;
	private float openTimeCounter;

    // Start is called before the first frame update
    void Start()
    {
        gateSprite = transform.GetChild(0).gameObject; // 0-> sprite
		lockedPos = transform.GetChild(1).gameObject;
		openedPos = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		if (!LogicScript.instance.isFreeze())
		{
			if (currentKeyCount == transform.GetChild(3).childCount && !gateOpened) // child 3 -> keys
			{
				gateOpen();
			}
			if (gateOpened && openTimeCounter < howLongToOpen)
			{
				gateSprite.transform.position = Vector3.Lerp(lockedPos.transform.position, openedPos.transform.position, openTimeCounter / howLongToOpen);
				openTimeCounter = Mathf.Min(openTimeCounter + Time.deltaTime, howLongToOpen);
				
			}
		}
		
	}

	private void gateOpen()
	{
		gateOpened = true;
		//gateSprite.transform.position = openedPos.transform.position;
		openTimeCounter = 0;
	}

	public void gateReset()
	{
		gateOpened = false;
		currentKeyCount = 0;
		gateSprite.transform.position = lockedPos.transform.position;

		//reset all key in keys
		for(int i = 0; i < transform.GetChild(3).childCount; i++) // 3->keys
		{
			transform.GetChild(3).GetChild(i).GetComponent<KeyScript>().keyReset();
		}
	}

	public void keyObtain()
	{
		currentKeyCount++;
		/*if(currentKeyCount == transform.GetChild(3).childCount) // child 3 -> keys
		{
			gateOpen();
		}*/
	}


	//funcitons for custom editor
	public void setLockedPos()
	{
		transform.GetChild(1).position = // 1 -> locked pos
		transform.GetChild(0).position; // 0-> sprite
	}

	public void setOpenedPos()
	{
		transform.GetChild(2).position = // 2 -> locked pos
		transform.GetChild(0).position; // 0-> sprite
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(GateScript))]
public class GateCustomInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GateScript Gate = (GateScript)target;

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Set Locked Position", GUILayout.Width(180f)))
		{
			Gate.setLockedPos();
		}

		if (GUILayout.Button("Set Opened Position", GUILayout.Width(180f)))
		{
			Gate.setOpenedPos();
		}
		EditorGUILayout.EndHorizontal();
	}


}

#endif
