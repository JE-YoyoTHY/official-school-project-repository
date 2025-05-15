using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusPhaseTwoScript : MonoBehaviour
{
    //how boss work
    /* Boss stay at same position
     * it keeps attack until a certain point
     * and he becomes vulnerable, then player needs to shoot fireball
     * when boss is hit, change area and continue attacking
     */
    //how attack work
    /* summon a lightning
     * and wait for certain time
     * summon another lighting
     */


    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private ZeusBossFightData[] fightDatas;
    private int currentDataIndex;
    private float attackWaitTimeCounter;

    private bool isAttackActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LogicScript.instance.isFreeze())
        {
            if (isAttackActive)
            {
                attackMain();
            }
        }
    }

    private void attackMain()
    {
        if(attackWaitTimeCounter < 0)
        {

        }
        else
        {
            attackWaitTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void summonAttack(int i)
    {
        BossLightningInstanceScript attackInstance = Instantiate(attackPrefab).GetComponent<BossLightningInstanceScript>();
        attackInstance.summon(fightDatas[i].lightningAttackData, fightDatas[i].attackPos.transform.position);
    }

    public void resetBoss()
    {
        isAttackActive = false;
        currentDataIndex = 0;
    }
}

[System.Serializable]
public class ZeusBossFightData
{
    //public ZeusBossFightDataState dataState;

    //attack
    public BossLightningAttackData lightningAttackData;
    public GameObject attackPos;
    public float waitTimeAfterAttack;

    
}

/*public enum ZeusBossFightDataState
{
    Attack,
    WaitUntilHit
}*/
