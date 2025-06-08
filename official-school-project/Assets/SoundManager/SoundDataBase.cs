using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDataBase", menuName = "CustomScriptableObject/SoundDataBase")]
public class SoundDataBase : ScriptableObject
{
    public enum SFXType
    {
        GrassStep, RockStep, RockStep1, RockStep2,
        Jump,
        Land,
        ShootFireball, FireballExplode, TriggeredRocketJump,
        SheepStepped,
        CloseSetting, 
        RockGrassStep1, RockGrassStep2, 
        OptionHover, 
    }
    [SerializeField] private List<SFXType> SFXTypeList;
    [SerializeField] private List<AudioClip> SFXClipList;
    public Dictionary<SFXType, AudioClip> SFXClipDict = new Dictionary<SFXType, AudioClip>();

    public enum MusicType
    {
        _NULL,
        MainMenu,
        Chapter1, Chapter2
    }
    [SerializeField] private List<MusicType> musicTypeList;
    [SerializeField] private List<AudioClip> musicClipList;
    public Dictionary<MusicType, AudioClip> musicClipDict = new Dictionary<MusicType, AudioClip>();

    public enum AmbientSoundType
    {
        _NULL, 
        Sparrow, Frog, Breeze, Cicada, Earthquake

    }
    [SerializeField] private List<AmbientSoundType> ambientSoundTypeList;
    [SerializeField] private List<AudioClip> ambientSoundClipList;
    public Dictionary<AmbientSoundType, AudioClip> ambientSoundClipDict = new Dictionary<AmbientSoundType, AudioClip>();



    private void _initDict()
    {
        // SFX
        if (SFXTypeList.Count != SFXClipList.Count) { Debug.LogError("琿计秖籔type计秖ぃ"); }
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

        // Background Music
        if (musicTypeList.Count != musicClipList.Count) { Debug.LogError("贾琿计秖籔贾type计秖ぃ"); }
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

        // Ambient Sound
        if (ambientSoundTypeList.Count != ambientSoundClipList.Count) { Debug.LogError("贾琿计秖籔贾type计秖ぃ"); }
        if (ambientSoundTypeList.Count <= ambientSoundClipList.Count)
        {
            for (int i = 0; i < ambientSoundTypeList.Count; i++)
            {
                ambientSoundClipDict.Add(ambientSoundTypeList[i], ambientSoundClipList[i]);
            }
        }
        else
        {
            for (int i = 0; i < ambientSoundClipList.Count; i++)
            {
                ambientSoundClipDict.Add(ambientSoundTypeList[i], ambientSoundClipList[i]);
            }
        }
    }

    private void OnEnable()
    {
        _initDict();
    }

}
