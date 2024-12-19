using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
	//variables
	#region variable
	//reference
	private Rigidbody2D rb;

	//physics
	private float myGravityScale;
	private float myGravityMaxSpeed; // max fall speed


	#endregion



	// Start is called before the first frame update
	void Start()
    {
        //init
		rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

	#region physic function

	private void physicMain()
	{

	}

	/*private void myAcceleration()*/

	#endregion

}
