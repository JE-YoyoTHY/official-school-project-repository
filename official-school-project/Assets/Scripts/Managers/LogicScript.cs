using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class LogicScript : MonoBehaviour
{
	//singleton
	public static LogicScript instance { get; private set; }
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}


	//variable
	public UnityEvent freezeEndEvent;

	public List<TutorialShadeScript> tutorialShades;

	[SerializeField] private Light2D globalLight;

	private float freezeTimer;

	// Start is called before the first frame update
	void Start()
    {
		//gridColor();

		//GameObject[] shades = GameObject.FindGameObjectsWithTag("TutorialShade");
		//foreach (GameObject shade in shades)
		//{
		//	freezeEndEvent.AddListener(shade.GetComponent<TutorialShadeScript>().freezeEnd);
		//}

		
    }

    // Update is called once per frame
    void Update()
    {
		if (isFreeze())
		{
			freezeMain();
		}

	}

	#region freeze

	private void freezeMain()
	{
		freezeTimer -= Time.deltaTime;

		if (freezeTimer <= 0)
		{
			freezeEndEvent.Invoke();
		}
	}

	public void setFreezeTime(float t)
	{
		if (freezeTimer <= 0) // from not freeze to freeze -> set up player
		{
			PlayerControlScript.instance.freezeStart();

			//GameObject[] shades = GameObject.FindGameObjectsWithTag("TutorialShade");
			//foreach(GameObject shade in shades)
			//{
			//	shade.GetComponent<TutorialShadeScript>().freezeStart();
			//}
			//print(tutorialShades.Count);

			if(tutorialShades.Count > 0)
			{
				foreach(var shade in tutorialShades)
				{
					//print(shade);
					shade.freezeStart();
				}
			}

		}

		freezeTimer = t;
	}

	public bool isFreeze()
	{
		if (freezeTimer > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}


	#endregion

	#region grid

	public void gridColor()
	{
		GameObject[] killFbZones = GameObject.FindGameObjectsWithTag("KillFireballWithoutExplode");
		foreach (GameObject killFbZone in killFbZones)
		{
			if (killFbZone.GetComponent<Tilemap>() != null)
				killFbZone.GetComponent<Tilemap>().color = Color.clear;
		}
	}

	public void performanceTileDestroy(string name)
	{
		GameObject[] performanceTiles = GameObject.FindGameObjectsWithTag("PerformanceTile");
		foreach(GameObject performanceTile in performanceTiles)
		{
			PlayerPerformanceBreakableGround breakableGround = performanceTile.GetComponent<PlayerPerformanceBreakableGround>();
			if (breakableGround.m_tileName == name) breakableGround.groundBreak();
		}
	}
	#endregion


	public void tutorialShadeFreezeTime(TutorialShadeScript shade)
	{
		//GameObject[] shades = GameObject.FindGameObjectsWithTag("TutorialShade");
		//foreach (GameObject shade in shades)
		//{
		//	freezeEndEvent.AddListener(shade.GetComponent<TutorialShadeScript>().freezeEnd);
		//}

		tutorialShades.Add(shade);
		freezeEndEvent.AddListener(shade.freezeEnd);
	}

	public void setGlobalLightIntensity(float intensity)
	{
		globalLight.intensity = intensity;
	}

}
