using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLightningInstanceScript : MonoBehaviour
{
    [SerializeField] private GameObject brightnessBlock;
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
    [SerializeField] private GameObject thunderPreview;
    private GameObject thunderPreviewInstance;

    private void Awake()
    {
        if (brightnessBlock == null)
        {
            brightnessBlock = GameObject.Find("Thunder Brightness");
        }
    }
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
        spriteRenderer.enabled = false;
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
        //spriteRenderer.enabled = false;

        float t = myAttackPrepareTime;
        thunderPreviewInstance = Instantiate(thunderPreview, transform.position, Quaternion.identity);
        SpriteRenderer thunderPreviewSpriteRender = thunderPreviewInstance.GetComponent<SpriteRenderer>();
        thunderPreviewSpriteRender.enabled = true;
        thunderPreviewSpriteRender.size = transform.localScale / thunderPreview.transform.localScale.x;

        //Color color = spriteRenderer.color;
        while (t > 0)
        {
            if (!LogicScript.instance.isFreeze())
                t -= Time.deltaTime;

            //color.a = flickerCurve.Evaluate(1 - (t / myAttackPrepareTime));
            //spriteRenderer.color = color;

            yield return null;
        }

        t = myAttackDuration;
        //spriteRenderer.color = Color.clear;
        thunderPreviewSpriteRender.enabled = false;
        spriteRenderer.enabled = false;
        gameObject.layer = killZoneLayer;
        GetComponent<Collider2D>().enabled = true;

        if (thunderInstance != null) Destroy(thunderInstance);
        thunderInstance = Instantiate(thunderSprite, transform.position, Quaternion.identity);
        SpriteRenderer thunderSpriteRenderer = thunderInstance.GetComponent<SpriteRenderer>();
        thunderSpriteRenderer.size = transform.localScale / thunderSprite.transform.localScale.x;
        brightnessBlock.GetComponent<VisualEffectManager>().applyVFX();
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
        Destroy(thunderPreviewInstance);
    }
}


[System.Serializable]
public class BossLightningAttackData
{
    public Vector2 attackSize;
    public Vector2 attackPosFromAxis; //�����a����m

    public float attackPrepareTime;
    public float attackDuration;

}
