using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecorationCloudSpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private DecorationCloud cloudPrefab;
    public List<Sprite> cloudImages = new List<Sprite>();

    [Header("Spawner")]
    [SerializeField] private float cloudSpawnRateMin;  // in sec
    [SerializeField] private float cloudSpawnRateMax;
    [SerializeField] private float cloudSpawnYMin;
    [SerializeField] private float cloudSpawnYMax;
    [SerializeField] public bool canSpawn = false;

    private Transform spawnerTransform;
    private List<string> sortingLayerNames;
    public float cloudSpeedMultiplier = 1;  // can set to 0 to stop the cloud


    private void Awake()
    {
        spawnerTransform = transform;
        sortingLayerNames = new List<string>()
        {
            "MainMenu_Decor_Middle",
            "MainMenu_Decor_Front"
        };
    }

    void Start()
    {
        StartCoroutine(SpawnClouds());
    }

    void Update()
    {

    }

    public void spawnCloud()
    {
        Vector3 spawnPosition = getRandomSpawnPos();
        DecorationCloud cloudScript = Instantiate(cloudPrefab, getRandomSpawnPos(), new Quaternion(0, 0, 0, 0), spawnerTransform);
        GameObject cloudObj = cloudScript.gameObject;
        cloudObj.GetComponent<SpriteRenderer>().sprite = cloudImages[Random.Range(0, cloudImages.Count)];
        cloudObj.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[Random.Range(0, sortingLayerNames.Count)];
    }

    public Vector3 getRandomSpawnPos()
    {
        Vector3 spawnPos = new Vector3(gameObject.transform.position.x, Random.Range(cloudSpawnYMin, cloudSpawnYMax), 0);
        return spawnPos;
    }
    
    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (canSpawn)
            {
                spawnCloud();
                yield return new WaitForSeconds(Random.Range(cloudSpawnRateMin, cloudSpawnRateMax));
            }
            else
            {
                yield return null;
            }
            
        }
    }
}
