using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.UI;
using UnityEngine.UI;

/// <summary>
/// How To Use: 將此腳本放入每一個 RebindUIPrefab, 拖入:
/// inputAsset, targetActionRef, rebindingManager 即可使用
/// </summary>
public class RebindingUI : MonoBehaviour
{
    //// TO DO: 在同一map中偵測所有action
    #region UI Reference
    // 父子級架構:
    // ------------------------------------ \\
    // Canvas
    //
    //    只會有一個
    //  > Overlay (Image)
    //     > PromptLabel (TextMeshProUGUI)
    //
    //    可以有很多個
    //  > RebindUIPrefab (Empty GameObject)
    //     > ActionLabel (TextMeshProUGUI)
    //     > StartRebindButton (Button)
    //        > BindingLabel (TextMeshProUGUI)
    //     > ResetButton (Button)
    //        > ResetLabel (TextMeshProUGUI)
    // -------------------------------------- \\
    
    private GameInputControl gameInputControl;


    [Header("--- Drag-Needed ---")]
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private InputActionReference targetActionRef;
    [SerializeField] private RebindingManager rebindingManager;
    [SerializeField] private GameObject overlay;

    [Header("--- Read Only ---")]
    [SerializeField] private GameObject promptLabel;
    [SerializeField] private GameObject rebindUIPrefab;
    [SerializeField] private GameObject actionLabel;
    [SerializeField] private GameObject startRebindButton;
    [SerializeField] private GameObject keyCodeText;
    [SerializeField] private GameObject keyImage;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject resetLabel;

    #endregion

    // promptLabel texts
    private Dictionary<promptStringsNames, string> promptStrings;
    private enum promptStringsNames
    {
        waitForInput, 
        rebindCanceled, 
    };

    private Dictionary<string, string> readableNameToShorterOrImagePair;

    // 綁定索引值
    private int bindingIndex = 0;
    private HashSet<InputBinding> playerControl_AllUsedBindings = new HashSet<InputBinding>();

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    private Dictionary<string, Sprite> keyImagesDict;



