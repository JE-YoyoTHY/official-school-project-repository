using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour
{
	[Header("Main Menu Objects")]
	[SerializeField] private GameObject loadingBar;
	[SerializeField] private Image loadingBarImage;
	[SerializeField] private GameObject[] objectsToHide;

	[Header("Scene field")]
	[SerializeField] private SceneField persistentGameplay;
	[SerializeField] private SceneField level;

	private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

	private void Awake()
	{
		loadingBar.SetActive(false);
	}


	public void startGame()
	{
		//set up
		loadingBar.SetActive(true);

		//hide menu
		hideMenu();

		//load scene
		sceneToLoad.Add(SceneManager.LoadSceneAsync(persistentGameplay));
		sceneToLoad.Add(SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive));

		//loading bar
		StartCoroutine(progressLoadingBar());
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
			while (!sceneToLoad[i].isDone)
			{
				loadProgress += sceneToLoad[i].progress;
				loadingBarImage.fillAmount = loadProgress / sceneToLoad.Count;
				yield return null;
			}
		}
	}
}
