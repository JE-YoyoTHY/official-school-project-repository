using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLightningInstanceScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private AnimationCurve flickerCurve;
    public bool isStaticWall;
    //[SerializeField] private bool initialActiveState;
    /*[SerializeField]*/ private float myAttackPrepareTime;
    /*[SerializeField]*/ private float myAttackDuration;

    private const int killZoneLayer = 7;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(initialActiveState);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void summon(BossLightningAttackData attackData, Vector3 startPos)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localScale = attackData.attackSize;
        transform.position = startPos + (Vector3)attackData.attackPosFromAxis;

        myAttackDuration = attackData.attackDuration;
        myAttackPrepareTime = attackData.attackPrepareTime;

        StartCoroutine(attackMain());
    }

    IEnumerator attackMain()
    {
        float t = myAttackPrepareTime;
        Color color = spriteRenderer.color;
        while (t > 0)
        {
            if (!LogicScript.instance.isFreeze())
                t -= Time.deltaTime;

            color.a = flickerCurve.Evaluate(1 - (t / myAttackPrepareTime));
            spriteRenderer.color = color;

            yield return null;
        }

        t = myAttackDuration;
        spriteRenderer.color = Color.yellow;
        gameObject.layer = killZoneLayer;
        GetComponent<Collider2D>().enabled = true;
        while (t > 0)
        {
            if (!LogicScript.instance.isFreeze())
                t -= Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }

    public void setStaticWallState(bool active)
    {
        //Destroy(gameObject);
        gameObject.SetActive(active);
    }
}


[System.Serializable]
public class BossLightningAttackData
{
    public Vector2 attackSize;
    public Vector2 attackPosFromAxis; //Â÷ª±®aªº¦ì¸m

    public float attackPrepareTime;
    public float attackDuration;

}
