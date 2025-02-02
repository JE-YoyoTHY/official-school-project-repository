using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;
using Unity.VisualScripting;

public class CameraControlTriggerScript : MonoBehaviour
{
	private LevelManagerScript levelManager;
	private Collider2D coll;

	[SerializeField] private CameraSwitchDirection cameraSwitchDir;

	[Header("Vertical")]
	[SerializeField] private CinemachineVirtualCamera cameraAbove;
	[SerializeField] private CinemachineVirtualCamera cameraBelow;
	[Header("Horizontal")]
	[SerializeField] private CinemachineVirtualCamera cameraOnLeft;
	[SerializeField] private CinemachineVirtualCamera cameraOnRight;

	// Start is called before the first frame update
	void Start()
    {
        coll = GetComponent<Collider2D>();
		levelManager = transform.parent.parent.parent.GetComponent<LevelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

			switch(cameraSwitchDir)
			{
				case CameraSwitchDirection.Vertical:
					if(exitDirection.y < 0) //below
					{
						levelManager.swapCamera(cameraBelow);
					}
					else if(exitDirection.y > 0)
					{
						levelManager.swapCamera(cameraAbove);
					}

					break;
				case CameraSwitchDirection.Horizontal:
					if (exitDirection.x < 0) //left
					{
						levelManager.swapCamera(cameraOnLeft);
					}
					else if (exitDirection.x > 0)
					{
						levelManager.swapCamera(cameraOnRight);
					}
					break;
			}
		}
	}
}

public enum CameraSwitchDirection // vertical means 2 cameras are above and below
{
	Vertical, Horizontal
}

