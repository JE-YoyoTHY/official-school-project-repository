using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    [SerializeField] private SoundDataBase soundData;
    [SerializeField] private AudioSource audioSource;


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
        instance.audioSource.Play();
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
}
