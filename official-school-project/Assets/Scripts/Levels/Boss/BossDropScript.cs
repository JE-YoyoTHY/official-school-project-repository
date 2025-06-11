using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BossDropScript : MonoBehaviour
{
    [SerializeField] private GameObject movingSprite;

    [Header("Physic value")]
    [SerializeField] private float dropDuration;
    [SerializeField] private float vy0;
    [SerializeField] private float myGravity;
    [SerializeField] private GameObject targetPos; // only use for x

    [Header("Rotation")]
    [SerializeField] private AnimationCurve rotateCurve;
    [SerializeField] private Vector3 finalRotation;

    private float dropTimeCounter;
    private Vector3 initialPos;

    private bool isDropActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isDropActive)
            myDropMain();
    }

    private void myDropMain()
    {
        dropTimeCounter += Time.fixedDeltaTime;

        //rotation
        movingSprite.transform.localEulerAngles = finalRotation * rotateCurve.Evaluate(dropTimeCounter / dropDuration);

        //x pos
        movingSprite.transform.position = 
            new Vector3(Mathf.Lerp(initialPos.x, targetPos.transform.position.x, dropTimeCounter / dropDuration), 
            movingSprite.transform.position.y, 
            movingSprite.transform.position.z);

        //y pos
        movingSprite.transform.position =
            new Vector3(movingSprite.transform.position.x,
            initialPos.y + vy0 * dropTimeCounter - (1.0f/2.0f) * myGravity * Mathf.Pow(dropTimeCounter, 2),
            movingSprite.transform.position.z);
    }

    public void myDropStart()
    {
        dropTimeCounter = 0;
        initialPos = transform.position;
        movingSprite.transform.position = transform.position;
        movingSprite.GetComponent<SpriteRenderer>().sortingOrder = -10;

        isDropActive = true;
    }

    public void myDropEnd()
    {
        isDropActive = false;
        movingSprite.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

}
