using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public static class SubtitleManager
{
    public static string _currentLanguage = "chinese";  // �]�w���嬰�w�]�y��
    public static string[] _availableLanguages = {"chinese", "english"};  // �i�λy��
    public static Dictionary<string, string> _languagePaths = new Dictionary<string, string>
    {
        { "chinese", Application.dataPath + "/Resources/JsonFiles/ch_subtitle.json"},
        { "english", Application.dataPath + "/Resources/JsonFiles/en_subtitle.json"},
    };

    //-----------------------------------------------------------------------------\\

    public static Subtitle getSubtitleById(string id)
    {
        string str_json = string.Empty;
        
        using (StreamReader read = new StreamReader(_languagePaths[_currentLanguage]))
        {
            // �̷ӥثe�y��Ū��JSON��
            str_json = read.ReadToEnd();
        }
        // �NJSON��r���ഫ��JSON��
        Dictionary<string, Subtitle> subtitleDict = JsonConvert.DeserializeObject<Dictionary<string, Subtitle>>(str_json);

        // ���o��Ӧr�����t���A���o���U���r����(��ȹ�A��: id, ��: class Subtitle)�A�A�Q��id���o�Ӧr����
        var subtitle = subtitleDict[id];
        
        if (subtitle == null)
        {
            Debug.LogError("[getSubtitleById]: id not match.");
            return null;
        }


        return subtitle;
    }

    public static string getSubtitleContentById(string id)
    {
        string str_json = string.Empty;
        using (StreamReader read = new StreamReader(_languagePaths[_currentLanguage]))
        {
            // �̷ӥثe�y��Ū��JSON��
            str_json = read.ReadToEnd();
        }
        // �NJSON��r���ഫ��JSON��
        Dictionary<string, Subtitle> subtitleDict = JsonConvert.DeserializeObject<Dictionary<string, Subtitle>>(str_json);

        // ���o��Ӧr�����t���A���o���U���r����(��ȹ�A��: id, ��: class Subtitle)�A�A�Q��id���o�Ӧr����

        Subtitle subtitle = subtitleDict[id];
        string content = subtitle.content;

        if (subtitle == null)
        {
            Debug.LogError("[getSubtitleContentById]: id not match.");
            return null;
        }

        if (content == null)
        {
            Debug.LogError("[getSubtitleContentById]: id matches, but no content found.");
            return null;
        }



        return content;
    }

    public static void changeCurrentLanguage(string lan)
    {
        if (_availableLanguages.Contains(lan))
        {
            _currentLanguage = lan;
        }
        else
        {
           Debug.LogError("[changeCurrentLanguage]: Not available.");
        }
    }

    
}
