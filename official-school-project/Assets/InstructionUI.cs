using DG.Tweening;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour
{
    public enum InstructionTypeEnum
    {
        _None,
        Move,
        Jump,
        ShootFireball_OneKey,
        ShootFireball_TwoKey,
    }
    public enum ActionsEnum
    {
        MoveLeft, MoveRight,
        Jump,
        UpShootFireball, DownShootFireball, LeftShootFireball, RightShootFireball
    }

    

    [Header("Drag-Needed")]
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] RebindSystemDataBase rebindSystemDataBase;
    [SerializeField] private InstructionTypeEnum currentInstructionType;
    [Header("If InstructionTypeEnum is ShootFireball")]
    [Header("One Key")]
    [SerializeField] private ActionsEnum currentShootFireballInstruction_OneKey;
    [Header("Two Key")]
    [SerializeField] private ActionsEnum currentShootFireballInstruction_TwoKey_First;
    [SerializeField] private ActionsEnum currentShootFireballInstruction_TwoKey_Second;


    [Header("Data")]
    [SerializeField] private Vector2 maskMaxSize;
    [SerializeField] private string actionName;


    [Header("Read Only")]
    [SerializeField] private InputActionMap playerMap;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject keyHorizontalLayout;



    private Dictionary<ActionsEnum, InputAction> inputActions;

    private void Awake()
    {
        if (maskMaxSize == Vector2.zero)
        {
            maskMaxSize = gameObject.GetComponent<RectTransform>().sizeDelta;
        }

        playerMap = inputAsset.FindActionMap("Player");

        background = transform.Find("Background").gameObject;
        keyHorizontalLayout = background.transform.Find("KeyHorizontalLayout").gameObject;
        List<GameObject> childrenOfHorizontalLayout = GameObjectMethods.GetAllChildren(keyHorizontalLayout);


        inputActions = new Dictionary<ActionsEnum, InputAction>()
        {
            { ActionsEnum.MoveLeft,                   playerMap.FindAction("moveLeft") },
            { ActionsEnum.MoveRight,                 playerMap.FindAction("moveRight") },
            { ActionsEnum.Jump,                           playerMap.FindAction("jump") },
            { ActionsEnum.UpShootFireball,       playerMap.FindAction("fireballDirUp") },
            { ActionsEnum.DownShootFireball,   playerMap.FindAction("fireballDirDown") },
            { ActionsEnum.LeftShootFireball,   playerMap.FindAction("fireballDirLeft") },
            { ActionsEnum.RightShootFireball, playerMap.FindAction("fireballDirRight") }
        };


        gameObject.SetActive(false);
    }
    
    void Start()
    {
        UIInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 getMaskMaxSize()
    {
        return maskMaxSize;
    }

    public InstructionTypeEnum getCurrentInstructionType()
    {
        return currentInstructionType;
    }
    public void manageChangingKeyDisplay()
    {

        if (currentInstructionType == InstructionTypeEnum._None)
        {
            Debug.LogError("currentInstructionType is _NULL");
            return;
        }

        else if (currentInstructionType == InstructionTypeEnum.Move)
        {
            changeKeyboardKeyImageDisplay(0, 0);
            changeKeyboardKeyImageDisplay(1, 1);
            setInstructionSize();
        }

        else if (currentInstructionType == InstructionTypeEnum.Jump)
        {
            changeKeyboardKeyImageDisplay(2, 0);
            setInstructionSize();
        }

        else if (currentInstructionType == InstructionTypeEnum.ShootFireball_OneKey)
        {

            changeKeyboardKeyImageDisplay((int)currentShootFireballInstruction_OneKey, 0);
            setInstructionSize();
        }

        else if (currentInstructionType == InstructionTypeEnum.ShootFireball_TwoKey)
        {
            print("Manage changing key display - two key");
            print((int)currentShootFireballInstruction_TwoKey_First);
            print((int)currentShootFireballInstruction_TwoKey_Second);
            changeKeyboardKeyImageDisplay((int)currentShootFireballInstruction_TwoKey_First, 0);  
            changeKeyboardKeyImageDisplay((int)currentShootFireballInstruction_TwoKey_Second, 1);  
            setInstructionSize();
        }
    }

    public void changeKeyboardKeyImageDisplay(int actionEnumIndex, int targetKeyboardKeyImageIndex)
    {
        print($"Received actionEnumIndex: {actionEnumIndex}");
        ActionsEnum m_action = (ActionsEnum)actionEnumIndex;
        if (rebindSystemDataBase == null)
            Debug.LogError("rebindSystemDataBase 是 null");

        if (inputActions == null)
            Debug.LogError("inputActions 是 null");

        if (!inputActions.ContainsKey(m_action))
            Debug.LogError($"inputActions 不包含 key: {m_action}");

        else if (inputActions[m_action].bindings.Count == 0)
            Debug.LogError($"inputActions[{m_action}].bindings 是空的");

        string readableName = rebindSystemDataBase.getReadableNameFromBindingName(inputActions[m_action].bindings[0]);
        string shorterTerm = rebindSystemDataBase.getShorterTermFromReadableName(readableName);
        GameObject targetKeyboardKeyImage = getAllKeyboardKeyImage()[targetKeyboardKeyImageIndex];
        GameObject keyCodeText = targetKeyboardKeyImage.transform.GetChild(0).gameObject;
        GameObject keyImage = targetKeyboardKeyImage.transform.GetChild(1).gameObject;
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

                Sprite targetSprite = rebindSystemDataBase.keyImagesDict[readableName];  // 從這取得圖片
                keyImage.GetComponent<Image>().sprite = targetSprite;
                keyImage.GetComponent<Image>().color = Color.black;
                RectTransform keyImageTransform = keyImage.GetComponent<RectTransform>();
                float spriteWidth = keyImage.GetComponent<Image>().sprite.rect.width;
                float spriteHeight = keyImage.GetComponent<Image>().sprite.rect.height;
                print($"Sprite Width: {spriteWidth}");
                print($"Sprite Height: {spriteHeight}");
                float _scale = getMaskMaxSize().y / spriteHeight;
                float _scale_fineTune = 1.0f / 2.75f;
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

        print("change display completed");
    }
    public List<GameObject> getAllKeyboardKeyImage()
    {
        List<GameObject> result = new List<GameObject>();

        GameObject background = transform.GetChild(0).transform.gameObject;
        GameObject horizontalLayout = background.transform.GetChild(1).transform.gameObject;  // child 0: ActionNameLabel | child 1: KeyboardKeyImage_HorizontalLayout

        for (int i = 0; i < horizontalLayout.transform.childCount; i++)
        {
            GameObject keyboardKeyImage = horizontalLayout.transform.GetChild(i).transform.gameObject;
            if (keyboardKeyImage.name.IndexOf("KeyboardKeyImage") != -1)
            {
                result.Add(keyboardKeyImage);
            }
        }

        return result;
    }

    #region FUNCTION: setInstructionSize()
    [ContextMenu("set instruction size")]
    public void setInstructionSize()
    {
        print("set instru");
        const string imageFilterName = "KeyboardKeyImage";
        const float widthPaddingForText = 60.0f;
        const float widthPaddingForImage = 18.0f;

        GameObject background = transform.GetChild(0).transform.gameObject;
        GameObject horizontalLayout = background.transform.GetChild(1).transform.gameObject;  // child 0: ActionNameLabel | child 1: KeyboardKeyImage_HorizontalLayout
        List<GameObject> horizontalLayout_AllChildren = GameObjectMethods.GetAllChildren(horizontalLayout);
        List<GameObject> horizontalLayout_AllKeyboardKeyImage = new List<GameObject>();
        foreach (GameObject child in horizontalLayout_AllChildren)
        {
            if (child.name.IndexOf(imageFilterName) != -1)
            {
                // 不是按鍵圖片
                horizontalLayout_AllKeyboardKeyImage.Add(child);
                print(child.name);
            }
        }

        float totalDiffWidth = 0;
        foreach (GameObject keyboardKeyImage in horizontalLayout_AllKeyboardKeyImage)
        {
            GameObject keyCodeText = keyboardKeyImage.transform.GetChild(0).gameObject;
            GameObject keyImage = keyboardKeyImage.transform.GetChild(1).gameObject;
            print(keyCodeText.name);
            
            if (keyCodeText.gameObject.activeSelf == true && keyImage.gameObject.activeSelf == false)
            {
                if (keyCodeText.GetComponent<TextMeshProUGUI>() != null)
                {
                    float oldWidth = keyboardKeyImage.GetComponent<RectTransform>().sizeDelta.x;
                    float newWidth = keyCodeText.GetComponent<TextMeshProUGUI>().preferredWidth + widthPaddingForText;
                    float diffWidth = newWidth - oldWidth;
                    totalDiffWidth += diffWidth;

                    keyboardKeyImage.GetComponent<RectTransform>().sizeDelta += new Vector2(diffWidth, 0);
                }
            }
            else if (keyCodeText.gameObject.activeSelf == false && keyImage.gameObject.activeSelf == true)
            {
                float oldWidth = keyboardKeyImage.GetComponent<RectTransform>().sizeDelta.x;
                float newWidth = keyImage.GetComponent<RectTransform>().sizeDelta.x + widthPaddingForImage;
                float diffWidth = newWidth - oldWidth;
                totalDiffWidth += diffWidth;

                keyboardKeyImage.GetComponent<RectTransform>().sizeDelta += new Vector2(diffWidth, 0);
            }

        }

        GetComponent<RectTransform>().sizeDelta += new Vector2(totalDiffWidth, 0);
        background.GetComponent<RectTransform>().sizeDelta += new Vector2(totalDiffWidth, 0);


    }
    #endregion

    public void UIInit()
    {
        GameObject background = transform.GetChild(0).gameObject;
        GameObject actionNameLabel = background.transform.GetChild(0).gameObject;

        actionNameLabel.GetComponent<TextMeshProUGUI>().text = actionName;
        manageChangingKeyDisplay();

        if (currentInstructionType == InstructionTypeEnum.ShootFireball_OneKey)
        {
            if (getAllKeyboardKeyImage().Count > 1)
            {
                Debug.LogError("Selected ShootFireball_OneKey but detected two KeyboardKeyImages");
            }
        }
    }

    public void changeAction_ShootFireball_OneKey(ActionsEnum newAction)
    {
        currentShootFireballInstruction_OneKey = newAction;
        manageChangingKeyDisplay();
    }

    public void changeAction_ShootFireball_TwoKey(ActionsEnum newFirstAction, ActionsEnum newSecondAction)
    {
        currentShootFireballInstruction_TwoKey_First = newFirstAction;
        currentShootFireballInstruction_TwoKey_Second = newSecondAction;
        manageChangingKeyDisplay();
    }



}

