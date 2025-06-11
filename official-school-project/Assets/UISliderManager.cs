using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISliderManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    #region SLIDER STRUCTER
    // Slider
    // - Background
    // - Fill Area
    // - - Fill
    // - Handle Slide Area
    // - - Handle
    #endregion

    public static GameObject currentDragingSlider;
    public enum UISliderType
    {
        Music,
        SoundEffect,
        CameraShake, 
        AmbientSound
    }

    [Header("Fill In")]
    [SerializeField] private UISliderType currentSliderType;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource ambientSoundAudioSource;

    [SerializeField] private GameObject percentageDisplay;
    

    [Header("Percentage Display")]
    [SerializeField] private float displayAboveHandleHeight;

    [Header("Read")]
    [SerializeField] private GameObject handle;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private bool isHandleBeingPressed;
    [SerializeField] private bool isHandleBeingHovered;
    private void Awake()
    {
        handle = gameObject.GetComponent<Slider>().handleRect.gameObject;
        isHandleBeingPressed = false;
        isHandleBeingHovered = false;

        if (displayAboveHandleHeight == 0)
        {
            displayAboveHandleHeight = 90;
        }

        if (percentageDisplay != null)
        {
            // TO DO this func won't execute
            percentageText = percentageDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            percentageDisplay.SetActive(false);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        sliderPosSticking();
        percentageDisplayAppearOrDisappear();
    }

    public void onSliderValueChanged()
    {
        changePercentageDisplay();
        changeOriginalValue();
    }

    public void changeSliderValue(float newValue)
    {
        gameObject.GetComponent<Slider>().value = newValue;
    }

    private void changePercentageDisplay()
    {
        if (percentageDisplay != null)
        {
            float sliderValue = gameObject.GetComponent<Slider>().value;
            float sliderPercentage = sliderValue * 100;
            sliderPercentage = Mathf.Floor(sliderPercentage);
            print($"slider percentage {sliderPercentage}");
            if (percentageText == null)
            {
                print("percentageText is null");
            }
            if (percentageText.text == null) print("this text is null");
            print($"percentage's text {percentageText.text}");

            percentageText.text = sliderPercentage.ToString() + "%";
        }
    }

    public void changeOriginalValue()
    {
        if (currentSliderType == UISliderType.Music)
        {
            musicAudioSource.volume = getCurrentSliderValue();
        }
        else if (currentSliderType == UISliderType.SoundEffect)
        {
            sfxAudioSource.volume = getCurrentSliderValue();
        }
        else if (currentSliderType == UISliderType.AmbientSound)
        {
            ambientSoundAudioSource.volume = getCurrentSliderValue();
        }
    }

    public float getCurrentSliderValue()
    {
        return gameObject.GetComponent<Slider>().value;
    }

    private void sliderPosSticking()
    {
        percentageDisplay.transform.position = handle.transform.position + new Vector3(0, displayAboveHandleHeight, 0);
    }

    public void percentageDisplayAppearOrDisappear()
    {
        if (isHandleBeingHovered == true || isHandleBeingPressed == true)
        {
            percentageDisplay.gameObject.SetActive(true);
        }
        else
        {
            percentageDisplay.gameObject.SetActive(false);
        }
    }


    #region Handle Being Hovered / Pressed
    public void onPointerEnterHandle()
    {
        isHandleBeingHovered = true;
    }
    public void onPointerExitHandle()
    {
        isHandleBeingHovered = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentDragingSlider != null && currentDragingSlider != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
            currentDragingSlider = gameObject;
        }
        isHandleBeingPressed = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isHandleBeingPressed = false;
    }

    #endregion


}
