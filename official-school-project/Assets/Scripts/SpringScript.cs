using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringScript : MonoBehaviour
{
	private PlayerControlScript player;

	[SerializeField] private float pushForce;
	[SerializeField] private float fireballSpeedScale;

	[SerializeField] private bool removePlayerMoveAbility;

    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			springPlayerTrigger();
		}

		if(collision.gameObject.tag == "Fireball")
		{
			//Vector2 localDir = transform.GetChild(0).localPosition;
			Vector3 deltaPos = transform.GetChild(0).position - transform.position;
			Vector2 localDir = new Vector2(deltaPos.x, deltaPos.y).normalized;
			collision.gameObject.GetComponent<FireballScript>().springPush(localDir.normalized, fireballSpeedScale);
		}
	}

	private void springPlayerTrigger()
	{
		//Vector2 localDir = transform.GetChild(0).localPosition; // child 0 for target
		Vector3 deltaPos = transform.GetChild(0).position - transform.position;
		Vector2 localDir = new Vector2(deltaPos.x, deltaPos.y).normalized;
		//Debug.Log(deltaPos + ", " + localDir);
		player.springPush(localDir.normalized * pushForce, removePlayerMoveAbility, transform.position, transform.GetChild(0).position);
	}

	/*private void springFireballTrigger()
	{
		Vector2 localDir = transform.GetChild(0).localPosition;

	}*/

}
