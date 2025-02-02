using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;
using Unity.VisualScripting;

public class CameraControlTriggerScript : MonoBehaviour
{
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
					if(exitDirection.y < 0)
					{

					}

					break;
				case CameraSwitchDirection.Horizontal:

					break;
			}
		}
	}
}

public enum CameraSwitchDirection // vertical means 2 cameras are above and below
{
	Vertical, Horizontal
}

