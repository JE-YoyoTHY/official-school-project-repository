using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

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
        SingleHandler  // 只需要一個就好, 例如ESC
    }
    
    private Vector2 originalTMPPosition;
    private float mainMenuButtonDisplacement = 50.0f;
    private float mainMenuButtonScale = 1.1f;

    [Header("General (Might be used multiple times)")]
    // buttons
    [SerializeField] private ButtonTypes whichButton;  // inspector
    [SerializeField] private DecorationManager decorManager;
    [SerializeField] UIIndicators selectionTriangle;
    [SerializeField] private RebindSystemDataBase rebindSystemDataBase;

    // targets
    [Header("Rebind Switch")]


    [Header("Open / Close Setting Button")]
    [SerializeField] private GameObject settingTab;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject openSettingButton;
    [SerializeField] private GameObject openCreditButton;

    [Header("Start Game Button")]
    [SerializeField] MainMenuManager mainMenuManager;


    private void Awake()
    {
        if (transform.childCount > 0 && transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>() != null)
        {
            originalTMPPosition = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().transform.position;
        }

    }

    void Start()
    {
        if (whichButton == ButtonTypes._None) { Debug.LogError("ButtonType 未設置"); }
    }

    // Update is called once per frame
    void Update()
    {
        if (whichButton == ButtonTypes.SingleHandler)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                print("esc pressed");
                if (rebindSystemDataBase.isRebinding == false)
                {
                    if (settingTab.activeSelf == false)
                    {
                        // want to open
                        if (startGameButton != null)  // at main menu
                        {
                            startGameButton.SetActive(false);
                            openSettingButton.SetActive(false);
                            openCreditButton.SetActive(false);
                            selectionTriangle.gameObject.SetActive(false);
                        }
                        openSettingButton.SetActive(false);
                        print("set setting button false");
                    }
                    else
                    {
                        // want to close setting
                        if (startGameButton != null)
                        {
                            startGameButton.SetActive(true);
                            openSettingButton.SetActive(true);
                            openCreditButton.SetActive(true);
                            startGameButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
                            openSettingButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
                            openCreditButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
                        }
                        openSettingButton.SetActive(true);
                        print("set setting button true");
                    }
                    settingTab.SetActive(!settingTab.activeSelf);
                    SFXManager.playSFXOneShot(SoundDataBase.SFXType.CloseSetting);
                }
            }
        }

    }

    public void buttonOnClick()
    {

        if (whichButton == ButtonTypes.CloseSettingButton)
        {
            SFXManager.playSFXOneShot(SoundDataBase.SFXType.CloseSetting);
            print("setting sfx");
            if (startGameButton != null) // at main menu
            {
                startGameButton.SetActive(true);
                startGameButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
                openSettingButton.SetActive(true);
                openCreditButton.SetActive(true);
                openCreditButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
            }
            settingTab.SetActive(false);
            openSettingButton.SetActive(true);
            openSettingButton.GetComponent<ButtonManager>().TMPBackToOriginalInstantly();
            print("BACK TO ORIGINAL");

        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            
            if (startGameButton != null)
            {   
                TMPBackToOriginalInstantly();
                selectionTriangle.disappear();
                startGameButton.SetActive(false);
                openCreditButton.SetActive(false);
            }
            openSettingButton.SetActive(false);
            settingTab.SetActive(true);
        }
        else if (whichButton == ButtonTypes.StartGameButton)
        {
            selectionTriangle.disappear();
            TMPBackToOriginalInstantly();
            mainMenuManager.startGame();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            selectionTriangle.gameObject.SetActive(true);
            selectionTriangle.showSelectionTriangle(UIIndicators.UIIndicatorReferencesEnum.playSelection);
            decorManager.performDecorationColorize();
            pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            if (startGameButton != null)
            {
                selectionTriangle.gameObject.SetActive(true);
                selectionTriangle.showSelectionTriangle(UIIndicators.UIIndicatorReferencesEnum.settingSelection);
                pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
            }
        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {
            selectionTriangle.gameObject.SetActive(true);
            selectionTriangle.showSelectionTriangle(UIIndicators.UIIndicatorReferencesEnum.creditSelection);
            pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            selectionTriangle.disappear();
            decorManager.performDecorationToGrayGradually();
            pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {
            if (selectionTriangle != null)
            {
                selectionTriangle.disappear();
                pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
            }
            
        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {
            selectionTriangle.disappear();
            pointerEnterEffect_SlideAndGrowText(new Vector2(mainMenuButtonDisplacement, 0), mainMenuButtonScale, true);
        }
    }

    public void TMPBackToOriginalInstantly()
    {
        // 文字
        transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().transform.position = originalTMPPosition;
        transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().transform.DOScale(1, 0);
    }
    private void pointerEnterEffect_SlideAndGrowText(Vector2 displacement, float growScale, bool isReverse = false, float duration = 0.2f, Ease easeType = Ease.Linear)
    {
        GameObject targetText = transform.GetChild(0).transform.gameObject;
        RectTransform textTransform = targetText.GetComponent<RectTransform>();
        if (isReverse == false)
        {
            Vector2 destination = originalTMPPosition + displacement;
            textTransform.DOMove(destination, duration).SetEase(easeType);
            textTransform.DOScale(growScale, duration).SetEase(easeType);
        }

        if (isReverse == true)
        {
            Vector2 destination = originalTMPPosition;
            textTransform.DOMove(destination, duration).SetEase(easeType);
            textTransform.DOScale(1, duration).SetEase(easeType);
        }
    }

}
