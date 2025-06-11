using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TutorialShadeAnimationScript : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private TutorialShadeScript followingShade;
    private Rigidbody2D shadeRB;
    //private PlayerGroundTriggerScript playerGroundTriggerScript;

    #region States
    // states,
    public string currentState;
    public string STATE_IDLE { get; private set; } = "TutorIdle";
    public string STATE_RUN { get; private set; } = "TutorRun";
    public string STATE_JUMP { get; private set; } = "TutorJump";
    public string STATE_FALL { get; private set; } = "TutorFall";
    public string STATE_FALL_RIGHT { get; private set; } = "PlayerFallRight";
    public string STATE_FALL_LEFT { get; private set; } = "PlayerFallLeft";
    public string STATE_LAND { get; private set; } = "PlayerLand";
    public string STATE_HARD_LAND { get; private set; } = "PlayerHardLand";
    public string STATE_PARKOUR_ROLL { get; private set; } = "PlayerParkourRoll";

    private List<string> availableAnims = new List<string>()
    {
        "TutorRun", "TutorIdle", "TutorJump", "TutorFall", "PlayerFallRight", "PlayerFallLeft", "PlayerLand"
    };
    #endregion

    public bool canChangeState { get; private set; } = true;
    [SerializeField] private Vector2 previousVelocity;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float hardLandMinVy = 10f; 
    [SerializeField] private float parkourRollMinVx = 10f;  
    [SerializeField] private bool previousIsGrounded;
    [SerializeField] private bool currentIsGrounded;
    private sbyte facingDir;
    private Dictionary<string, bool> stateCondition = new Dictionary<string, bool>();
    void Start()
    {

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //player = GameObject.FindGameObjectWithTag("Player");
        //playerControlScript = player.GetComponent<PlayerControlScript>();
        //playerGroundTriggerScript = player.GetComponentInChildren<PlayerGroundTriggerScript>(false);
        shadeRB = followingShade.GetComponent<Rigidbody2D>();
        currentVelocity = shadeRB.velocity;
        refreshStateCondition();
    }

    void Update()
    {
        //transform.position = GameObject.Find("Player").transform.position;
        //Vector3 playerPos = GameObject.Find("Player").transform.position;
        Vector3 shadePos = followingShade.transform.position;
        transform.position = new Vector3(shadePos.x, shadePos.y + 0.5f, transform.position.z);
        //facingDir = playerControlScript.moveKeyValue;
        facingDir = followingShade.moveDir;
        flipPlayerSprite(facingDir);

        stateDetect();
        refreshStateCondition();

    }

    private void FixedUpdate()
    {
        refreshVelocityInfo();
    }
    public void changeState(string newState)
    {
        if (canChangeState == false) { return; }

        currentState = newState;
        if (availableAnims.Contains(newState))
        {
            animator.Play(currentState);
            //Debug.Log(currentState);
        }
    }

    public void stateDetect()
    {
        if (stateCondition["run"])
        {
            changeState(STATE_RUN);
        }
        if (stateCondition["jump"])
        {
            changeState(STATE_JUMP);
        }
        if (stateCondition["fall"])
        {
            /*if (currentVelocity.y <= -15)
            {
                if (currentVelocity.x <= -0.5f)
                {
                    Debug.Log("fall left");
                    changeState(STATE_FALL_LEFT);
                }
                if (currentVelocity.x >= 0.5f)
                {
                    Debug.Log("fall right");
                    changeState(STATE_FALL_RIGHT);
                }

                }*/
            changeState(STATE_FALL);
        }



        if (getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy)
        {
            //print("land");
            changeState(STATE_LAND);

        }


        //if (stateCondition["parkour_roll"])
        //{
        //    print("parkour_roll");
        //}
        if (stateCondition["idle"])
        {
            changeState(STATE_IDLE);
        }

    }

    public void flipPlayerSprite(sbyte facingDir)
    {
        bool shouldFlip = false;
        if (facingDir == 0) { return; }
        if (facingDir == 1) { shouldFlip = false; }
        else if (facingDir == -1) { shouldFlip = true; }

        spriteRenderer.flipX = shouldFlip;

    }

    public Vector2 getLandVelocity()
    {
        previousIsGrounded = currentIsGrounded;
        //currentIsGrounded = playerGroundTriggerScript.isGrounded;

        if (previousIsGrounded == false && currentIsGrounded == true) 
        {
            //print($"previousVelocity{previousVelocity}");
            return previousVelocity;
        }
        else
        {
            return Vector2.zero; 
        }
    }

    public void setCanChangeState(string state)
    {
        if (state == "land_start") { canChangeState = false; }
        else if (state == "land_end") { canChangeState = true; }

    }
    public void refreshVelocityInfo()
    {
        previousVelocity = currentVelocity;
        currentVelocity = shadeRB.velocity;
    }

    public void refreshStateCondition()
    {

        stateCondition = new Dictionary<string, bool>
        {
            {"run",  Mathf.Abs(currentVelocity.x) > 0.05f /*&& playerGroundTriggerScript.isGrounded == true*/},//{"run",  playerControlScript.isMoving == true && playerGroundTriggerScript.isGrounded == true}�H�e��run�P�O��
            {"jump", followingShade.isJumping == true},
            {"fall", currentVelocity.y <= -0.0001f/* && playerGroundTriggerScript.isGrounded == false*/},
            //{"land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy},
            //{"hard_land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy},
            //{"parkour_roll", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) > hardLandMinVy && Mathf.Abs(currentVelocity.x) > parkourRollMinVx},
            {"idle", Mathf.Abs(currentVelocity.x) <= 0.05f /*&& playerGroundTriggerScript.isGrounded == true*/ && followingShade.isMoving == false && followingShade.isJumping == false}
        };
    }
}
