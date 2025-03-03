using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TransitionBlackHole : MonoBehaviour
{
    private RectTransform holeRect;
    private RectTransform canvasRect;

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
        /* 要測試的時候可以打開
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
            UIPerforming.setUIPosWithWorldPos(canvasRect, holeRect, new Vector3(20, 20, 0));
        }
        */
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