    private void Awake()
    {
        promptStrings = new Dictionary<promptStringsNames, string>()
        {
            {promptStringsNames.waitForInput, "Press a new key... (Esc to cancel)" },
            {promptStringsNames.rebindCanceled, "Rebind canceled." }
        };

        readableNameToShorterOrImagePair = new Dictionary<string, string>()
        {
            {"Left Shift", "L Shift"},
            {"Right Shift", "R Shift"},
            {"Left Arrow", "IMAGE" },
            {"Right Arrow", "IMAGE" },
            {"Up Arrow", "IMAGE" },
            {"Down Arrow", "IMAGE" },
        };

        #region Set Ref
        gameInputControl = new GameInputControl();

        // 下面三個理論上要有，因為他們是在Inpector中拖入的
        if (rebindingManager == null)
        {
            rebindingManager = transform.parent.gameObject.GetComponent<RebindingManager>();  // 嘗試從父級取得
            if (rebindingManager == null)
            {
                Debug.Log("Rebind Canvas is null");
            }
        }
        keyImagesDict = rebindingManager.GetComponent<RebindingManager>().keyImagesDict;

        if (targetActionRef == null) { Debug.LogError("targetActionRef is null"); }
        if (inputAsset == null) { Debug.LogError("inputAsset is null"); }

        // rebind canvas's
        if (overlay == null)
        {
                     ///// To do
            overlay = rebindingManager.transform.GetChild(rebindingManager.transform.childCount - 1).transform.gameObject;
        }
        promptLabel = overlay.transform.GetChild(0).transform.gameObject;

        // rebind UI prefab's
        rebindUIPrefab = gameObject;
        actionLabel = rebindUIPrefab.transform.Find("ActionLabel").transform.gameObject;
        startRebindButton = rebindUIPrefab.transform.Find("StartRebindButton").transform.gameObject;
        keyCodeText = startRebindButton.transform.Find("KeyCodeText").transform.gameObject;
        keyImage = startRebindButton.transform.Find("KeyImage").transform.gameObject;
        resetButton = rebindUIPrefab.transform.Find("ResetButton").transform.gameObject;
        resetLabel = resetButton.transform.GetChild(0).transform.gameObject;

        #endregion





    }
    void Start()
    {
        updateStartBindingButtonDisplay();
        overlay.SetActive(false);
        promptLabel.SetActive(false);

        /*
        playerControl_AllUsedBindings = getAllUsedBindings(inputAsset.FindActionMap("Player"));

        foreach(InputBinding binding in playerControl_AllUsedBindings)
        {
            print(binding);
        }
        */
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rebindOperation != null && Input.GetKeyDown(KeyCode.Escape))
        {
            rebindOperation.Cancel();
        }

    }

    public void performRebind()
    {
        if (targetActionRef == null || targetActionRef.action == null)
        {
            Debug.LogError("InputActionReference 未設定");
            return;
        }

        // 提示玩家按下新按鍵
        overlay.SetActive(true);
        promptLabel.SetActive(true);
        promptLabel.GetComponent<TextMeshProUGUI>().text = promptStrings[promptStringsNames.waitForInput];

        // 將目前要改的action停掉
        targetActionRef.action.Disable();

        rebindOperation = targetActionRef.action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")  // 這行和下一行是一定要exclude的
            .WithControlsExcluding("Keyboard/escape")
            .OnComplete(operation =>
            {
                //Debug.Log("綁定完成");
                operation.Dispose();
                targetActionRef.action.Enable();
                updateStartBindingButtonDisplay();
                rebindingManager.bindingChangedBroadcast();

                promptLabel.SetActive(false);
                overlay.SetActive(false);

            })
            .OnCancel(operation =>
            {
                Debug.Log("綁定取消");
                operation.Dispose();
                targetActionRef.action.Enable();
                promptLabel.GetComponent<TextMeshProUGUI>().text = promptStrings[promptStringsNames.rebindCanceled];

                promptLabel.SetActive(false);
                overlay.SetActive(false);


            });
        rebindOperation.Start();

    }

    public void resetRebind()
    {
        targetActionRef.action.RemoveAllBindingOverrides();
        rebindingManager.bindingChangedBroadcast();
        updateStartBindingButtonDisplay();
    }

    public void updateStartBindingButtonDisplay()
    {
        if (targetActionRef == null || targetActionRef.action == null)
        {
            Debug.LogError("InputActionReference 未設定");
            return;
        }
        InputBinding binding = targetActionRef.action.bindings[bindingIndex];
        string readableName = convertBindingNameToReadableName(binding);
        string shorterTerm = convertReadableNameToShorterTerm(readableName);

        if (shorterTerm == null)
        {
            keyImage.SetActive(false);
            keyCodeText.SetActive(true);
            keyCodeText.GetComponent<TextMeshProUGUI>().text = readableName;
        }
        else if (shorterTerm != "IMAGE")
        {
            keyImage.SetActive(false);
            keyCodeText.SetActive(true);
            keyCodeText.GetComponent<TextMeshProUGUI>().text = shorterTerm;
        }
        else if (shorterTerm == "IMAGE")
        {
            if (keyCodeText != null && keyImage != null)
            {
                keyCodeText.SetActive(false);

                Sprite targetSprite = keyImagesDict[readableName];  // 從這取得圖片
                keyImage.GetComponent<Image>().sprite = targetSprite;
                RectTransform keyImageTransform = keyImage.GetComponent<RectTransform>();
                float spriteWidth = keyImage.GetComponent<Image>().sprite.rect.width;
                float spriteHeight = keyImage.GetComponent<Image>().sprite.rect.height;
                print(spriteWidth);
                print(spriteHeight);
                float _scale = startRebindButton.GetComponent<RectTransform>().sizeDelta.y / spriteHeight;
                float _scale_fineTune = 1.0f / 2.5f;
                _scale *= _scale_fineTune;
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
    }

    public void resetAllRebind(InputActionMap actionMap)
    {
        foreach (InputAction action in actionMap.actions)
        {
            action.RemoveAllBindingOverrides();
        }
        updateAllKeyDisplay();

    }

    public void resetAllRebindButton_OnClick()
    {
        resetAllRebind(inputAsset.FindActionMap("Player"));
        rebindingManager.bindingChangedBroadcast();
    }

    public void updateAllKeyDisplay()
    {
        foreach(GameObject rebindPrefab in GameObjectMethods.GetAllChildren(rebindingManager.gameObject))
        {
            if (rebindPrefab.GetComponent<RebindingUI>())
            {
                rebindPrefab.GetComponent<RebindingUI>().updateStartBindingButtonDisplay();
            }
            
        }
    }



    /// <summary>
    /// 將傳入的binding的名字轉換成人類可讀的文字，
    /// 例如：Keyboard/space => space
    /// </summary>
    /// <param name="binding"></param>
    /// <returns>人類可讀文字</returns>
    public string convertBindingNameToReadableName(InputBinding binding)
    {
        string readableName = InputControlPath.ToHumanReadableString(
        binding.effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames
        );

        return readableName;
    }

    public string convertReadableNameToShorterTerm(string readableName)
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

    

    

    /// <summary>
    /// 取得傳入之InputActionMap中所有用過的按鍵(輸入)
    /// </summary>
    /// <param name="actionMap"></param>
    /// <returns>一個List<InputBinding>，裡面是所有用過的輸入</returns>
    public HashSet<InputBinding> getAllUsedBindings(InputActionMap actionMap)
    {
        HashSet<InputBinding> allUsedBindings = new HashSet<InputBinding>();

        foreach(InputAction action in actionMap.actions)
        {
            foreach(InputBinding binding in action.bindings)
            {
                if (binding.isComposite || string.IsNullOrWhiteSpace(binding.effectivePath)) continue;
                print(binding.effectivePath);
                allUsedBindings.Add(binding);
            }
        }

        return allUsedBindings;
    }
}
