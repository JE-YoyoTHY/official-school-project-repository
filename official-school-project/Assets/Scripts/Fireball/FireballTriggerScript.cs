using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTriggerScript : MonoBehaviour
{
    public bool isColliding;
	public BreakablePlatformScript collidingBreakablePlatform;
    

    private const int groundLayer = 6;

    private void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.layer == groundLayer){
            /*if (collision.gameObject.tag == "BreakablePlatform")
				{
					collision.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
				}*/
            
            
            isColliding = true;
			if (collision.CompareTag("BreakablePlatform")) collidingBreakablePlatform = collision.GetComponent<BreakablePlatformScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.layer == groundLayer){
            isColliding = false;
			if (collision.CompareTag("BreakablePlatform")) collidingBreakablePlatform = null;
		}
    }
}
