using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private static SFXManager instance;
    public enum SFXType
    {
        FootStep_Grass, FootStep_Rock, 
        Jump, 
        Land, 
        ShootFireball, FireballExplode, TriggeredRocketJump, 
        SheepStepped
    }
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<SFXType> SFXTypeList;
    [SerializeField] private List<AudioClip> SFXClipList;
    public Dictionary<SFXType, AudioClip> SFXClipDict = new Dictionary<SFXType, AudioClip>();

    private void Awake()
    {        
        instance = this;
        audioSource = GetComponent<AudioSource>();

        if (SFXTypeList.Count != SFXClipList.Count) { Debug.LogError("音效片段數量與音效type數量不同"); }
        if (SFXTypeList.Count <= SFXClipList.Count)
        {
            for (int i = 0; i < SFXTypeList.Count; i++)
            {
                SFXClipDict.Add(SFXTypeList[i], SFXClipList[i]);
            }
        }
        else
        {
            for (int i = 0; i < SFXClipList.Count; i++)
            {
                SFXClipDict.Add(SFXTypeList[i], SFXClipList[i]);
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

    public static void playSFXOneShot(SFXType sfxType, float _volume = 1.0f)
    {
        if (_volume > 1.0f) _volume = 1.0f;
        else if (_volume < 0.0f) _volume = 0.0f;
        instance.audioSource.PlayOneShot(instance.SFXClipDict[sfxType], _volume);
    }




}


