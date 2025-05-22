using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTriggerScript : MonoBehaviour
{
    [SerializeField] private FireballTriggerScript anotherTrigger;


    //public bool isColliding;
    //public fireballTriggerCollidingObject collideObj = fireballTriggerCollidingObject.none;
	public BreakablePlatformScript collidingBreakablePlatform;
    public bool collideWithAirwall;
    public bool collideWithGround;
    public bool collideWithSpring;

    private const int groundLayer = 6;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            /*if (collision.gameObject.tag == "BreakablePlatform")
				{
					collision.gameObject.GetComponent<BreakablePlatformScript>().breakStart(true, null);
				}*/


            //isColliding = true;
            //collideObj = fireballTriggerCollidingObject.ground;
            collideWithGround = true;
			if (collision.CompareTag("BreakablePlatform")) collidingBreakablePlatform = collision.GetComponent<BreakablePlatformScript>();
        }

        if (collision.gameObject.tag == "KillFireballWithoutExplode")
        {
            //collideObj = fireballTriggerCollidingObject.airwall;
            collideWithAirwall = true;
        }

        if (collision.gameObject.tag == "Spring")
        {
            //collideObj = fireballTriggerCollidingObject.spring;
            collideWithSpring = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if(collision.gameObject.layer == groundLayer)
        {
            //isColliding = false;
            //collideObj = fireballTriggerCollidingObject.none;
            collideWithGround = false;
            if (collision.CompareTag("BreakablePlatform")) collidingBreakablePlatform = null;

		}

        if (collision.gameObject.tag == "KillFireballWithoutExplode")
        {
            collideWithAirwall = false;

        }

        if (collision.gameObject.tag == "Spring")
        {
            collideWithSpring = false;

        }


    }
}

public enum fireballTriggerCollidingObject
{
    none,
    ground,
    airwall,
    spring
}
