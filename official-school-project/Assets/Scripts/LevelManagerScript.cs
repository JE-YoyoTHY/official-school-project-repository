using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class LevelManagerScript : MonoBehaviour
{
	//reference
	private GameObject myCameraTarget;
	private PlayerControlScript player;

	private bool isCurrentLevel;

	[HideInInspector] public UnityEvent levelSetUpEvent; //currently no effect, cuz i havent implement gate yet

	// Start is called before the first frame update
	void Start()
	{
		myCameraTarget = transform.GetChild(4).gameObject; // child 4 -> camera target
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();

		transform.GetChild(3).GetComponent<CinemachineVirtualCamera>().Follow = myCameraTarget.transform; // child 3 ->virtual camera
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (isCurrentLevel)
		{
			cameraMain();
		}
	}

	public void startLevel()
	{
		levelSetUpEvent.Invoke();
		isCurrentLevel = true;
		transform.GetChild(3).GetComponent<CinemachineVirtualCamera>().Priority = 11;
	}

	public void disableLevel()
	{
		isCurrentLevel = false;
		transform.GetChild(3).GetComponent<CinemachineVirtualCamera>().Priority = 10;
	}

	public void restartLevel()
	{
		player.transform.position = transform.GetChild(0).transform.position; //child 0 -> respawn point
		levelSetUpEvent.Invoke();
	}

	private void cameraMain()
	{
		myCameraTarget.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity;

		if (myCameraTarget.transform.position.x < transform.GetChild(1).transform.position.x || player.transform.position.x < transform.GetChild(1).transform.position.x)
		{
			myCameraTarget.transform.position = new Vector3(transform.GetChild(1).transform.position.x, myCameraTarget.transform.position.y, myCameraTarget.transform.position.z);
			myCameraTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, myCameraTarget.GetComponent<Rigidbody2D>().velocity.y);
		}
		if (myCameraTarget.transform.position.y < transform.GetChild(1).transform.position.y || player.transform.position.y < transform.GetChild(1).transform.position.y)
		{
			myCameraTarget.transform.position = new Vector3(myCameraTarget.transform.position.x, transform.GetChild(1).transform.position.y, myCameraTarget.transform.position.z);
			myCameraTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(myCameraTarget.GetComponent<Rigidbody2D>().velocity.x, 0);
		}

		if (myCameraTarget.transform.position.x > transform.GetChild(2).transform.position.x || player.transform.position.x > transform.GetChild(2).transform.position.x)
		{
			myCameraTarget.transform.position = new Vector3(transform.GetChild(2).transform.position.x, myCameraTarget.transform.position.y, myCameraTarget.transform.position.z);
			myCameraTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(0, myCameraTarget.GetComponent<Rigidbody2D>().velocity.y);
		}
		if (myCameraTarget.transform.position.y > transform.GetChild(2).transform.position.y || player.transform.position.y > transform.GetChild(2).transform.position.y)
		{
			myCameraTarget.transform.position = new Vector3(myCameraTarget.transform.position.x, transform.GetChild(2).transform.position.y, myCameraTarget.transform.position.z);
			myCameraTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(myCameraTarget.GetComponent<Rigidbody2D>().velocity.x, 0);
		}
	}
}
