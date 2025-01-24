using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeCrystalScript : MonoBehaviour
{
	private LogicScript logic;
	private PlayerControlScript player;
	private SpriteRenderer sprite;

	[SerializeField] private float cooldownDuration;
	private float cooldownCounter;
	private bool isPowerActive;
	[SerializeField] private Color activeColor;
	[SerializeField] private Color deactiveColor;


    // Start is called before the first frame update
    void Start()
    {
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

	public void regainPower()
	{
		cooldownCounter = 0;
		isPowerActive = true;
		sprite.color = activeColor;
	}

	private void losePower()
	{
		cooldownCounter = cooldownDuration;
		isPowerActive = false;
		sprite.color = deactiveColor;
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
