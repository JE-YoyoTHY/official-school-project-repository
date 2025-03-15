using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointScript : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera currentCameraAfterRespawn;
	private LevelManagerScript levelManager;


    // Start is called before the first frame update
    void Start()
    {
        levelManager = transform.parent.parent.parent.GetComponent<LevelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void changeCameraAfterRespawn()
	{
		if(currentCameraAfterRespawn != null)
		{
			levelManager.swapCamera(currentCameraAfterRespawn, 0);
		}
	}
}
