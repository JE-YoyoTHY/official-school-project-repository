using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTriggerScript : MonoBehaviour
{
    public bool isColliding;
    

    private const int groundLayer = 6;

    private void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.layer == groundLayer){
            /*if (collision.gameObject.tag == "BreakablePlatform")
				{
					collision.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
				}*/
            
            
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.layer == groundLayer){
            isColliding = false;
        }
    }
}
