using System.Collections;
using UnityEngine;

public class BlackEdge : MonoBehaviour
{
    public enum EdgeType { Top, Bottom }
    [SerializeField] private EdgeType edgeType;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float outerOffsetY;
    [SerializeField] private float innerOffsetY;

    [SerializeField] private float slideDuration = 0.5f;
    private float currentOffsetY;

    void Start()
    {
        // 預設 cameraTransform
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // 起始位置（畫面外）
        currentOffsetY = outerOffsetY * (edgeType == EdgeType.Top ? 1 : -1);
    }

    void FixedUpdate()
    {
        Vector3 camPos = cameraTransform.position;
        transform.position = new Vector3(camPos.x, camPos.y + currentOffsetY, -1f);
    }

    public void StartSlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(innerOffsetY * (edgeType == EdgeType.Top ? 1 : -1)));
    }

    public void StartSlideOut()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(outerOffsetY * (edgeType == EdgeType.Top ? 1 : -1)));
    }

    private IEnumerator SlideTo(float targetOffsetY)
    {
        float startOffsetY = currentOffsetY;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            currentOffsetY = Mathf.Lerp(startOffsetY, targetOffsetY, t);
            yield return null;
        }

        currentOffsetY = targetOffsetY;
    }
}
