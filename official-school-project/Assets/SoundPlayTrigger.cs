using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayTrigger : MonoBehaviour
{
    [SerializeField] private bool isPlay;  // true = start play; false = end plat
    [SerializeField] private SoundDataBase.AmbientSoundType currentAmbientSoundType;
    private bool hasPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (hasPlayed == false && isPlay == true)
            {
                AmbientSoundManager.playAmbientSoundLoop(currentAmbientSoundType, 0.5f);
            }
            else if (isPlay == false)
            {
                AmbientSoundManager.stopAllAmbientSound();

            }
        }

    }
}
