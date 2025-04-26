using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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

    // Start is called before the first frame update
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
            mainMenuManager.startGame();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            decorManager.performDecorationColorize();
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {

        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {

        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (whichButton == ButtonTypes.StartGameButton)
        {
            decorManager.performDecorationToGrayGradually();
        }
        else if (whichButton == ButtonTypes.OpenSettingButton)
        {

        }
        else if (whichButton == ButtonTypes.OpenCreditButton)
        {

        }
    }

}
