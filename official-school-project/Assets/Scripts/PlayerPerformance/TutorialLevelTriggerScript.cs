using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelTriggerScript : MonoBehaviour
{
    [SerializeField] private TutorialShadeScript shade;


    private void OnTriggerEnter2D(Collider2D collision)
	{
	    if (collision.gameObject == PlayerControlScript.instance.gameObject){
            //shade.startLoop();
            //shade.isWaiting = false;
            shade.tutorialStarted = false;
            //print("dlfhal");
        }
	}

}
