using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
	private GameObject gateSprite;
	private GameObject lockedPos;
	private GameObject openedPos;

	private int currentKeyCount; //while max key count = child 3(keys) . childcount
	private bool gateOpened;

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
		if (currentKeyCount == transform.GetChild(3).childCount && !gateOpened) // child 3 -> keys
		{
			gateOpen();
		}
	}

	private void gateOpen()
	{
		gateOpened = true;
		gateSprite.transform.position = openedPos.transform.position;
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
}
