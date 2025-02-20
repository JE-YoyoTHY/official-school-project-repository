using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformanceSystemScript : MonoBehaviour
{
	#region Singleton

	public static PlayerPerformanceSystemScript instance { get; private set; }
	private void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	#endregion

	private PlayerPerformanceTriggerScript currentController;
	public bool isBeingControl { get; private set; }


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentController != null && !LogicScript.instance.isFreeze())
		{
			currentController.controlMain();
		}
    }

	private void controlStart(PlayerPerformanceTriggerScript newTrigger)
	{
		isBeingControl = true;
		currentController = newTrigger;

		PlayerControlScript.instance.performanceStart();

	}

	private void controlEnd()
	{
		isBeingControl = false;
		currentController = null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("PerformanceTrigger"))
		{
			PlayerPerformanceTriggerScript newTrigger = collision.GetComponent<PlayerPerformanceTriggerScript>();

			if (currentController == null)
			{
				if (newTrigger.performanceTriggerInspectorObject.isStartPoint)
				{
					newTrigger.enableTrigger();
					controlStart(newTrigger);
				}
			}

			if (currentController != null)
			{
				if(newTrigger == currentController.performanceTriggerInspectorObject.nextTrigger)
				{
					newTrigger.enableTrigger();
					currentController.disableTrigger();
					currentController = newTrigger;
				}

				if (currentController.performanceTriggerInspectorObject.isEndPoint)
				{
					controlEnd();
				}
			}
		}
	}
}
