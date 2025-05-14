using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZeusPhaseOneScript : MonoBehaviour
{
    public UnityEvent damagedEvent;

    // Start is called before the first frame update
    void Start()
    {
        myIgnoreCollision(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showUp()
    {
        gameObject.SetActive(true);
    }

    public void zeusDamaged()
    {
        damagedEvent.Invoke();
        gameObject.SetActive(false);
    }

    private void myIgnoreCollision(bool ignore)
    {
        Collider2D[] colls = PlayerControlScript.instance.GetComponents<Collider2D>();
        foreach (Collider2D coll in colls)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), coll, ignore);
        }
    }
}
