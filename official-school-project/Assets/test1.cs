using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            MusicManager.playMusic(SoundDataBase.MusicType.Chapter1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MusicManager.playMusic(SoundDataBase.MusicType.Chapter2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            MusicManager.playMusic(SoundDataBase.MusicType.Chapter3);

        }

    }
}
