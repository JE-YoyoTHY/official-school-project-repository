using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    private string _textContent = string.Empty;
    public TextMeshProUGUI textUI;
    private float _charRate;
    private float _spaceRate;
    public SubtitleManager subtitleManager = new SubtitleManager();
    void Start()
    {
        _charRate = 0.03f;
        _spaceRate = 0.01f;
        subtitleManager.changeCurrentLanguage("english");
        _textContent = subtitleManager.getSubtitleById("002").content;
        StartCoroutine(showSubtitleText());
    }

    public IEnumerator showSubtitleText()
    {
        if (_textContent != "")
        {
            for (int i=0; i < _textContent.Length; i++)
            {
                textUI.text += _textContent[i];
                if (_textContent[i].ToString() == " ")
                {
                    yield return new WaitForSeconds(_spaceRate);
                }
                else
                {
                    yield return new WaitForSeconds(_charRate);
                }
                
                
            }
        }
        else
        {
            Console.WriteLine("[showSubtitleText]: text is empty.");
        }
    }

    public float getCharRate() { return _charRate; }
    public float getSpaceRate() { return _spaceRate; }

}
