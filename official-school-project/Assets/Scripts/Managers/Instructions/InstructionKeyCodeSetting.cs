using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstructionKeyCodeSetting : MonoBehaviour
{
    [Header("Drag-Needed")]
    [SerializeField] private InputActionAsset inputAsset;
    public InstructionsTypeEnum currentInstruction;  // Inspector設定
    [SerializeField] private RebindingUI rebindingUI;  // 為了使用它的convertBindingNameToReadableName()

    [Header("Read Only")]
    [SerializeField] private InputActionMap playerMap;

    // Start is called before the first frame update
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
        playerMap = inputAsset.FindActionMap("Player");
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

    public void changeKeyCodeLabelText()
    {
        if (currentInstruction == InstructionsTypeEnum._None)
        {
            Debug.LogError("currentInstruction is _None");
            return;
        }

        else if (currentInstruction == InstructionsTypeEnum.MoveInstruction)
        {
            print(getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text);

            // left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.MoveLeft].bindings[0]);
            print(getAllKeyCodeLabel()[0].name);
            // right
            getAllKeyCodeLabel()[1].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.MoveRight].bindings[0]);
            print(getAllKeyCodeLabel()[1].name);

        }

        else if (currentInstruction == InstructionsTypeEnum.JumpInstruction)
        {
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.Jump].bindings[0]);
        }

        else if (currentInstruction == InstructionsTypeEnum.ShootFireballInstruction_1)
        {
            // shoot left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.LeftShootFireball].bindings[0]);

            // shoot down
            getAllKeyCodeLabel()[1].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.DownShootFireball].bindings[0]);

        }

        else if (currentInstruction == InstructionsTypeEnum.ShootFireballInstruction_2)
        {
            // shoot left
            getAllKeyCodeLabel()[0].GetComponent<TextMeshProUGUI>().text = rebindingUI.convertBindingNameToReadableName(inputActions[ActionsEnum.LeftShootFireball].bindings[0]);
        }
    }

    public List<GameObject> getAllKeyCodeLabel()
    {
        List<GameObject> result = new List<GameObject>();

        GameObject background = gameObject.transform.GetChild(0).transform.gameObject;
        for (int i = 1; i < background.transform.childCount; i++)
        {
            GameObject keyboardKeyImage = background.transform.GetChild(i).transform.gameObject;

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
}
