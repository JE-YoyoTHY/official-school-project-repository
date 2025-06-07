using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SpringScript : MonoBehaviour
{
	private Vector2 pushDir;
	private CinemachineImpulseSource impulseSource;
	private Animator animator;

	[Header("Common Settings")]
	[SerializeField] private float pushForce = 30;
	[SerializeField] private float fireballSpeedScale = 2;
	[SerializeField] private ScreenShakeProfile screenShakeProfile;
	[SerializeField] private float fireballLessTime;
	[SerializeField] private VFXManager m_VFXManager;

	[Header("Remove Ability")]
	[SerializeField] private bool removePlayerMoveAbility = false;
	[SerializeField] private float springDuration = 0.3f;
	[SerializeField] private float springGravityScale = 20;
	[SerializeField] private float springFriction = 20;
	

	[Header("Fireball Settings")]
	[SerializeField] private Vector2 hitPlayerVelocity;

	[Header("Trajectory Preview")]
	[SerializeField] private float pointTimeInterval = 0.05f;
	
	private Vector2[] segments;
	private LineRenderer lineRenderer;

	private void Awake()
	{
		//push dir
		//Vector3 deltaPos = transform.GetChild(0).position - transform.position;
		//pushDir = new Vector2(deltaPos.x, deltaPos.y).normalized;


		//trajectory
		lineRenderer = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();

		int _segCount = (int)(springDuration / pointTimeInterval);// print(_segCount);
		segments = new Vector2[_segCount];
		lineRenderer.positionCount = segments.Length;
	}

	// Start is called before the first frame update
	void Start()
    {
		Vector3 deltaPos = transform.GetChild(0).position - transform.position;
		pushDir = new Vector2(deltaPos.x, deltaPos.y).normalized;

		lineRenderer.enabled = false;

		impulseSource = GetComponent<CinemachineImpulseSource>();
	}

    void Update()
    {
		//lineRenderer.enabled = true;

		Vector3 deltaPos = transform.GetChild(0).position - transform.position;
		pushDir = new Vector2(deltaPos.x, deltaPos.y).normalized;
		if (removePlayerMoveAbility)
			drawTrajectory();
		else lineRenderer.positionCount = 0;

		//Debug.DrawRay(transform.position, Vector2.down * transform.localScale.y * 0.5f, Color.green);
	}

	/*private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			springPlayerTrigger();
		}

		if(collision.gameObject.tag == "Fireball")
		{
			//Vector2 localDir = transform.GetChild(0).localPosition;
			Vector3 deltaPos = transform.GetChild(0).position - transform.position;
			Vector2 localDir = new Vector2(deltaPos.x, deltaPos.y).normalized;
			collision.gameObject.GetComponent<FireballScript>().springPush(localDir.normalized, fireballSpeedScale);
		}
	}*/

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			springPlayerTrigger();
		}

		if (collision.gameObject.tag == "Fireball")
		{
			//Vector2 localDir = transform.GetChild(0).localPosition;
			//Vector3 deltaPos = transform.GetChild(0).position - transform.position;
			//Vector2 localDir = new Vector2(deltaPos.x, deltaPos.y).normalized;
			FireballScript fb = collision.gameObject.GetComponent<FireballScript>();
			if (fb.triggerState() == fireballTriggerCollidingObject.spring)
				collision.gameObject.GetComponent<FireballScript>().springPush(pushDir.normalized, fireballSpeedScale, hitPlayerVelocity);
		}

		if (collision.gameObject.tag == "TutorialShade")
		{
			TutorialShadeScript shade = collision.gameObject.GetComponent<TutorialShadeScript>();

			//player position, teleport player if player's y position is lower or greater than spring's height, only happen when take player's control
			Vector3 playerPos = shade.transform.position;
			if (removePlayerMoveAbility)
			{
				if (playerPos.y < transform.position.y + transform.localScale.y * -0.5f) shade.transform.position = new Vector3(playerPos.x, transform.position.y + transform.localScale.y * -0.5f, playerPos.z);
				if (playerPos.y > transform.position.y + transform.localScale.y * 0.5f) shade.transform.position = new Vector3(playerPos.x, transform.position.y + transform.localScale.y * 0.5f, playerPos.z);
			}

			//player push
			shade.springPush(pushDir.normalized * pushForce, removePlayerMoveAbility, springDuration, springGravityScale, springFriction);
		}
	}

	private void springPlayerTrigger()
	{
		//screen shake
		CameraShakeManagerScript.instance.cameraShakeWithProfileWithGivenDirection(screenShakeProfile, impulseSource, pushDir);

		//player position, teleport player if player's y position is lower or greater than spring's height, only happen when take player's control
		Vector3 playerPos = PlayerControlScript.instance.transform.position;
		if (removePlayerMoveAbility)
		{
			if (playerPos.y < transform.position.y + transform.localScale.y * -0.5f) PlayerControlScript.instance.transform.position = new Vector3(playerPos.x, transform.position.y + transform.localScale.y * -0.5f, playerPos.z);
			if (playerPos.y > transform.position.y + transform.localScale.y * 0.5f) PlayerControlScript.instance.transform.position = new Vector3(playerPos.x, transform.position.y + transform.localScale.y * 0.5f, playerPos.z);
		}
        m_VFXManager.performVFX();
        animator.Play("SheepBounce");
		SFXManager.playSFXOneShot(SoundDataBase.SFXType.SheepStepped);
		//player push
		PlayerControlScript.instance.springPush(pushDir.normalized * pushForce, removePlayerMoveAbility, springDuration, springGravityScale, springFriction, fireballLessTime);
	}

	/*private void springFireballTrigger()
	{
		Vector2 localDir = transform.GetChild(0).localPosition;

	}*/


	#region debug function

	private void drawTrajectory()
	{
		//lineRenderer.enabled = true;

		int _segCount = (int)(springDuration / pointTimeInterval);
		segments = new Vector2[_segCount];
		lineRenderer.positionCount = segments.Length;

		Vector2 startPos = transform.position, 
			initialVelocity = pushDir * pushForce, 
			localAcceleration = new Vector2(springFriction, -springGravityScale);
		if (pushDir.x > 0) localAcceleration.x *= -1;
		segments[0] = startPos;
		lineRenderer.SetPosition(0, startPos);

		for(int i = 1; i < segments.Length; i++)
		{
			// X = x0 + v0t + (1/2)a(t^2), delta t = point time interval * i
			segments[i] = startPos + 
				initialVelocity * pointTimeInterval * i + 
				localAcceleration * Mathf.Pow(pointTimeInterval * i, 2) * 0.5f;
			lineRenderer.SetPosition(i, segments[i]);
		}

	}

	#endregion
}
