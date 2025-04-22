using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class ZeusBlockScript : MonoBehaviour
{
	private Rigidbody2D rb;

	public ZeusBlockType blockType;

	[Header("Falling Block")]
	[SerializeField] private float explodeSpeed;
	[SerializeField] private float explodeGravity;
	[SerializeField] private float maxFallSpeed;
	private Vector2 explodeDir;


	[Header("Rotate")]
	[SerializeField] private float myAngularVelocity;
	[SerializeField] private int rotateDir;
	[SerializeField] private float endAngle;
	private bool isRotating; // positive for counter clockwise


	[Header("Debug")]
	[SerializeField] private int segmentCount;
	[SerializeField] private float segmentInterval;
	private Vector2[] segments;
	private LineRenderer lineRenderer;


	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		lineRenderer = GetComponent<LineRenderer>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (blockType == ZeusBlockType.fallingBlock)
		{
			explodeDir = (transform.GetChild(0).position - transform.position).normalized;
			drawTrajectory();
		}
			
    }

	private void FixedUpdate()
	{
		if (blockType == ZeusBlockType.fallingBlock)
		{
			gravityMain();
		}

		if (blockType == ZeusBlockType.rotatingBlock && isRotating)
		{
			rotateMain();
		}
	}

	#region fall

	//call this to start
	public void explodeStart()
	{
		rb.velocity = explodeDir * explodeSpeed;
	}


	private void gravityMain()
	{
		rb.velocity = rb.velocity + Vector2.down * Time.fixedDeltaTime * explodeGravity;
		if(rb.velocity.y < Mathf.Abs(maxFallSpeed) * -1)
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(maxFallSpeed) * -1);
		}
	}


	#endregion

	#region rotate

	public void startRotate()
	{
		isRotating = true;
	}

	private void rotateMain()
	{
		transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 0, 1) * rotateDir * myAngularVelocity * Time.fixedDeltaTime;
		if (rotateDir > 0 && transform.localEulerAngles.z > endAngle)
		{
			transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 0, endAngle);
			isRotating = false;
		}
		else if(rotateDir < 0 && transform.localEulerAngles.z < endAngle)
		{
			transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 0, endAngle);
			isRotating = false;
		}
	}

	#endregion

	#region debug

	private void drawTrajectory()
	{
		segments = new Vector2[segmentCount];
		lineRenderer.positionCount = segments.Length;

		Vector2 startPos = transform.position;
		Vector2 initialVelocity = explodeDir * explodeSpeed;
		Vector2 localAcceleration = Vector2.down * explodeGravity;

		segments[0] = startPos;
		lineRenderer.SetPosition(0, startPos);

		bool overMaxFallSpeed = false;
		int j = 0;
		for (int i = 1; i < segments.Length; i++)
		{
			// X = x0 + v0t + (1/2)a(t^2), delta t = point time interval * i

			if (initialVelocity.y + localAcceleration.y * segmentInterval * i >= Mathf.Abs(maxFallSpeed) * -1)
			{
				segments[i] = startPos +
				initialVelocity * segmentInterval * i +
				localAcceleration * Mathf.Pow(segmentInterval * i, 2) * 0.5f;

			}
			else
			{
				if (!overMaxFallSpeed)
				{
					overMaxFallSpeed = true;
					j = i - 1;
					startPos = startPos +
						initialVelocity * segmentInterval * j +
						localAcceleration * Mathf.Pow(segmentInterval * j, 2) * 0.5f;
					initialVelocity.y = Mathf.Abs(maxFallSpeed) * -1;
				}

				segments[i] = startPos + initialVelocity * (i - j) * segmentInterval;
			}
			lineRenderer.SetPosition(i, segments[i]);
		}
	}

	#endregion

}

public enum ZeusBlockType
{
	fallingBlock,
	rotatingBlock
}
