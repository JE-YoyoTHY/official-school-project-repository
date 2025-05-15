using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZeusPowerCrystalManagerScript : MonoBehaviour
{
    public UnityEvent allCrystalGoneEvent;

    private int currentDestroyedCount; // child -> all crystal, child count -> crystal count

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void crystalDestroy()
    {
        currentDestroyedCount++;
        if (currentDestroyedCount == transform.childCount)
        {
            allCrystalGoneEvent.Invoke();
        }
    }

    public void resetZeusPowerCrystal()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ZeusPowerCrystalInstanceScript>().resetCrystal();
        }
        currentDestroyedCount = 0;
    }
}
