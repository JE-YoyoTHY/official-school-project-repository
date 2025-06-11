using System.Collections;
using UnityEngine;

public class BlackEdge : MonoBehaviour
{
    public enum EdgeType { Top, Bottom }
    [SerializeField] private EdgeType edgeType;

    [SerializeField] private Transform cameraTransform;

    private RectTransform rect;

    private float outerOffsetY = 0;
    private float innerOffsetY = 135;

    [SerializeField] private float slideDuration = 0.5f;
    private float currentOffsetY;
    private float size;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        currentOffsetY = outerOffsetY * (edgeType == EdgeType.Top ? 1 : -1);
        //Vector3 anchoredPosition = cameraTransform.position;

        rect = GetComponent<RectTransform>();

        transform.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        rect.anchoredPosition = new Vector3(0, 140*(edgeType == EdgeType.Top ? 1 : -1), 0);
    }

    void FixedUpdate()
    {
        rect.anchoredPosition = new Vector3(0, 140*(edgeType == EdgeType.Top ? 1 : -1) - currentOffsetY, 0);
        //transform.position = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y + 135*(edgeType == EdgeType.Top ? 1 : -1), -5f);
    }

    public void StartSlideIn()
    {
        size = Camera.main.GetComponent<Camera>().orthographicSize;
        StopAllCoroutines();
        StartCoroutine(SlideTo(innerOffsetY/10* size* (edgeType == EdgeType.Top ? 1 : -1)));
    }

    public void StartSlideOut()
    {
        size = Camera.main.GetComponent<Camera>().orthographicSize;
        StopAllCoroutines();
        StartCoroutine(SlideTo(outerOffsetY/10*size * (edgeType == EdgeType.Top ? 1 : -1)));
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
