using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "RebindSystemDataBase", menuName = "CustomScriptableObject/RebindSystemDataBase")]
public class RebindSystemDataBase : ScriptableObject
{
    public Dictionary<string, string> readableNameToShorterOrImagePair
    = new Dictionary<string, string>()
    {
        // SHORTER TERM AREA
            {"Left Shift", "L Shift"},
            {"Right Shift", "R Shift"},
            {"Left Control", "L Ctrl" },
            {"Right Control", "R Ctrl" },
            {"Left Alt", "L Alt" },
            {"Right Alt", "R Alt" },
            {"Left System", "L System" },
            {"Right System", "R System" },

            // IMAGE AREA
            {"Left Arrow", "IMAGE" },
            {"Right Arrow", "IMAGE" },
            {"Up Arrow", "IMAGE" },
            {"Down Arrow", "IMAGE" }
    };

    // key value pair
    public List<string> keyImages_Keys;
    public List<Sprite> keyImages_Values;
    public Dictionary<string, Sprite> keyImagesDict = new Dictionary<string, Sprite>();

    public bool isRebinding;
    public string getReadableNameFromBindingName(InputBinding binding)
    {
        string readableName = InputControlPath.ToHumanReadableString(
        binding.effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames
        );

        return readableName;

    }
    public string getShorterTermFromReadableName(string readableName)
    {
        if (readableNameToShorterOrImagePair.ContainsKey(readableName))
        {
            if (readableNameToShorterOrImagePair[readableName] != "IMAGE")
            {
                return readableNameToShorterOrImagePair[readableName];
            }
            else
            {
                return "IMAGE";
            }
        }
        else
        {
            return null;
        }
    }

    public void dataInit()
    {
        isRebinding = false;

        // �T�{�W�r�M�Ϥ��ƶq�O�_�k�X
        if (keyImages_Keys.Count != keyImages_Values.Count)
        {
            Debug.LogWarning("key and image's length not match");
        }

        // �H�W�r��key, �Ϥ���value, �[�Jdict
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

    private void OnEnable()
    {
        dataInit();
    }

}
