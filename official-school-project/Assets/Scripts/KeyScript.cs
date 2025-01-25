using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
	[SerializeField] private Color notObtainedColor;
	[SerializeField] private Color obtainedColor;

	private bool isObtained;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Fireball") && !isObtained)
		{
			keyObtain();
		}
	}

	private void keyObtain()
	{
		isObtained = true;
		GetComponent<SpriteRenderer>().color = obtainedColor;

		//inform parent that key is obtained
		transform.parent.parent.GetComponent<GateScript>().keyObtain();
	}

	public void keyReset()
	{
		isObtained = false;
		GetComponent<SpriteRenderer>().color = notObtainedColor;
	}
}
