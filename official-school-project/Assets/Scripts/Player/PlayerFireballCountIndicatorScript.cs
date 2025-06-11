using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerFireballCountIndicatorScript : MonoBehaviour
{
    public static PlayerFireballCountIndicatorScript instance {  get; private set; }

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


    [Header("Basic Rotate Settings")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateRadius;
    private float currentRotateAngle = 0f;

    [Header("Rotate Patterns")]
    [SerializeField] private Vector3[] rotatePatterns;
    private bool patternChanged = false;
    private int currentPattern = 0;

    [Header("Display")]
    [SerializeField] private GameObject indicatorSprite;


    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = rotatePatterns[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LogicScript.instance.isFreeze())
        {
            rotateMain();

            indicatorSprite.transform.position = (Vector2)transform.GetChild(0).position;
            
        }

        transform.position = PlayerControlScript.instance.transform.position;

    }

    private void Update()
    {
        transform.position = PlayerControlScript.instance.transform.position;
    }

    private void rotateMain()
    {
        //rotate
        currentRotateAngle += rotateSpeed * Time.fixedDeltaTime;
        //reset angle
        if(currentRotateAngle > 360)
        {
            while (currentRotateAngle > 360) currentRotateAngle -= 360;
            patternChanged = false;
        }


        //change pattern
        if(!patternChanged && currentRotateAngle > 90 + rotatePatterns[currentPattern].y)
        {
            patternChanged = true;
            currentPattern++;
            if (currentPattern >= rotatePatterns.Length) currentPattern = 0;

            transform.eulerAngles = rotatePatterns[currentPattern];

            currentRotateAngle = 90 + rotatePatterns[currentPattern].y;
        }

        //set layer
        if(currentRotateAngle >= 0 + rotatePatterns[currentPattern].y && currentRotateAngle < 180 + rotatePatterns[currentPattern].y)
        {
            setSpriteLayer(-1);
        }
        else
        {
            setSpriteLayer(5);
        }

        //set child state
        transform.GetChild(0).localPosition = new Vector3(rotateRadius * Mathf.Cos(currentRotateAngle * Mathf.Deg2Rad), rotateRadius * Mathf.Sin(currentRotateAngle * Mathf.Deg2Rad), 0);
    }

    private void setSpriteLayer(int layer)
    {
        //child 0 is the rotating object
        //GameObject child = transform.GetChild(0).gameObject;
        //child.GetComponent<SpriteRenderer>().sortingOrder = layer;
        //child.GetComponent<TrailRenderer>().sortingOrder = layer;

        indicatorSprite.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    public void setIndicatorActiveState(bool state)
    {
        //transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = state;
        //transform.GetChild(0).GetComponent<TrailRenderer>().enabled = state;
        //transform.GetChild(0).GetComponent<Light2D>().enabled = state;
        indicatorSprite.SetActive(state);
    }
}
