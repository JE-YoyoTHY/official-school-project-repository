using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusPowerCrystalInstanceScript : MonoBehaviour
{
    private ZeusPowerCrystalManagerScript crystalManager;

    //private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        crystalManager = transform.parent.GetComponent<ZeusPowerCrystalManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void crystalInstanceDestroy()
    {
        crystalManager.crystalDestroy();

        gameObject.SetActive(false);
        GetComponent<ParticleCommonScript>().emitParticle();
    }

    public void resetCrystal()
    {
        gameObject.SetActive(true);
    }
}
