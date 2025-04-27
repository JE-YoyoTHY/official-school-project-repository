using UnityEngine;

public class BackgroundChange : MonoBehaviour
{
    //[SerializeField] private Sprite[] backgroundImages;
    [SerializeField] private GameObject[] backgroundImages;
	[SerializeField] private float[] lightsIntensity;
    private int currentIndex = 0;

	private void Start()
	{
		//Camera cam = Camera.main;
		transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main; //child 0 for canvas
		//transform.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
	}



	public void swapBackground(int i/*, float lightIntensity*/){ //from current to i
        //disable current
        backgroundImages[i].SetActive(false);

        //enable next
        currentIndex = i;
        backgroundImages[i].SetActive(true);

		//light
		LogicScript.instance.setGlobalLightIntensity(lightsIntensity[i]);
    }

}