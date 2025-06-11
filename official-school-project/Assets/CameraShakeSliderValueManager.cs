using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraShakeSliderValueManager : MonoBehaviour
{
    private static CameraShakeSliderValueManager instance;
    [SerializeField] private SoundDataBase soundData;
    [SerializeField] private Slider _slider;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void sliderVolumeChanged()
    {
        float newValue = instance._slider.value;
        instance.soundData.cameraShakeDegree = newValue;

    }
    public static void setSliderValue(float _value)
    {
        instance._slider.value = _value;
    }
}
