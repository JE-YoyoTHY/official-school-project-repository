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
        Sparrow, Frog, Breeze, Cicada
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
    }

    private void OnEnable()
    {
        _initDict();
    }

}
