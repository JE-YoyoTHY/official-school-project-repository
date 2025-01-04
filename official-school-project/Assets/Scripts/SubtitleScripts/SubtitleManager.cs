using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class SubtitleManager
{
    private string _currentLanguage = "chinese";  // 設定中文為預設語言
    private string[] _availableLanguages = {"chinese", "english"};  // 可用語言
    private Dictionary<string, string> _language_path_pairs = new Dictionary<string, string>
    {
        { "chinese", Application.dataPath + "/Resources/JsonFiles/ch_subtitle.json"},
        { "english", Application.dataPath + "/Resources/JsonFiles/en_subtitle.json"},
    };
    string str_json = string.Empty;  // 先將JSON文字檔預設為空，並在函式中讀取

    //-----------------------------------------------------------------------------\\

    public Subtitle getSubtitleById(string id)
    {
        
        using (StreamReader read = new StreamReader(_language_path_pairs[_currentLanguage]))
        {
            // 依照目前語言讀取JSON檔
            str_json = read.ReadToEnd();
        }
        // 將JSON文字檔轉換為JSON檔
        Dictionary<string, Subtitle> subtitleDict = JsonConvert.DeserializeObject<Dictionary<string, Subtitle>>(str_json);

        // 取得整個字幕的宇集，取得底下的字幕組(鍵值對，鍵: id, 值: class Subtitle)，再利用id取得該字幕組
        var subtitle = subtitleDict[id];
        


        return subtitle;
    }

    public void changeCurrentLanguage(string lan)
    {
        if (_availableLanguages.Contains(lan))
        {
            _currentLanguage = lan;
        }
        else
        {
           Console.WriteLine("[changeCurrentLanguage]: Not available.");
        }
    }
    public string getCurrentLanguage()
    {
        return _currentLanguage;
    }
    public string[] getAvailableLanguages()
    {
        return _availableLanguages;
    }

    
}
