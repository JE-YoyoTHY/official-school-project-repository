using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformanceBreakableGround : MonoBehaviour
{
	[SerializeField] private Sprite[] images;
	
	[SerializeField] private string tileName;
	public string m_tileName { get { return tileName; } }

    // Start is called before the first frame update
    void Start()
    {
		GetComponent<SpriteRenderer>().sprite = images[Random.Range(0, images.Length)];
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
}
