using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Events;
using Cinemachine;

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
    //[SerializeField] private Vector2 attackDisplayIndex; // x for i, y for j, for debug method
    [SerializeField] private int profileDisplayIndex; [SerializeField] private int dataDisplayIndex;
    [SerializeField] private GameObject[] attackPositions; // every object contains multiple position -> j
    //[SerializeField] private ZeusBossFightData[][] fightDatas;
    [SerializeField] private ZeusPhaseTwoAttackProfile[] attackProfiles;
    private int currentProfileIndex; // i
    private int currentDataIndex; // j
    private float attackWaitTimeCounter;
    public UnityEvent[] resetEvents;
    public UnityEvent[] damagedEvents;
    [SerializeField] private GameObject[] respawnPoints;
    [SerializeField] private CinemachineVirtualCamera[] vcams;
    [SerializeField] private GameObject[] bossPositions;

    private bool isAttackActive = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        myIgnoreCollision(true);

        animator = GetComponent<Animator>();
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
        if (currentDataIndex < attackProfiles[currentProfileIndex].fightDatas.Length)
        {
            if(attackWaitTimeCounter < 0)
            {
                attackStart();
            }
            else
            {
                attackWaitTimeCounter -= Time.fixedDeltaTime;
            }
        }
        else
        {
            isAttackActive = false;
        }
        
    }

    private void attackStart()
    {
        summonAttack();
        attackWaitTimeCounter = attackProfiles[currentProfileIndex].fightDatas[currentDataIndex].waitTimeAfterAttack;
        currentDataIndex++;

        animator.Play("ZeusAttack");

    }

    private void summonAttack()
    {
        BossLightningInstanceScript attackInstance = Instantiate(attackPrefab).GetComponent<BossLightningInstanceScript>();
        attackInstance.summon(attackProfiles[currentProfileIndex].fightDatas[currentDataIndex].lightningAttackData, attackProfiles[currentProfileIndex].fightDatas[currentDataIndex].attackPos);
    }

    public void setActiveState(bool state)
    {
        gameObject.SetActive(state);
    }

    //private void 

    #region public method

    public void resetBoss()
    {
        isAttackActive = true; // maybe it should be changed
        //currentProfileIndex = 0;
        currentDataIndex = 0;

        //kill lightning
        GameObject[] lightningAttacks = GameObject.FindGameObjectsWithTag("LightningAttackInstance");
        foreach (GameObject lightningAttack in lightningAttacks)
        {
            if (!lightningAttack.GetComponent<BossLightningInstanceScript>().isStaticWall)
                Destroy(lightningAttack.gameObject);
        }

        resetEvents[currentProfileIndex].Invoke();
        animator.Play("ZeusIdle");
    }

    [ContextMenu("Fight start")]
    public void startBossFight()
    {
        isAttackActive = true;
        currentProfileIndex = 0;
        currentDataIndex = 0;
    }


    public void zeusDamaged()
    {
        if(!isAttackActive)
        {
            //print("Zeus was hit");
            damagedEvents[currentProfileIndex].Invoke();
            //currentProfileIndex++;
            //currentDataIndex = 0;
        }
    }

    public void enterNewRoom()
    {
        currentProfileIndex++;
        currentDataIndex = 0;
        isAttackActive = true;

        transform.parent.parent.GetComponent<LevelManagerScript>().swapRespawnPoint(respawnPoints[currentProfileIndex]);
        transform.parent.parent.GetComponent<LevelManagerScript>().swapCamera(vcams[currentProfileIndex], 1f);

        transform.position = bossPositions[currentProfileIndex].transform.position;
    }

    #endregion

    private void myIgnoreCollision(bool ignore)
    {
        Collider2D[] colls = PlayerControlScript.instance.GetComponents<Collider2D>();
        foreach (Collider2D coll in colls)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), coll, ignore);
        }
    }

    #region debug function

    public void applyPosition()
    {
        for (int i = 0; i < attackProfiles.Length; i++)
        {
            for (int j = 0; j < attackProfiles[i].fightDatas.Length; j++)
            {
                attackProfiles[i].fightDatas[j].attackPos = attackPositions[i].transform.GetChild(j).position;
            }
        }
    }

    public void showCurrentAttack()
    {
        //attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].attackPos.GetComponent<ZeusPhaseTwoAttackPosScript>().showUp(attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].lightningAttackData.attackSize);
        attackPositions[profileDisplayIndex].transform.GetChild(dataDisplayIndex).GetComponent<ZeusPhaseTwoAttackPosScript>().showUp(attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].lightningAttackData.attackSize);
    }
    public void showNextAttack()
    {
        //attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].attackPos.GetComponent<ZeusPhaseTwoAttackPosScript>().hideAttack();
        attackPositions[profileDisplayIndex].transform.GetChild(dataDisplayIndex).GetComponent<ZeusPhaseTwoAttackPosScript>().hideAttack();

        //attackDisplayIndex.y++;
        dataDisplayIndex++;
        if(dataDisplayIndex >= attackProfiles[profileDisplayIndex].fightDatas.Length) dataDisplayIndex = 0;

        //attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].attackPos.GetComponent<ZeusPhaseTwoAttackPosScript>().showUp(attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].lightningAttackData.attackSize);
        attackPositions[profileDisplayIndex].transform.GetChild(dataDisplayIndex).GetComponent<ZeusPhaseTwoAttackPosScript>().showUp(attackProfiles[profileDisplayIndex].fightDatas[dataDisplayIndex].lightningAttackData.attackSize);
    }

    public void hideAllAttack()
    {
        for (int i = 0; i < attackProfiles.Length; i++)
        {
            for (int j = 0; j < attackProfiles[i].fightDatas.Length; j++)
            {
                //attackProfiles[i].fightDatas[j].attackPos.GetComponent<ZeusPhaseTwoAttackPosScript>().hideAttack();
                attackPositions[i].transform.GetChild(j).GetComponent<ZeusPhaseTwoAttackPosScript>().hideAttack();
            }
        }
    }

    [ContextMenu("Skip This Room")]
    public void skipRoom()
    {
        currentDataIndex = attackProfiles[currentProfileIndex].fightDatas.Length - 1;
        isAttackActive = false;

    }

    #endregion
}

[System.Serializable]
public class ZeusBossFightData
{
    //public ZeusBossFightDataState dataState;

    //attack
    public BossLightningAttackData lightningAttackData;
    //public GameObject attackPos;
    public Vector3 attackPos;
    public float waitTimeAfterAttack;

    
}

/*public enum ZeusBossFightDataState
{
    Attack,
    WaitUntilHit
}*/


#if UNITY_EDITOR

[CustomEditor(typeof(ZeusPhaseTwoScript))]
public class ZeusPhaseTwoCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();

        ZeusPhaseTwoScript zeus = (ZeusPhaseTwoScript)target;

        if (GUILayout.Button("Apply Positions", GUILayout.Width(180f)))
        {
            zeus.applyPosition();
        }

        if (GUILayout.Button("Show Current Attack", GUILayout.Width(180f)))
        {
            zeus.showCurrentAttack();
        }

        if (GUILayout.Button("Show Next Attack", GUILayout.Width(180f)))
        {
            zeus.showNextAttack();
        }

        if (GUILayout.Button("Hide All", GUILayout.Width(180f)))
        {
            zeus.hideAllAttack();   
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(zeus);
        }
    }


}


#endif

