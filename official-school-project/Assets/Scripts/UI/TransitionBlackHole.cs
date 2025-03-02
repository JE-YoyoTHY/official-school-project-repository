using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform holeRect;
    private RectTransform canvasRect;
    public Transform squareTransform;

    [SerializeField] private float openDuration;
    [SerializeField] private float openRadius;
    [SerializeField] private Ease openEaseType;
    public UnityEvent openFinished; 
    [SerializeField] private float closeDuration;
    [SerializeField] private float closeRadius;
    [SerializeField] private Ease closeEaseType;
    public UnityEvent closeFinished;



    void Start()
    {
        holeRect = GetComponent<RectTransform>();
        canvasRect = holeRect.root.GetComponent<Canvas>().GetComponent<RectTransform>();

        openRadius = 3000;
        openDuration = 1.5f;
        openEaseType = Ease.OutQuint;

        closeRadius = 0;
        closeDuration = 1.5f;
        closeEaseType = Ease.OutQuint;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            openHole();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            closeHole();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            setHolePosWithWorldPos(GameObject.Find("Square").transform.position);
        }
    }

    
    public Vector2 getUIPosFromWorldPos(Vector3 worldPos)
    {
        Vector2 UIPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Camera.main.WorldToScreenPoint(worldPos),
            Camera.main,
            out UIPos
        );
        return UIPos;

    }

    public Vector3 getWorldPosFromRectTransform(RectTransform rect)
    {
        Vector3 worldPos = rect.position;
        return worldPos;
    }

    public void setHolePosWithWorldPos(Vector3 worldPos)
    {
        holeRect.anchoredPosition = getUIPosFromWorldPos(worldPos);
    }


    public void openHole(float? duration = null, Ease? easeType = null)
    {
        duration ??= openDuration;  // 如果沒有duration參數傳入則使用預設值
        easeType ??= openEaseType;
        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(openRadius, openRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, openDuration).SetEase((Ease)easeType).onComplete = openCallBack;
    }

    public void closeHole(float? duration = null, Ease? easeType = null)
    {
        duration ??= closeDuration;
        easeType ??= closeEaseType;
        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(closeRadius, closeRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, closeDuration).SetEase((Ease)easeType).onComplete = closeCallBack;
    }

    public void openCallBack()
    {
        openFinished.Invoke();
    }

    public void closeCallBack()
    {
        closeFinished.Invoke();
    }










}
