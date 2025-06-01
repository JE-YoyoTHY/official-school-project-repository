using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDeathSoulScript : MonoBehaviour
{   
    private bool isMoveUpward;
    private float moveTimeCounter;

    [Header("Move Upward (for death)")]
    [SerializeField] private AnimationCurve moveUpCurve; // negative for left, positive for right
    [SerializeField] private float moveUpXRange;
    [SerializeField] private float moveUpTime;
    [SerializeField] private float upwardSpeed;
    private float moveUpMidPoint; // record the x pos it spawns // for death

    [Header("Move downward (for respawn)")]
    [SerializeField] private float moveDownTime;
    [SerializeField] private float moveDownSpanwY; // above player's respawn point
    private float targetYPos; // record the y position it needs to arrive // for respawn

    

    [Header("Sprite")]
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite tailDown;
    [SerializeField] private Sprite tailLeft;
    [SerializeField] private Sprite tailRight;


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!LogicScript.instance.isFreeze())
        {
            if (isMoveUpward)
            {
                moveUpwardMain();
            }
            else
            {
                moveDownwardMain();
            }
        }

    }

    public void summon(bool upward)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveUpMidPoint = PlayerControlScript.instance.transform.position.x;

        isMoveUpward = upward;

        if (!isMoveUpward)
        {
            targetYPos = PlayerControlScript.instance.transform.position.y;
            transform.position += Vector3.up * moveDownSpanwY;
            moveTimeCounter = moveDownTime;
        }
    }

    //private void moveStart()
    //{

    //}

    private void moveEnd()
    {
        if (!isMoveUpward)
        {
            PlayerAnims.instance.GetComponent<SpriteRenderer>().enabled = true;
        }

        Destroy(gameObject);
    }

    private void moveUpwardMain()
    {

        // x dir
        float x = moveUpCurve.Evaluate(moveTimeCounter / moveUpTime) * moveUpXRange;
        transform.position = new Vector3(x + moveUpMidPoint, transform.position.y, transform.position.z);

        float m = moveUpCurve.Evaluate(moveTimeCounter + Time.deltaTime / moveUpTime) - moveUpCurve.Evaluate(moveTimeCounter / moveUpTime);  
        if(m > 0)
        {
            spriteRenderer.sprite = tailLeft;
        }
        else
        {
            spriteRenderer.sprite = tailRight;
        }

        //y dir
        transform.position += Vector3.up * upwardSpeed * Time.fixedDeltaTime;

        moveTimeCounter += Time.fixedDeltaTime;

        if(moveTimeCounter > moveUpTime)
        {
            moveEnd();
        }
    }

    private void moveDownwardMain()
    {
        float x = moveUpCurve.Evaluate(moveTimeCounter / moveDownTime) * moveUpXRange;
        transform.position = new Vector3(x + moveUpMidPoint, transform.position.y, transform.position.z);

        float m = moveUpCurve.Evaluate(moveTimeCounter - Time.deltaTime / moveDownTime) - moveUpCurve.Evaluate(moveTimeCounter / moveDownTime);
        if (m > 0)
        {
            spriteRenderer.sprite = tailLeft;
        }
        else
        {
            spriteRenderer.sprite = tailRight;
        }

        //y dir
        //transform.position += Vector3.down * moveDownSpeed * Time.fixedDeltaTime;
        float y = Mathf.Lerp(targetYPos + moveDownSpanwY, targetYPos, 1 - (moveTimeCounter / moveDownTime));
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        moveTimeCounter -= Time.fixedDeltaTime;

        if (moveTimeCounter < 0)
        {
            moveEnd();
        }
    }
}
