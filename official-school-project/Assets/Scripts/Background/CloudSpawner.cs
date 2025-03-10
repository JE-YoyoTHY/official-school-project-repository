using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public int cloudCount = 10;
    public float minY = -3f, maxY = 3f;
    public float minXOffset = -5f, maxXOffset = 5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        SpawnClouds();
    }

    void SpawnClouds()
    {
        for (int i = 0; i < cloudCount; i++)
        {
            Vector2 spawnPosition = GetRandomPositionInView();
            Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomPositionInView()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float randomX = Random.Range(-camWidth / 2, camWidth / 2) + mainCamera.transform.position.x;
        float randomY = Random.Range(minY, maxY) + mainCamera.transform.position.y;

        return new Vector2(randomX, randomY);
    }
}
