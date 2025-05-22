using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
    public static PlayerAnims instance { get; private set; }

    private void Awake(){
        if (instance != null && instance != this) {
            Destroy(this);
        }
        else {
            instance = this;
        }
    }


    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private GameObject player;
    private PlayerControlScript playerControlScript;
    private PlayerGroundTriggerScript playerGroundTriggerScript;

    #region States
    // states, �P�ɤ]�O�ʵe���W��
    public string currentState;
    public string STATE_IDLE { get; private set; } = "PlayerIdle";
    public string STATE_RUN { get; private set; } = "PlayerRun";
    public string STATE_JUMP { get; private set; } = "PlayerJump";
    public string STATE_FALL { get; private set; } = "PlayerFall";
    public string STATE_LAND { get; private set; } = "PlayerLand";
    public string STATE_HARD_LAND { get; private set; } = "PlayerHardLand";
    public string STATE_PARKOUR_ROLL { get; private set; } = "PlayerParkourRoll";

    private List<string> availableAnims = new List<string>()
    {
        "PlayerRun", "PlayerIdle", "PlayerJump", "PlayerFall", "PlayerLand"
    };
    #endregion

    public bool canChangeState { get; private set; } = true;
    private Rigidbody2D playerRb2D;
    [SerializeField] private Vector2 previousVelocity;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float hardLandMinVy = 10f;  // �n�ŦXHardLand�һݪ��̤pY�t��
    [SerializeField] private float parkourRollMinVx = 10f;  // �n�ŦXHardLand�P�ɤS�n���̤pX�t�פ~�|½�u
    [SerializeField] private bool previousIsGrounded;
    [SerializeField] private bool currentIsGrounded;
    private sbyte facingDir;
    private Dictionary<string, bool> stateCondition = new Dictionary<string, bool>();
    void Start()
    {
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerControlScript = player.GetComponent<PlayerControlScript>();
        playerGroundTriggerScript = player.GetComponentInChildren<PlayerGroundTriggerScript>(false);  // false �N���u����active���l����
        playerRb2D = playerControlScript.gameObject.GetComponent<Rigidbody2D>();
        currentVelocity = playerRb2D.velocity;
        refreshStateCondition();
    }

    void Update()
    {
        //transform.position = GameObject.Find("Player").transform.position;
        //Vector3 playerPos = GameObject.Find("Player").transform.position;
        Vector3 playerPos = PlayerControlScript.instance.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y+0.5f, transform.position.z);
        //facingDir = playerControlScript.moveKeyValue;
        facingDir = PlayerPerformanceSystemScript.instance.getPlayerFacingDir();
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
            changeState(STATE_FALL);
        }


        //print($"getLandVelocity(): {getLandVelocity()}");
        //print($"is vector2.zero: {getLandVelocity() == Vector2.zero}");
        if (getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) > hardLandMinVy)
        {
            print("hard_land");
        }

        if (getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy)
        {
            print("land");
            changeState(STATE_LAND);
        }


        if (stateCondition["parkour_roll"])
        {
            print("parkour_roll");
        }
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
        currentIsGrounded = playerGroundTriggerScript.isGrounded;

        if (previousIsGrounded == false && currentIsGrounded == true)  // �N���踨�a
        {
            //print($"previousVelocity{previousVelocity}");
            return previousVelocity;
        }
        else
        {
            return Vector2.zero;  // �N���S�����a
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
        currentVelocity = playerRb2D.velocity;
    }

    public void refreshStateCondition()
    {

        stateCondition = new Dictionary<string, bool>
        {
            {"run",  Mathf.Abs(currentVelocity.x) > 0.05f && playerGroundTriggerScript.isGrounded == true},//{"run",  playerControlScript.isMoving == true && playerGroundTriggerScript.isGrounded == true}以前的run判別式
            {"jump", playerControlScript.isJumping == true},
            {"fall", currentVelocity.y <= 0 && playerGroundTriggerScript.isGrounded == false},
            {"land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy},
            {"hard_land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy},
            {"parkour_roll", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) > hardLandMinVy && Mathf.Abs(currentVelocity.x) > parkourRollMinVx},
            {"idle", Mathf.Abs(currentVelocity.x) <= 0.05f && playerGroundTriggerScript.isGrounded == true && playerControlScript.isMoving == false && playerControlScript.isJumping == false}
        };
    }
}
