using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public enum MusicType
    {
        _NULL, 
        MainMenu, 
        Chapter1, Chapter2
    }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<MusicType> musicTypeList;
    [SerializeField] private List<AudioClip> musicClipList;
    public Dictionary<MusicType, AudioClip> musicClipDict = new Dictionary<MusicType, AudioClip>();

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();

        if (musicTypeList.Count != musicClipList.Count) { Debug.LogError("音樂片段數量與音樂type數量不同"); }
        if (musicTypeList.Count <= musicClipList.Count)
        {
            for (int i = 0; i < musicTypeList.Count; i++)
            {
                musicClipDict.Add(musicTypeList[i], musicClipList[i]);
            }
        }
        else
        {
            for (int i = 0; i < musicClipList.Count; i++)
            {
                musicClipDict.Add(musicTypeList[i], musicClipList[i]);
            }
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void playMusic(MusicType musicType, float _volume = 1f)
    {
        instance.audioSource.Play();
    }

    public static void changeCurrentClip(MusicType musicType)
    {
        AudioClip newClip = instance.musicClipDict[musicType];
        instance.audioSource.clip = newClip;
    }

    public static AudioClip getCurrentClip()
    {
        return instance.audioSource.clip;
    }

    public static MusicType getCurrentMusicType()
    {
        AudioClip m_clip = instance.audioSource.clip;
        foreach(MusicType musicType in instance.musicClipDict.Keys)
        {
            if (instance.musicClipDict[musicType] == m_clip)
            {
                return musicType;
            }
        }
        return MusicType._NULL;
    }
}
