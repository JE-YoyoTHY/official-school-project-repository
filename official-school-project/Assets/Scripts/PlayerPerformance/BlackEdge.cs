using System.Collections;
using UnityEngine;

public class BlackEdge : MonoBehaviour
{
    public enum EdgeType { Top, Bottom }
    [SerializeField] private EdgeType edgeType;

    [SerializeField] private Transform cameraTransform;

    private float outerOffsetY = 13;
    private float innerOffsetY = 10;

    [SerializeField] private float slideDuration = 0.5f;
    private float currentOffsetY;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        currentOffsetY = outerOffsetY * (edgeType == EdgeType.Top ? 1 : -1);
    }

    void FixedUpdate()
    {
        Vector3 cameraPosition = cameraTransform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y + currentOffsetY, -5f);
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
