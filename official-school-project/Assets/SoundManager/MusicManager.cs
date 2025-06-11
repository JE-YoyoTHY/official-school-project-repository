using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
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

    public static void playMusic(SoundDataBase.MusicType musicType, float _volume = 1f)
    {
        changeCurrentClip(musicType);
        instance.audioSource.Play();
    }
    public static void pauseMusic()
    {
        instance.audioSource.Pause();
    }
    public static void unpauseMusic()
    {
        instance.audioSource.UnPause();
    }

    public static void endMusic()
    {
        instance.audioSource.Stop();
    }

    public static void changeCurrentClip(SoundDataBase.MusicType musicType)
    {
        AudioClip newClip = instance.soundData.musicClipDict[musicType];
        instance.audioSource.clip = newClip;
    }

    public static AudioClip getCurrentClip()
    {
        return instance.audioSource.clip;
    }

    public static SoundDataBase.MusicType getCurrentMusicType()
    {
        AudioClip currentClip = instance.audioSource.clip;
        foreach(SoundDataBase.MusicType musicType in instance.soundData.musicClipDict.Keys)
        {
            if (instance.soundData.musicClipDict[musicType] == currentClip)
            {
                return musicType;
            }
        }
        return SoundDataBase.MusicType._NULL;
    }

    public static void sliderVolumeChanged()
    {
        print("value changed");
        float newValue = instance.audioSource.volume;
        print(newValue);

        instance.soundData.musicVolume = newValue;

    }

    public static void setSliderValue(float _value)
    {
        if (instance._slider == null) print("slider is null");
        instance._slider.value = _value;
    }
}
