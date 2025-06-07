using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private static SFXManager instance;
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

    public static void playSFXOneShot(SoundDataBase.SFXType sfxType, float _volume = 1.0f)
    {
        instance.audioSource.PlayOneShot(instance.soundData.SFXClipDict[sfxType], _volume);
    }




}


