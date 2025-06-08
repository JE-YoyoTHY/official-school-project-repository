using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZeusPhaseOneScript : MonoBehaviour
{
    //public UnityEvent damagedEvent;
    [SerializeField] private BossLightningManagerScript bossLightningManager;
    [SerializeField] private BossLightningInstanceScript bossLightningInstance;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        myIgnoreCollision(true);
        gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showUp()
    {
        gameObject.SetActive(true);
        animator = GetComponent<Animator>();
        animator.Play("ZeusIdle");

        myIgnoreCollision(true);
    }

    public void zeusDamaged()
    {
        //damagedEvent.Invoke();
        bossLightningManager.setAttackState("Expel");
        bossLightningInstance.setStaticWallState(false);
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
