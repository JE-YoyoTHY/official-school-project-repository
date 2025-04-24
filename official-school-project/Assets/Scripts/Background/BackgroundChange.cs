using UnityEngine;

public class BackgroundChange : MonoBehaviour
{
    //[SerializeField] private Sprite[] backgroundImages;
    [SerializeField] private GameObject[] backgroundImages;
    private int currentIndex = 0;

    public void swapBackground(int i){ //from current to i
        //disable current
        backgroundImages[i].SetActive(false);

        //enable next
        currentIndex = i;
        backgroundImages[i].SetActive(true);
    }

}