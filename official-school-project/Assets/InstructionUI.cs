using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InstructionKeyCodeSetting;
using UnityEngine.InputSystem;
using TMPro;

public class InstructionUI : MonoBehaviour
{
    [Header("Drag-Needed")]
    [SerializeField] private InputActionAsset inputAsset;
    public InstructionsTypeEnum currentInstruction;  // Inspector設定
    [SerializeField] private RebindingManager rebindingManager;  // 為了使用它的convertBindingNameToReadableName()

    [Header("Data")]
    [SerializeField] private Vector2 maskMaxSize;
    [SerializeField] private string actionName;


    [Header("Read Only")]
    [SerializeField] private InputActionMap playerMap;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject keyHorizontalLayout;

    [SerializeField] private Dictionary<string, GameObject> keyboardKeyImagesPair;

    public enum InstructionsTypeEnum
    {
        _None,
        MoveInstruction,
        JumpInstruction,
        ShootFireballInstruction_1,
        ShootFireballInstruction_2,
    }
    public enum ActionsEnum
    {
        MoveLeft, MoveRight,
        Jump,
        UpShootFireball, DownShootFireball, LeftShootFireball, RightShootFireball
    }
    private Dictionary<ActionsEnum, InputAction> inputActions;

    private void Awake()
    {
        if (maskMaxSize == null)
        {
            maskMaxSize = gameObject.GetComponent<RectTransform>().sizeDelta;
        }

        playerMap = inputAsset.FindActionMap("Player");
        background = transform.Find("Background").gameObject;
        keyHorizontalLayout = background.transform.Find("KeyHorizontalLayout").gameObject;
        List<GameObject> childrenOfHorizontalLayout = GameObjectMethods.GetAllChildren(keyHorizontalLayout);
        foreach (GameObject child in childrenOfHorizontalLayout)
        {
            if (child.name.IndexOf("KeyboardKeyImage") != -1)
            {
                // 確認不是例如"+"號這類的東西
                keyboardKeyImagesPair.Add(child.name, child);
            }


        }

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 getMaskMaxSize()
    {
        return maskMaxSize;
    }

    public void changeKeyCodeLabelText()
    {
        if (currentInstruction == InstructionsTypeEnum._None)
        {
            Debug.LogError("currentInstruction is _None");
            return;
        }

        else if (currentInstruction == InstructionsTypeEnum.MoveInstruction)
        {
            // left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.MoveLeft].bindings[0]);

            // right
            getAllKeyCodeLabel()[1].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.MoveRight].bindings[0]);

            setInstructionSize();
        }

        else if (currentInstruction == InstructionsTypeEnum.JumpInstruction)
        {
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.Jump].bindings[0]);
            setInstructionSize();
        }

        else if (currentInstruction == InstructionsTypeEnum.ShootFireballInstruction_1)
        {
            // shoot left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.LeftShootFireball].bindings[0]);

            // shoot down
            getAllKeyCodeLabel()[1].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.DownShootFireball].bindings[0]);

            setInstructionSize();
        }

        else if (currentInstruction == InstructionsTypeEnum.ShootFireballInstruction_2)
        {
            // shoot left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingManager.convertBindingNameToReadableName(inputActions[ActionsEnum.LeftShootFireball].bindings[0]);

            setInstructionSize();
        }
    }

    public List<GameObject> getAllKeyCodeLabel()
    {
        List<GameObject> result = new List<GameObject>();

        GameObject background = transform.GetChild(0).transform.gameObject;
        GameObject horizontalLayout = background.transform.GetChild(1).transform.gameObject;  // child 0: ActionNameLabel | child 1: KeyboardKeyImage_HorizontalLayout

        for (int i = 0; i < horizontalLayout.transform.childCount; i++)
        {
            GameObject keyboardKeyImage = horizontalLayout.transform.GetChild(i).transform.gameObject;

            if (keyboardKeyImage.transform.childCount > 0)
            {
                GameObject keyCodeLabel = keyboardKeyImage.transform.GetChild(0).transform.gameObject;
                if (keyCodeLabel.GetComponent<TextMeshProUGUI>() != null)
                {
                    result.Add(keyCodeLabel);
                }
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
        const float padding = 60.0f;

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
            GameObject keyCodeLabel = keyboardKeyImage.transform.GetChild(0).transform.gameObject;
            print(keyCodeLabel.name);
            if (keyCodeLabel.GetComponent<TextMeshProUGUI>() != null)
            {
                float oldWidth = keyboardKeyImage.GetComponent<RectTransform>().sizeDelta.x;
                float newWidth = keyCodeLabel.GetComponent<TextMeshProUGUI>().preferredWidth + padding;
                float diffWidth = newWidth - oldWidth;
                totalDiffWidth += diffWidth;

                keyboardKeyImage.GetComponent<RectTransform>().sizeDelta += new Vector2(diffWidth, 0);


            }
        }

        GetComponent<RectTransform>().sizeDelta += new Vector2(totalDiffWidth, 0);
        background.GetComponent<RectTransform>().sizeDelta += new Vector2(totalDiffWidth, 0);


    }
    #endregion



}

