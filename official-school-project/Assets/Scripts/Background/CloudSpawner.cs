using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//by GPT, 還沒修過

//by ooflemon
/* 每隔一段時間會生成一次雲(outer cycle
 * 每一次生成雲會生成不特定的數量(inner cycle
 * 生成多個雲的時候，每一朵雲會有自己的間隔
 * back to outer cycle
 */

//現在有bug

public class CloudSpawner : MonoBehaviour
{
	//ooflemon rewrite the code
	/*public GameObject cloudPrefab;  // 連結你的雲朵Prefab
    public float spawnInterval = 2f; // 產生間隔
    public float cloudSpeed = 1f; // 雲朵移動速度
    public float minY = -2f, maxY = 2f; // 雲朵 Y 軸範圍

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            SpawnCloud();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCloud()
    {
        float cameraRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        float randomY = Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(cameraRightEdge + 1f, randomY, 0);
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

        cloud.GetComponent<Rigidbody2D>().velocity = new Vector2(-cloudSpeed, 0);
        Destroy(cloud, 10f); // 10秒後刪除，防止物件堆積
    }*/


	//variables
	private RectTransform rect;

	[SerializeField] private bool isSpawner = false; // if true, spawner, else -> moving prefab


	[SerializeField] private GameObject cloudPrefab;

	[SerializeField] private float spawnRateMin, spawnRateMax; //outer cycle
	[SerializeField] private int spawnAmountMin, spawnAmountMax;
	[SerializeField] private float innerCycleIntervalMin, innerCycleIntervalMax; // inner cycle interval
	[SerializeField] private float YRange;
	private float spawnRateCounter = 0, spawnAmountCounter = 0, innerCycleTimeCounter = 0;

	private GameObject prefabInstance;
	[SerializeField] private float prefabMoveSpeedMin, prefabMoveSpeedMax;
	private float moveSpeed;
	//[SerializeField] private Object[] images;
	[SerializeField] private GameObject spawnPos;

	[SerializeField] private Sprite[] imgs;


	private void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (!LogicScript.instance.isFreeze())
		{
			if (isSpawner)
				spawnMain();
			else moveMain();
		}
	}

	private void spawnMain()
	{
		if(spawnRateCounter <= 0) // outer cycle
		{
			if(spawnAmountCounter <= 0) //inner cycle
			{
				innerCycleStart();
			}
			else
			{
				if(innerCycleTimeCounter <= 0)
				{
					spawnPrefab();
				}
				else
				{
					innerCycleTimeCounter -= Time.deltaTime;
				}
			}
		}
		else
		{
			spawnRateCounter -= Time.deltaTime;
		}
	}

	private void innerCycleStart()
	{
		spawnAmountCounter = Random.Range(spawnAmountMin, spawnAmountMax);
	}

	private void spawnPrefab()
	{
		//Vector3 pos = rect.position;
		//pos.y = pos.y + Random.Range(-YRange, YRange);

		//spawn
		prefabInstance = Instantiate(cloudPrefab, transform.parent);
		prefabInstance.GetComponent<CloudSpawner>().prefabInit(Random.Range(prefabMoveSpeedMin, prefabMoveSpeedMax), Random.Range(-YRange, YRange), spawnPos);

		//inner cycle
		spawnAmountCounter--;
		if(spawnAmountCounter > 0)
		{
			innerCycleTimeCounter = Random.Range(innerCycleIntervalMin, innerCycleIntervalMax);
		}
		else
		{
			spawnRateCounter = Random.Range(spawnRateMin, spawnRateMax);
		}
	}


	#region prefab

	private void moveMain()
	{
		rect.position = rect.position - new Vector3(moveSpeed * Time.deltaTime, 0, 0);

		//print(rect.anchoredPosition);
		
		if (rect.anchoredPosition.x < -3000)
		{
			Destroy(gameObject);
		}
	}

	public void prefabInit(float speed, float y, GameObject pos)
	{
		moveSpeed = speed * Time.deltaTime;
		rect = GetComponent<RectTransform>();
		//rect.position = new Vector3(1000f, y, 0f);
		rect.anchoredPosition = pos.GetComponent<RectTransform>().anchoredPosition;
		rect.anchoredPosition = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y + y, 0);

		//GetComponent<Image>().sourceImage = imgs[Random.Range(0, imgs.Length)];
		//print(rect.position);
	}

	#endregion

}
