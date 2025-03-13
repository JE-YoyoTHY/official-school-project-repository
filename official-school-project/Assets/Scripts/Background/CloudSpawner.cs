using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//by GPT, 還沒修過
public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;  // 連結你的雲朵Prefab
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
    }
}
