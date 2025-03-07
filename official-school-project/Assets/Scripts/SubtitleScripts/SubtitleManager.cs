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
    public static string _currentLanguage = "chinese";  // 設定中文為預設語言
    public static string[] _availableLanguages = {"chinese", "english"};  // 可用語言
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
            // 依照目前語言讀取JSON檔
            str_json = read.ReadToEnd();
        }
        // 將JSON文字檔轉換為JSON檔
        Dictionary<string, Subtitle> subtitleDict = JsonConvert.DeserializeObject<Dictionary<string, Subtitle>>(str_json);

        // 取得整個字幕的宇集，取得底下的字幕組(鍵值對，鍵: id, 值: class Subtitle)，再利用id取得該字幕組
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
            // 依照目前語言讀取JSON檔
            str_json = read.ReadToEnd();
        }
        // 將JSON文字檔轉換為JSON檔
        Dictionary<string, Subtitle> subtitleDict = JsonConvert.DeserializeObject<Dictionary<string, Subtitle>>(str_json);

        // 取得整個字幕的宇集，取得底下的字幕組(鍵值對，鍵: id, 值: class Subtitle)，再利用id取得該字幕組

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
