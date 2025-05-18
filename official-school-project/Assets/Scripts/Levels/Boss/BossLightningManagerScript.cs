using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLightningManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private BossAttackState initialState;

    [Header("Loop")]
    [SerializeField] private float attackInterval;
    //private bool isAttackActive;
    private BossAttackState attackState;
    private float attackTimeCounter;
    [SerializeField] private BossLightningAttackData[] attackDatas;

    [Header("Expel")]
    [SerializeField] private GameObject expelAttackLeftMost;
    [SerializeField] private GameObject expelAttackRightMost;
    [SerializeField] private BossLightningAttackData expelAttackData;
    [SerializeField] private float expelAttackInterval;



    // Start is called before the first frame update
    void Start()
    {
        attackTimeCounter = attackInterval;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LogicScript.instance.isFreeze())
        {
            if (attackState == BossAttackState.Loop)
                attackLoopMain();
            //if (attackState == BossAttackState.Expel)
            //    attackExpelMain();
        }
        
    }

    #region attack -> loop

    private void attackLoopMain()
    {
        if (attackTimeCounter < 0)
        {
            attackStart();
        }
        else
        {
            attackTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void attackStart()
    {
        attackTimeCounter = attackInterval;

        BossLightningInstanceScript attackInstance = Instantiate(attackPrefab).GetComponent<BossLightningInstanceScript>();
        attackInstance.summon(attackDatas[Random.Range(0, attackDatas.Length)], PlayerControlScript.instance.transform.position);
    }

    #endregion

    #region attack -> expel

    /* 由左端點向右端點一直攻擊
     * 將玩家驅逐
     */
    private IEnumerator expelAttackMain()
    {
        //kill all existing lighting
        GameObject[] lightningAttacks = GameObject.FindGameObjectsWithTag("LightningAttackInstance");
        foreach (GameObject lightningAttack in lightningAttacks)
        {
            if (!lightningAttack.GetComponent<BossLightningInstanceScript>().isStaticWall)
                Destroy(lightningAttack.gameObject);
        }


        float current_x = expelAttackLeftMost.transform.position.x;

        while(current_x < expelAttackRightMost.transform.position.x)
        {
            float t = expelAttackInterval;
            while(t > 0)
            {
                if (!LogicScript.instance.isFreeze())
                    t -= Time.deltaTime;

                yield return null;
            }


            BossLightningInstanceScript attackInstance = Instantiate(attackPrefab).GetComponent<BossLightningInstanceScript>();
            Vector3 summonPos = PlayerControlScript.instance.transform.position;
            summonPos.x = current_x;
            attackInstance.summon(expelAttackData, summonPos);

            current_x += expelAttackData.attackSize.x - 1; // 1 for 重疊
        }

    }

    #endregion

    #region public methods

    public void setAttackState(string state)
    {
        //attackState = state;
        if (state == "Wait") attackState = BossAttackState.Wait;
        if (state == "Loop") attackState = BossAttackState.Loop;
        if (state == "Expel")
        {
            attackState = BossAttackState.Expel;
            StartCoroutine(expelAttackMain());
        }
    }

    public void resetManager()
    {
        //isAttackActive = true;
        //attackState = BossAttackState.Loop;
        attackState = initialState;
        attackTimeCounter = attackInterval;
        gameObject.SetActive(true);

        StopAllCoroutines();

        GameObject[] lightningAttacks = GameObject.FindGameObjectsWithTag("LightningAttackInstance");
        foreach(GameObject lightningAttack in lightningAttacks)
        {
            if(!lightningAttack.GetComponent<BossLightningInstanceScript>().isStaticWall)
                Destroy(lightningAttack.gameObject);
        }
    }
    #endregion
}

public enum BossAttackState
{
    Wait,
    Loop,
    Expel //when every crystal is destroyed and boss is hit
}
