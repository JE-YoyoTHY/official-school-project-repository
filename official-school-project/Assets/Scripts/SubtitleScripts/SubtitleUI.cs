using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    public string textContent = string.Empty;
    public TextMeshProUGUI subtitleText;
    public float letterRate;
    public float spaceRate;
    public SubtitleManager subtitleManager = new SubtitleManager();
    void Start()
    {
        letterRate = 0.03f;
        spaceRate = 0.01f;
        subtitleManager.changeCurrentLanguage("english");
        textContent = subtitleManager.getSubtitleById("002").content;
        StartCoroutine(showSubtitleText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private IEnumerator showSubtitleText()
    {
        if (textContent != "")
        {
            for (int i=0; i < textContent.Length; i++)
            {
                subtitleText.text += textContent[i];
                if (textContent[i].ToString() == " ")
                {
                    yield return new WaitForSeconds(spaceRate);
                }
                else
                {
                    yield return new WaitForSeconds(letterRate);
                }
                
                
            }
        }
        else
        {
            Console.WriteLine("[showSubtitleText]: text is empty.");
        }
    }
    
}
