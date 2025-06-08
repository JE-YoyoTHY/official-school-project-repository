using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientSoundManager: MonoBehaviour
{
    private static AmbientSoundManager instance;
    [SerializeField] private SoundDataBase soundData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<GameObject> tempGameObjs;


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

    public static void playAmbientSoundOneShot(SoundDataBase.AmbientSoundType ambientType, float _volume = 1.0f)
    {
        instance.audioSource.PlayOneShot(instance.soundData.ambientSoundClipDict[ambientType], _volume);
    }

    public static void playAmbientSound(SoundDataBase.AmbientSoundType ambientType, float _volume = 1, int _priority = 128)
    {
        GameObject tempGameObj = new GameObject("temp_ambientSound_" + ambientType.ToString());
        instance.tempGameObjs.Add(tempGameObj);
        AudioSource _source = tempGameObj.AddComponent<AudioSource>();
        AudioClip _clip = instance.soundData.ambientSoundClipDict[ambientType];
        _source.clip = _clip;
        _source.volume = _volume;
        _source.priority = _priority;
        _source.Play();
    }

    public static void stopAllAmbientSound()
    {
        foreach (GameObject obj in instance.tempGameObjs)
        {
            instance.tempGameObjs.Clear();
            Destroy(obj);
        }
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




}


