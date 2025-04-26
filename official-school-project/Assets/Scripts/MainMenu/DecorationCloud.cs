using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationCloud : MonoBehaviour
{
    [Header("Inspector Setting")]
    [SerializeField] private DecorationCloudSpawner decorCloudSpawner;
    [SerializeField] private Material grayToColoredMAT;
    [SerializeField] private Vector2 cloudMoveDir;
    [SerializeField] private float cloudSpeedMin;
    [SerializeField] private float cloudSpeedMax;
    [SerializeField] private float cloudDeadLeftX;
    [SerializeField] private float cloudDeadRightX;

    private float cloudSpeed;
    private float cloudSpeedMultiplier;  // can set to 0 to stop the cloud

    private void Awake()
    {
        if (decorCloudSpawner == null)
        {
            decorCloudSpawner = GameObject.Find("DecorationCloudSpawner").GetComponent<DecorationCloudSpawner>();
        }
        GetComponent<SpriteRenderer>().sharedMaterial = grayToColoredMAT;
        cloudSpeed = Random.Range(cloudSpeedMin, cloudSpeedMax);
        cloudSpeedMultiplier = decorCloudSpawner.cloudSpeedMultiplier;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cloudDeadCheck();
        cloudMove();
        cloudSpeedMultiplier = decorCloudSpawner.cloudSpeedMultiplier;
    }

    public void cloudMove()
    {
        Vector3 addPos = cloudMoveDir * cloudSpeed * cloudSpeedMultiplier * Time.deltaTime;
        transform.position += addPos;
    }

    public void killCloud()
    {
        Destroy(gameObject);
    }

    public void cloudDeadCheck()
    {
        if (cloudMoveDir.x > 0)
        {
            // spawner is at left
            if (gameObject.transform.position.x > cloudDeadRightX)
            {
                killCloud();
            }
        }
        else if (cloudMoveDir.x < 0)
        {
            // spawner is at right
            if (gameObject.transform.position.x < cloudDeadLeftX)
            {
                killCloud();
            }
        }
    }
}
