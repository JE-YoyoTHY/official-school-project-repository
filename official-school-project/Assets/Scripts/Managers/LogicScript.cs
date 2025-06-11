using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
	[SerializeField] private SceneField mainMenu;

	public UnityEvent freezeEndEvent;

	public List<TutorialShadeScript> tutorialShades;

	[SerializeField] private Light2D globalLight;

	private float freezeTimer;

	[SerializeField] private ComicPageScript[] comicPages;

	//private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();
	public List<SceneField> scenesCanLoad;
	private SceneField lastSceneLoaded;
	//[SerializeField] private SceneField mainMenu;

	[SerializeField] private GameObject settingTab;

	//pause
	public bool isPaused {  get; private set; }
	public PauseSource pauseSource;

	[SerializeField] private SoundDataBase soundData;



    // Start is called before the first frame update
    void Start()
    {
		//gridColor();

		//GameObject[] shades = GameObject.FindGameObjectsWithTag("TutorialShade");
		//foreach (GameObject shade in shades)
		//{
		//	freezeEndEvent.AddListener(shade.GetComponent<TutorialShadeScript>().freezeEnd);
		//}

		SceneManager.UnloadSceneAsync(mainMenu);
		setSettingSliderValue();
		settingTab.SetActive(false);
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

    #region pause

	public void pauseGame(PauseSource source)
	{	
		isPaused = true;
		Time.timeScale = 0f;
		pauseSource = source;

		InputManagerScript.instance.playerInput.SwitchCurrentActionMap("Pause");
	}

	public void unpauseGame()
	{	
		isPaused = false;
		Time.timeScale = 1f;
		pauseSource = PauseSource.none;

        InputManagerScript.instance.playerInput.SwitchCurrentActionMap("Player");
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

    #region scene

	public void myLoadScene(SceneField scene)
	{
		//print(scene);

		//foreach(var x in scenesCanLoad)
		//{
		//	print(x == scene);

		//	if (x == scene)
		//	{
  //              SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		//		scenesCanLoad.Remove(x);
		//		print("successful");

		//		break;
  //          }
		//}
        if (scene != lastSceneLoaded) SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        lastSceneLoaded = scene;

        //sceneToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));

    }

	public void backToMainMenu()
	{
		SceneManager.LoadSceneAsync(mainMenu);
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


	


	

	public void comicStart(string id)
	{
		//print("Called");
		//GameObject[] comicsPages = GameObject.FindGameObjectsWithTag("ComicPage");
		foreach (var page in comicPages)
		{
			//print("called");
			if (page.pageID == id)
			{
				//print("found");

				page.showPage();
				pauseGame(PauseSource.comic);
			}
				

        }
	}

	public void setSettingSliderValue()
	{
		MusicManager.setSliderValue(soundData.musicVolume);
		SFXManager.setSliderValue(soundData.sfxVolume);
		AmbientSoundManager.setSliderValue(soundData.ambientSoundVolume);
		CameraShakeSliderValueManager.setSliderValue(soundData.cameraShakeDegree);
	}


}

public enum PauseSource
{
	setting,
	comic,
	none
}
