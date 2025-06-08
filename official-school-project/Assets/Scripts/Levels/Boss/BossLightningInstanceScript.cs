using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLightningInstanceScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private AnimationCurve flickerCurve;
    public bool isStaticWall;
    //[SerializeField] private bool initialActiveState;
    /*[SerializeField]*/ private float myAttackPrepareTime;
    /*[SerializeField]*/ private float myAttackDuration;

    private const int killZoneLayer = 7;

    [Header("Animation")]
    [SerializeField] private GameObject thunderSprite;
    private GameObject thunderInstance;
    [SerializeField] private Sprite[] thunderImages;
    

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
        animator = GetComponent<Animator>();
        animator.enabled = false;

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
        spriteRenderer.color = Color.clear;
        spriteRenderer.enabled = false;
        gameObject.layer = killZoneLayer;
        GetComponent<Collider2D>().enabled = true;

        thunderInstance = Instantiate(thunderSprite, transform.position, Quaternion.identity);
        SpriteRenderer thunderSpriteRenderer = thunderInstance.GetComponent<SpriteRenderer>();
        thunderSpriteRenderer.size = transform.localScale / thunderSprite.transform.localScale.x;

        //animation
        //animator.enabled = true;
        //if (transform.localScale.y / transform.localScale.x > 3)
        //{
        //    animator.Play("ThunderVertical");
        //}
        //else
        //{
        //    animator.Play("ThunderNet");
        //}


        float i = new float();
        while (t > 0)
        {
            if (!LogicScript.instance.isFreeze())
                t -= Time.deltaTime;

            i = (1 - (t / myAttackDuration)) * (thunderImages.Length - 1);
            if (i >= thunderImages.Length - 1)
                thunderInstance.GetComponent<SpriteRenderer>().sprite = thunderImages[thunderImages.Length - 1];
            else
                thunderInstance.GetComponent<SpriteRenderer>().sprite = thunderImages[(int)Mathf.Floor(i) + 1];

            yield return null;
        }

        //Destroy(thunderInstance);
        //Destroy(gameObject);
        destroyAttackInstance();
    }

    public void setStaticWallState(bool active)
    {
        //Destroy(gameObject);
        gameObject.SetActive(active);
    }

    public void destroyAttackInstance()
    {
        Destroy(thunderInstance);
        Destroy(gameObject);
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
