using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerGroundTriggerScript : MonoBehaviour
{

	public bool isGrounded { get; private set; } = false;
	private const int groundLayer = 6;

	private BreakablePlatformScript currentBreakablePlatform;
	public UnityEvent PlayerLandEvent;
	[SerializeField] private Tilemap groundTileMap;
	

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (groundTileMap)
        {
            print(groundTileMap.WorldToCell(transform.position));
            print(groundTileMap.GetTile(groundTileMap.WorldToCell(transform.position)));
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayer && isGrounded == false)
		{
			print("land sfx");
            SFXManager.playSFXOneShot(SoundDataBase.SFXType.Land, 0.35f);
			PlayerLandEvent.Invoke();
			print(collision.gameObject.name);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.layer == groundLayer)
		{
			isGrounded = true;
            if (collision.gameObject.tag == "BreakablePlatform")
			{
				currentBreakablePlatform = collision.gameObject.GetComponent<BreakablePlatformScript>();
				currentBreakablePlatform.traversalThroughAdjacentTiles(null);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == groundLayer)
		{
			if (collision.CompareTag("BreakablePlatform")) currentBreakablePlatform = null;

			isGrounded = false;
		}
	}

	public void leaveGround(bool byJump)
	{
		if(isGrounded && byJump && currentBreakablePlatform != null)
		{
			currentBreakablePlatform.playerJumpOnThisTraversal(null);
		}


		isGrounded = false;
	}

	public bool onGround()
	{
		return isGrounded;
	}

	//public Tile getTileUnderFeet()
	//{

	//}
}
