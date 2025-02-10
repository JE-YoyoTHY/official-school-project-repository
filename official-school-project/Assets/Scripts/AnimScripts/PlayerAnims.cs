using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private GameObject player;
    private PlayerControlScript playerControlScript;
    private PlayerGroundTriggerScript playerGroundTriggerScript;

    #region States
    // states, 同時也是動畫的名稱
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
    [SerializeField] private float hardLandMinVy = 10f;  // 要符合HardLand所需的最小Y速度
    [SerializeField] private float parkourRollMinVx = 10f;  // 要符合HardLand同時又要有最小X速度才會翻滾
    private bool previousIsGrounded;
    private bool currentIsGrounded;
    private sbyte facingDir;
    private Dictionary<string, bool> stateCondition = new Dictionary<string, bool>();
    void Start()
    {
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerControlScript = player.GetComponent<PlayerControlScript>();
        playerGroundTriggerScript = player.GetComponentInChildren<PlayerGroundTriggerScript>(false);  // false 代表只偵測active的子物件
        playerRb2D = playerControlScript.gameObject.GetComponent<Rigidbody2D>();
        currentVelocity = playerRb2D.velocity;
    }

    void Update()
    {
        transform.position = GameObject.Find("Player").transform.position;
        facingDir = playerControlScript.moveKeyValue;
        flipPlayerSprite(facingDir);


        refreshStateCondition();
        stateDetect();
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
        if (stateCondition["hard_land"])
        {
            print("hard_land");
        }
        if (stateCondition["land"])
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

        if (previousIsGrounded == false && currentIsGrounded == true)  // 代表剛落地
        {
            print(previousVelocity);
            return previousVelocity;
        }
        else
        {
            return Vector2.zero;  // 代表沒有落地
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
            {"run",  playerControlScript.isMoving == true && playerGroundTriggerScript.isGrounded == true},
            {"jump", playerControlScript.isJumping == true},
            {"fall", currentVelocity.y <= 0 && playerGroundTriggerScript.isGrounded == false},
            {"land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) <= hardLandMinVy},
            {"hard_land", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) > hardLandMinVy},
            {"parkour_roll", getLandVelocity() != Vector2.zero && Mathf.Abs(getLandVelocity().y) > hardLandMinVy && Mathf.Abs(currentVelocity.x) > parkourRollMinVx},
            {"idle", playerGroundTriggerScript.isGrounded == true && playerControlScript.isMoving == false && playerControlScript.isJumping == false}
        };
    }
}
