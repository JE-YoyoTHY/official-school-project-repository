using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RechargeCrystalScript : MonoBehaviour
{
	private Animator animator;
	private LogicScript logic;
	private PlayerControlScript player;
	private SpriteRenderer sprite;

	[SerializeField] private float cooldownDuration;
	private float cooldownCounter;
	private bool isPowerActive;
	//[SerializeField] private Color activeColor;
	//[SerializeField] private Color deactiveColor;


    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
		logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();
		sprite = GetComponent<SpriteRenderer>();

		regainPower();
    }

    // Update is called once per frame
    void Update()
    {
        powerMain();
    }

	private void powerMain()
	{
		if(!isPowerActive)
		{
			if(!logic.isFreeze()) cooldownCounter -= Time.deltaTime;
			if(cooldownCounter < 0 ) regainPower();
		}
	}

    [ContextMenu("Regain Power")]
    public void regainPower()
	{
		cooldownCounter = 0;
		isPowerActive = true;
		//sprite.color = activeColor;
		animator.Play("ChargedFlameFlicker");
	}

	//[Button]
	[ContextMenu("Lose Power")]
	public void losePower()
	{
		cooldownCounter = cooldownDuration;
		isPowerActive = false;
		//sprite.color = deactiveColor;
        animator.Play("UnchargedFlameFlicker");
    }

    private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player" && isPowerActive && player.fireballChargeNeeded())
		{
			losePower();
			player.fireballChargeGain(3);
		}
	}
}
