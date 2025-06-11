using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private static SFXManager instance;
    [SerializeField] private SoundDataBase soundData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider _slider;


    private void Awake()
    {        
        instance = this;
        audioSource = GetComponent<AudioSource>();


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void playSFXOneShot(SoundDataBase.SFXType sfxType, float _volume = 0.4f)
    {
        instance.audioSource.PlayOneShot(instance.soundData.SFXClipDict[sfxType], _volume);
    }

    public static void sliderVolumeChanged()
    {
        print("value changed");
        float newValue = instance.audioSource.volume;
        print(newValue);

        instance.soundData.sfxVolume = newValue;

    }

    public static void setSliderValue(float _value)
    {
        instance._slider.value = _value;
    }



}


