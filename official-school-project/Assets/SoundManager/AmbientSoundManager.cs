using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AmbientSoundManager: MonoBehaviour
{
    private static AmbientSoundManager instance;
    private static float previousVolume;
    [SerializeField] private SoundDataBase soundData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<GameObject> tempGameObjs;
    [SerializeField] private Slider _slider;



    private void Awake()
    {        
        instance = this;
        audioSource = GetComponent<AudioSource>();
        previousVolume = audioSource.volume;
        sliderVolumeChanged();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void playAmbientSoundOneShot(SoundDataBase.AmbientSoundType ambientType, float _volume = 1.0f)
    {
        float finalVolume = _volume * instance.audioSource.volume;
        instance.audioSource.PlayOneShot(instance.soundData.ambientSoundClipDict[ambientType], _volume);
    }

    public static void playAmbientSoundLoop(SoundDataBase.AmbientSoundType ambientType, float _volume = 1, int _priority = 128)
    {
        // _volume must be 1 to avoid some issues (not bug)
        GameObject tempGameObj = new GameObject("temp_ambientSound_" + ambientType.ToString());
        tempGameObj.transform.parent = instance.transform;
        instance.tempGameObjs.Add(tempGameObj);
        AudioSource _source = tempGameObj.AddComponent<AudioSource>();
        AudioClip _clip = instance.soundData.ambientSoundClipDict[ambientType];
        _source.clip = _clip;
        _source.volume = _volume * instance.audioSource.volume;
        _source.priority = _priority;
        _source.loop = true;
        _source.Play();
    }

    /* DO NOT USE OTHERWISE BUDS WILL OCCUR !
    public static void stopSpecificAmbientSound(SoundDataBase.AmbientSoundType targetAmbientType)
    {
        List<GameObject> shouldBeRemovedGameObjs = new List<GameObject>();
        foreach (GameObject obj in instance.tempGameObjs)
        {
            foreach (SoundDataBase.AmbientSoundType _ambientType in instance.soundData.ambientSoundClipDict.Keys)
            {
                if (instance.soundData.ambientSoundClipDict[targetAmbientType] == obj.GetComponent<AudioSource>().clip && targetAmbientType == _ambientType)
                {
                    shouldBeRemovedGameObjs.Add(obj);
                    Destroy(obj);
                }
            }
        }

    }
    */

    public static void stopAllAmbientSound()
    {
        foreach (GameObject obj in instance.tempGameObjs)
        {
            Destroy(obj);
        }
        instance.tempGameObjs.Clear();
    }

    public static void pauseAllAmbientSound()
    {
        foreach (GameObject obj in instance.tempGameObjs)
        {
            obj.GetComponent<AudioSource>().Pause();
        }
    }

    public static void unpauseAllAmbientSound()
    {
        foreach (GameObject obj in instance.tempGameObjs)
        {
            obj.GetComponent<AudioSource>().UnPause();
        }
    }

    public static void sliderVolumeChanged()
    {
        print("value changed");
        float newValue = instance.audioSource.volume;
        print(newValue);

        foreach (GameObject obj in instance.tempGameObjs)
        {
            print(newValue / previousVolume);
            float finalVolume = instance.audioSource.volume;
            obj.GetComponent<AudioSource>().volume = finalVolume;
        }
        instance.GetComponent<AudioSource>().volume = newValue;
        instance.soundData.ambientSoundVolume = newValue;
        previousVolume = newValue;

    }

    public static void setSliderValue(float _value)
    {
        instance._slider.value = _value;
    }




}


