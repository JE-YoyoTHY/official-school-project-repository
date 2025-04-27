using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class RechargeCrystalScript : MonoBehaviour
{
	private Animator animator;
	private Light2D light;
	//private LogicScript logic;
	//private PlayerControlScript player;
	//private SpriteRenderer sprite;

	[SerializeField] private float cooldownDuration;
	private float cooldownCounter;
	private bool isPowerActive;
	//[SerializeField] private Color activeColor;
	//[SerializeField] private Color deactiveColor;

	//shake
	private Vector3 defaultPos;
	[SerializeField] private float shakeCooldown; //the interval of changing position
	[SerializeField] private float shakeDistance; // the max shake distance from default
	private Coroutine shakeCoroutine;

	//screen shake
	[SerializeField] private ScreenShakeProfile screenShakeProfile;
	private CinemachineImpulseSource impulseSource;


    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
		light = GetComponent<Light2D>();
		//logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
		//player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>();
		//sprite = GetComponent<SpriteRenderer>();
		impulseSource = GetComponent<CinemachineImpulseSource>();


		regainPower();
		defaultPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        powerMain();

		if (shakeCoroutine == null) StartCoroutine(shake());
    }

	private void powerMain()
	{
		if(!isPowerActive)
		{
			if(!LogicScript.instance.isFreeze()) cooldownCounter -= Time.deltaTime;
			if(cooldownCounter < 0 ) regainPower();
		}
	}

	private IEnumerator shake()
	{
		//random pos
		float randomAngle = Random.Range(0, 360f);
		float randomDistance = Random.Range(0f, shakeDistance);

		transform.position = defaultPos + new Vector3(randomDistance * Mathf.Cos(Mathf.Deg2Rad * randomAngle), randomDistance * Mathf.Sin(Mathf.Deg2Rad * randomAngle), 0f);

		//wait for second
		float t = shakeCooldown;
		while (t > 0)
		{
			if(!LogicScript.instance.isFreeze())
				t -= Time.deltaTime;
			
			yield return null;
		}
	}

    [ContextMenu("Regain Power")]
    public void regainPower()
	{
		cooldownCounter = 0;
		isPowerActive = true;
		//sprite.color = activeColor;
		animator.Play("ChargedFlameFlicker");
		light.enabled = true;
	}

	//[Button]
	[ContextMenu("Lose Power")]
	public void losePower()
	{
		cooldownCounter = cooldownDuration;
		isPowerActive = false;
		//sprite.color = deactiveColor;
        animator.Play("UnchargedFlameFlicker");

		//screen shake
		CameraShakeManagerScript.instance.cameraShakeWithProfileWithRandomDirection(screenShakeProfile, impulseSource);

		//particle
		GetComponent<ParticleCommonScript>().emitParticle();

		//light
		light.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player" && isPowerActive && PlayerControlScript.instance.fireballChargeNeeded())
		{
			losePower();
			PlayerControlScript.instance.fireballChargeGain(3);
		}
	}
}
