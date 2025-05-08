using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonTypes
    {
        _None,
        StartGameButton,
        RebindSwitch,
        OpenSettingButton,
        CloseSettingButton, 
        OpenCreditButton, 
        CloseCreditButton,
    }
    
    private Vector2 originalPosition;
    private float mainMenuButtonDisplacement = 50.0f;
    private float mainMenuButtonScale = 1.1f;

    [Header("General (Might be used multiple times)")]
    // buttons
    [SerializeField] private ButtonTypes whichButton;  // inspector
    [SerializeField] private DecorationManager decorManager;

    // targets
    [Header("Rebind Switch")]
    [SerializeField] private GameObject rebindCanvas;

    [Header("Open / Close Setting Button")]
    [SerializeField] private GameObject settingTab;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject openSettingButton;
    [SerializeField] private GameObject openCreditButton;

    [Header("Start Game Button")]
    [SerializeField] MainMenuManager mainMenuManager;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    void Start()
    {
        if (whichButton == ButtonTypes._None) { Debug.LogError("ButtonType ¥¼³]¸m"); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonOnClick()
    {
        if (whichButton == ButtonTypes.RebindSwitch)
        {
            rebindCanvas.SetActive(!rebindCanvas.activeSelf);
        }
        else if (whichButton == ButtonTypes.CloseSettingButton)
        {
            if (startGameButton != null)
            {
                startGameButton.SetActive(true);
                openSettingButton.SetActive(true);
                openCreditButton.SetActive(true);
            }
            settingTab.SetActive(false);

        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            backToOriginalInstantly();
            if (startGameButton != null)
            {
                startGameButton.SetActive(false);
                openSettingButton.SetActive(false);
                openCreditButton.SetActive(false);
            }
            settingTab.SetActive(true);
        }
        else if (whichButton == ButtonTypes.StartGameButton)
        {
            backToOriginalInstantly();
            mainMenuManager.startGame();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            decorManager.performDecorationColorize();
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            decorManager.performDecorationToGrayGradually();
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {
            pointerEnterEffect_SlideAndGrow(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
        }
    }

    private void backToOriginalInstantly()
    {
        transform.position = originalPosition;
        transform.DOScale(1, 0);
    }
    private void pointerEnterEffect_SlideAndGrow(Vector2 displacement, float growScale, bool isReverse = false, float duration = 0.2f, Ease easeType = Ease.Linear)
    {
        if (isReverse == false)
        {
            Vector2 destination = originalPosition + displacement;
            transform.DOMove(destination, duration).SetEase(easeType);
            transform.DOScale(growScale, duration).SetEase(easeType);
        }

        if (isReverse == true)
        {
            Vector2 destination = originalPosition;
            transform.DOMove(destination, duration).SetEase(easeType);
            transform.DOScale(1, duration).SetEase(easeType);
        }
    }

}
