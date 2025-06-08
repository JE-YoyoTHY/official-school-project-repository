using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class MainMenuManager : MonoBehaviour
{
	[Header("Main Menu Objects")]
	[SerializeField] private DecorationManager decorManager;
	[SerializeField] private GameObject loadingBar;
	[SerializeField] private Image loadingBarImage;
	[SerializeField] private GameObject[] objectsToHide;
	[SerializeField] private GameObject[] objectsToHideWhenSceneIsLoading;
	[SerializeField] private CinemachineVirtualCamera mainMenuCam;
	//[SerializeField] private GameObject m_mainCamera;
	

	[Header("Scene field")]
	[SerializeField] private SceneField mainMenu;
	[SerializeField] private SceneField persistentGameplay;
	[SerializeField] private SceneField level;

	[Header("Modify")]
	[SerializeField] private float loadAnimDuration;
	private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

	[Header("Start Performance")]
	//private Coroutine startPerformanceCoroutine;
	[SerializeField] private CinemachineVirtualCamera slabCam;
	[SerializeField] private float slabWaitTime;

	

	private void Awake()
	{
		loadingBar.SetActive(false);
	}
    private void Start()
    {
        playEnvironmentalSFX();
    }


    public void startGame()
	{
		//if (decorManager != null)
		//{
		//	decorManager.currentTweener.Kill();
		//}

		//hide menu
		//hideMenu();

		//load scene
		//sceneToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay));
		//sceneToLoad.Add(SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive));

		//loading bar
		//if (loadingBar != null)
		//{
        //    loadingBar.SetActive(true);
        //    StartCoroutine(progressLoadingBar());
		//}

		StartCoroutine(startGamePerformance());
		
	}

	private IEnumerator startGamePerformance()
	{
		//setup
		hideMenu();

		// slab
		mainMenuCam.enabled = false;
		slabCam.enabled = true;

		float t = slabWaitTime;
		yield return new WaitForSeconds(t);


		#region change scene in coroutine

		//change scene
		if (decorManager != null)
		{
			decorManager.currentTweener.Kill();
		}

		//hide menu
		//hideMenu();

		//load scene

		sceneToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay, LoadSceneMode.Additive));
		//sceneToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay));
		//m_mainCamera.SetActive(false);

		sceneToLoad.Add(SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive));
		//sceneToLoad.Add(SceneManager.UnloadSceneAsync(mainMenu));
		//SceneManager.UnloadSceneAsync(mainMenu);
		//loading bar
		if (loadingBar != null)
		{
            loadingBar.SetActive(true);
            StartCoroutine(progressLoadingBar());
		}

		#endregion
	}

	private void hideMenu()
	{
		for(int i = 0; i <  objectsToHide.Length; i++)
		{
			objectsToHide[i].SetActive(false);
		}
	}

	private IEnumerator progressLoadingBar()
	{
		float loadProgress = 0f;

		for(int i = 0; i < sceneToLoad.Count; i++)
		{
			//if(sceneToLoad[0].isDone) m_mainCamera.SetActive(false);
			if (sceneToLoad[0].isDone)
			{
				foreach (var obj in objectsToHideWhenSceneIsLoading)
					obj.SetActive(false);
			}

			while (!sceneToLoad[i].isDone)
			{
				loadProgress += sceneToLoad[i].progress;
				loadingBarImage.fillAmount = loadProgress / sceneToLoad.Count;
				yield return null;
			}
		}
	}

	public void playEnvironmentalSFX()
	{
		SFXManager.playSFXOneShot(SoundDataBase.SFXType.Sparrow);
		SFXManager.playSFXOneShot(SoundDataBase.SFXType.Frog);
		SFXManager.playSFXOneShot(SoundDataBase.SFXType.Breeze);
		SFXManager.playSFXOneShot(SoundDataBase.SFXType.Cicada);
	}
}
