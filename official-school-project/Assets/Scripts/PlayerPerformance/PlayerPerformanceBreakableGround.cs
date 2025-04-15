using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformanceBreakableGround : MonoBehaviour
{
	[SerializeField] private Sprite[] images;
	
	[SerializeField] private string tileName;
	public string m_tileName { get { return tileName; } }

	public PlayerPerformanceBreakableGround tileOnLeft;
	public PlayerPerformanceBreakableGround tileOnRight;

	// Start is called before the first frame update
	void Start()
    {
		GetComponent<SpriteRenderer>().sprite = images[Random.Range(0, images.Length)];
		StartCoroutine(mergeCollider());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void groundBreak()
	{
		GetComponent<ParticleCommonScript>().emitParticle();
		gameObject.SetActive(false);
	}

	private IEnumerator mergeCollider() // merge by disable other collider nearby and add new collider to the leftmost one
	{
		yield return new WaitForSeconds(1);


		if (tileOnLeft != null) yield break;
		//isLeftMost = true;

		int d = depth(1);

		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<BoxCollider2D>().usedByComposite = true;

		//print(d);

		for (int i = 1; i < d; i++)
		{
			//Collider2D newColl = gameObject.AddComponent(typeof(Collider2D)) as Collider2D;
			BoxCollider2D newColl = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
			newColl.enabled = true;
			newColl.usedByComposite = true;
			newColl.offset = Vector2.right * i;
		}


	}

	public int depth(int d) // d for current depth
	{
		GetComponent<BoxCollider2D>().enabled = false;

		if (tileOnRight != null) return tileOnRight.depth(d + 1);
		else return d;
	}
}
