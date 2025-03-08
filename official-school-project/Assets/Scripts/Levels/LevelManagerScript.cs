using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class LevelManagerScript : MonoBehaviour
{
	//reference
	//private LevelManagerScript previousLevel;
	public LevelManagerScript nextLevel;

	/*legacy below
	//private GameObject myCameraTarget;
	//private Transform camLB; // cam lb for camera left bottom boundary
	//private Transform camRT; // cam rt for camera right top 
	
	//private CinemachineVirtualCamera myVirtualCam;*/
	private CinemachineVirtualCamera currentCam;
	private PlayerControlScript player;

	/*//camera
	private Vector3 playerLastFramePos;
	private const int playerLastFramePosUpdateRate = 4;
	private int playerLastFramePosCounter;*/

	//private bool isCurrentLevel;
	//private Transform currentRespawnPoint; // idk 要放在Player還是level manager
	private GameObject currentRespawnPoint; // default is child 0 -> 0-> 0-> 0 //basic -> respawn points -> trigger ->pos

	[HideInInspector] public UnityEvent levelSetUpEvent; //set up crystal, gate ...

	//change level
	//[SerializeField] private float levelChangeFreezeTime;

	// Start is called before the first frame update
	void Start()
	{
		/*//myCameraTarget = transform.GetChild(1).GetChild(3).gameObject; // 1 -> camera component, 3 -> camera target
		//camLB = transform.GetChild(1).GetChild(0); // 1->camera component, 0 -> left bottom
		//camRT = transform.GetChild(1).GetChild(1); // 1->camera component, 1 -> right top
		//myVirtualCam = transform.GetChild(1).GetChild(0).GetComponent<CinemachineVirtualCamera>();  // child 1 -> camera component , 0 ->virtual cam*/
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();

		//myVirtualCam.Follow = player.transform;
		//myVirtualCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = myVirtualCam.transform.parent.GetChild(1).GetComponent<Collider2D>(); // my virtual cam parent -> cam component , child 1 -> boundary

		currentRespawnPoint = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;

		for(int i = 0; i < transform.GetChild(1).GetChild(0).childCount; i++) // 1 -> camera component, 0 -> virtual cams
		{
			//if (i == 0) swapCamera(transform.GetChild(1).GetChild(0).GetChild(i).GetComponent<CinemachineVirtualCamera>());
			currentCam = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>();
			transform.GetChild(1).GetChild(0).GetChild(i).GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
			//transform.GetChild(1).GetChild(0).GetChild(i).GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = transform.GetChild(1).GetChild(2).GetComponent<Collider2D>(); // 1 -> cam component, 2 -> boundary
		}

		for(int i = 0; i < transform.GetChild(2).childCount; i++) // child 2 -> level objects
		{
			if (transform.GetChild(2).GetChild(i).tag == "RechargeCrystal") levelSetUpEvent.AddListener(transform.GetChild(2).GetChild(i).GetComponent<RechargeCrystalScript>().regainPower);
			if (transform.GetChild(2).GetChild(i).tag == "Gate") levelSetUpEvent.AddListener(transform.GetChild(2).GetChild(i).GetComponent<GateScript>().gateReset);
			//if (transform.GetChild(2).GetChild(i).tag == "BreakablePlatform") levelSetUpEvent.AddListener(transform.GetChild(2).GetChild(i).GetComponent<BreakablePlatformScript>().restoreAfterBreak);
		}

		//blockage -> when player exit this level, enable this to prevent player from returning
		transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = Color.clear; // 0 -> basic, 2 -> blockage
		transform.GetChild(0).GetChild(2).gameObject.SetActive(false); // 0 -> basic, 2 -> blockage
	}

	// Update is called once per frame
	void LateUpdate()
	{
		/*if (isCurrentLevel)
		{
			cameraMain();
		}*/
	}

	/*void FixedUpdate()
	{
		if (isCurrentLevel)
		{
			cameraMain();
		}
	}*/

	public void startLevel()
	{
		levelSetUpEvent.Invoke();
		//isCurrentLevel = true;
		//transform.GetChild(3).GetComponent<CinemachineVirtualCamera>().Priority = 11;
		currentCam.Priority = 11;
		player.transform.position = currentRespawnPoint.transform.position;
		currentRespawnPoint.transform.parent.GetComponent<RespawnPointScript>().changeCameraAfterRespawn();

	}

	public void disableLevel() // means leave this level
	{
		//isCurrentLevel = false;
		currentCam.Priority = 10;
		//transform.GetChild(0).GetChild(2).gameObject.SetActive(true); // 0 -> basic, 2 -> blockage

		//player.freezeStart(levelChangeFreezeTime);
	}

	public void restartLevel()
	{
		//player.transform.position = transform.GetChild(0).GetChild(0).transform.position; //child 0 -> basic, 0 -> respawn
		player.transform.position = currentRespawnPoint.transform.position;
		currentRespawnPoint.transform.parent.GetComponent<RespawnPointScript>().changeCameraAfterRespawn();
		levelSetUpEvent.Invoke();

		GameObject[] BreakablePlatforms = GameObject.FindGameObjectsWithTag("BreakablePlatform");
		foreach (GameObject bp in BreakablePlatforms)
		{
			bp.GetComponent<BreakablePlatformScript>().restoreAfterBreak();
		}
	}

	public void enableBlockage()
	{
		transform.GetChild(0).GetChild(2).gameObject.SetActive(true); // 0 -> basic, 2 -> blockage
	}



	public void swapRespawnPoint(GameObject newRespawnPoint)
	{
		if (currentRespawnPoint != newRespawnPoint)
		{
			currentRespawnPoint = newRespawnPoint;
		}
	}

	#region camera

	public void swapCamera(CinemachineVirtualCamera newCam)
	{
		if(currentCam != newCam)
		{
			if(currentCam != null)
				currentCam.Priority = 10;
			currentCam = newCam;
			newCam.Priority = 11;
		}
		
	}



	#endregion


}
