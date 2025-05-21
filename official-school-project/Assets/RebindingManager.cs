using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RebindingManager : MonoBehaviour
{
    public UnityEvent BindingChangedEvent;
    public bool isRebinding;


    // key value pair
    public List<string> keyImages_Keys;
    public List<Sprite> keyImages_Values;
    

    
    public Dictionary<string, Sprite> keyImagesDict = new Dictionary<string, Sprite>();

    private void Awake()
    {
        isRebinding = false;

        
        if (keyImages_Keys.Count != keyImages_Values.Count)
        {
            Debug.LogWarning("key and image's length not match");
        }
        
        if (keyImages_Keys.Count <= keyImages_Values.Count)
        {
            foreach (string _key in keyImages_Keys)
            {
                keyImagesDict.Add(_key, keyImages_Values[keyImages_Keys.IndexOf(_key)]);
            }
        }
        else
        {
            foreach (Sprite _value in keyImages_Values)
            {
                keyImagesDict.Add(keyImages_Keys[keyImages_Values.IndexOf(_value)], _value);
            }
        }
    }

    public void bindingChangedBroadcast()
    {
        BindingChangedEvent.Invoke();
    }

    /// <summary>
    /// DEPRECATED
    /// </summary>
    /// <param name="rebindUI"></param>
    /// <param name="keyCodeName"></param>
    public void rebindUI_ShowImageAndDisableText(GameObject rebindUI, string keyCodeName)
    {
        GameObject startRebindButton = rebindUI.gameObject.transform.Find("StartRebindButton").gameObject;
        if (startRebindButton != null)
        {
            GameObject keyCodeText = startRebindButton.transform.Find("KeyCodeText").gameObject;
            GameObject keyImage = startRebindButton.transform.Find("KeyImage").gameObject;
            if (keyCodeText != null && keyImage != null)
            {
                keyCodeText.SetActive(false);

                Sprite targetSprite = keyImagesDict[keyCodeName];  // 從這取得圖片
                keyImage.GetComponent<Image>().sprite = targetSprite;
                RectTransform keyImageTransform = keyImage.GetComponent<RectTransform>();
                float spriteWidth = keyImage.GetComponent<Image>().sprite.rect.width;
                float spriteHeight = keyImage.GetComponent<Image>().sprite.rect.height;
                print(spriteWidth);
                print(spriteHeight);
                float _scale = startRebindButton.GetComponent<RectTransform>().sizeDelta.y / spriteHeight;
                spriteWidth *= _scale;
                spriteHeight *= _scale;
                keyImageTransform.sizeDelta = new Vector2(spriteWidth, spriteHeight);

                // 將真正的Button隱藏掉, 圖片剩KeyImage
                /* 暫時取消此功能
                Color startRebindButtonColor = startRebindButton.GetComponent<Image>().color;
                startRebindButtonColor.a = 0;
                startRebindButton.GetComponent<Image>().color = startRebindButtonColor;
                */

                keyImage.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Can't find child named [StartRebindButton]");
        }
    }


}
