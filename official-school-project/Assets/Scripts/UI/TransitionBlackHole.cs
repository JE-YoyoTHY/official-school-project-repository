using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;

public class TransitionBlackHole : MonoBehaviour
{
    public RectTransform holeRect {  get; private set; }
    private RectTransform canvasRect;

    [SerializeField] private float openDuration = 1.5f;
    [SerializeField] private float openRadius = 3000;
    [SerializeField] private Ease openEaseType = Ease.OutQuint;
    public UnityEvent openHoleFinished; 
    [SerializeField] private float closeDuration = 0.25f;
    [SerializeField] private float closeRadius = 0;
    [SerializeField] private Ease closeEaseType = Ease.OutQuint;
    public UnityEvent closeHoleFinished;

    



    void Start()
    {
        holeRect = GetComponent<RectTransform>();
        canvasRect = holeRect.root.GetComponent<Canvas>().GetComponent<RectTransform>();
        holeRect.sizeDelta = new Vector2(openRadius, openRadius);

        gameObject.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        
    }



    #region FUNCTION: openHole()
    public void openHole()
    {
        float duration = openDuration;
        Ease easeType = openEaseType;
        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(openRadius, openRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, openDuration).SetEase((Ease)easeType).onComplete = openCallBack;
    }

    public void openHole(float duration, Ease easeType)
    {
        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(openRadius, openRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, openDuration).SetEase((Ease)easeType).onComplete = openCallBack;
    }
    #endregion

    #region FUNCTION: closeHole()
    public void closeHole()
    {
        float duration = closeDuration;
        Ease easeType = closeEaseType;

        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(closeRadius, closeRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, closeDuration).SetEase((Ease)easeType).onComplete = closeCallBack;
    }
    public void closeHole(float duration, Ease easeType)
    {
        Vector2 holeSize = holeRect.sizeDelta;
        Vector2 finalSize = new Vector2(closeRadius, closeRadius);

        DOTween.To(() => holeRect.sizeDelta, x => holeRect.sizeDelta = x, finalSize, closeDuration).SetEase((Ease)easeType).onComplete = closeCallBack;
    }
    #endregion

    public void openCallBack()
    {
        openHoleFinished.Invoke();
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void closeCallBack()
    {
        closeHoleFinished.Invoke();
    }










}

#if UNITY_EDITOR

[CustomEditor(typeof(TransitionBlackHole))]
public class TransitionBlackHoleCustomInspector : Editor
{

    TransitionBlackHole transitionBlackHole;

    private void OnEnable()
    {
        transitionBlackHole = (TransitionBlackHole)target; 
    }

    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        

        if (GUILayout.Button("Open Hole", GUILayout.Width(180f)))
        {
            transitionBlackHole.openHole();
        }
        if (GUILayout.Button("Close Hole", GUILayout.Width(180f)))
        {
            transitionBlackHole.closeHole();
        }
    }
}

#endif