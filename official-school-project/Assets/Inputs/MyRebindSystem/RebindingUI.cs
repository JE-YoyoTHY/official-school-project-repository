using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// How To Use: �N���}����J�C�@�� RebindUIPrefab, ��J:
/// inputAsset, targetActionRef, rebindCanvas �Y�i�ϥ�
/// </summary>
public class RebindingUI : MonoBehaviour
{
    //// TO DO: �b�P�@map�������Ҧ�action
    #region UI Reference
    // ���l�Ŭ[�c:
    // ------------------------------------ \\
    // Canvas
    //
    //    �u�|���@��
    //  > Overlay (Image)
    //     > PromptLabel (TextMeshProUGUI)
    //
    //    �i�H���ܦh��
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
    [SerializeField] private Canvas rebindCanvas;
    [SerializeField] private BindingChangedEventBroadcaster broadCaster;  // �u�n���N�|Invoke, ���׬O�@���٬O����
    [SerializeField] private GameObject overlay;

    [Header("--- Read Only ---")]
    [SerializeField] private GameObject promptLabel;
    [SerializeField] private GameObject rebindUIPrefab;
    [SerializeField] private GameObject actionLabel;
    [SerializeField] private GameObject startRebindButton;
    [SerializeField] private GameObject bindingLabel;
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

    // �j�w���ޭ�
    private int bindingIndex = 0;
    private HashSet<InputBinding> playerControl_AllUsedBindings = new HashSet<InputBinding>();

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    
    private void Awake()
    {
        promptStrings = new Dictionary<promptStringsNames, string>()
        {
            {promptStringsNames.waitForInput, "Press a new key..." },
            {promptStringsNames.rebindCanceled, "Rebind canceled." }
        };

        #region Set Ref
        gameInputControl = new GameInputControl();

        // �U���T�Ӳz�פW�n���A�]���L�̬O�bInpector����J��
        if (rebindCanvas == null) { Debug.LogError("rebindCanvas is null"); }
        if (targetActionRef == null) { Debug.LogError("targetActionRef is null"); }
        if (inputAsset == null) { Debug.LogError("inputAsset is null"); }

        // rebind canvas's
        if (overlay == null)
        {
            overlay = rebindCanvas.transform.GetChild(rebindCanvas.transform.childCount - 1).transform.gameObject;
        }
        promptLabel = overlay.transform.GetChild(0).transform.gameObject;

        // rebind UI prefab's
        rebindUIPrefab = gameObject;
        actionLabel = rebindUIPrefab.transform.GetChild(0).transform.gameObject;
        startRebindButton = rebindUIPrefab.transform.GetChild(1).transform.gameObject;
        bindingLabel = startRebindButton.transform.GetChild(0).transform.gameObject;
        resetButton = rebindUIPrefab.transform.GetChild(2).transform.gameObject;
        resetLabel = resetButton.transform.GetChild(0).transform.gameObject;

        #endregion





    }
    void Start()
    {
        updateKeyDisplay();
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
            Debug.LogError("InputActionReference ���]�w");
            return;
        }

        // ���ܪ��a���U�s����
        overlay.SetActive(true);
        promptLabel.SetActive(true);
        promptLabel.GetComponent<TextMeshProUGUI>().text = promptStrings[promptStringsNames.waitForInput];

        // �N�ثe�n�諸action����
        targetActionRef.action.Disable();

        rebindOperation = targetActionRef.action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")  // �o��M�U�@��O�@�w�nexclude��
            .WithControlsExcluding("Keyboard/escape")
            .OnComplete(operation =>
            {
                //Debug.Log("�j�w����");
                operation.Dispose();
                targetActionRef.action.Enable();
                updateKeyDisplay();
                broadCaster.broadCast();

                promptLabel.SetActive(false);
                overlay.SetActive(false);

            })
            .OnCancel(operation =>
            {
                Debug.Log("�j�w����");
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
        broadCaster.broadCast();
        updateKeyDisplay();
    }

    public void updateKeyDisplay()
    {
        if (targetActionRef == null || targetActionRef.action == null)
        {
            Debug.LogError("InputActionReference ���]�w");
            return;
        }
        InputBinding binding = targetActionRef.action.bindings[bindingIndex];
        string readableName = convertBindingNameToReadableName(binding);

        bindingLabel.GetComponent<TextMeshProUGUI>().text = readableName;
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
        broadCaster.broadCast();
    }

    public void updateAllKeyDisplay()
    {
        foreach(GameObject rebindPrefab in GameObjectMethods.GetAllChildren(rebindCanvas.gameObject))
        {
            if (rebindPrefab.GetComponent<RebindingUI>())
            {
                rebindPrefab.GetComponent<RebindingUI>().updateKeyDisplay();
            }
            
        }
    }



    /// <summary>
    /// �N�ǤJ��binding���W�r�ഫ���H���iŪ����r�A
    /// �Ҧp�GKeyboard/space => space
    /// </summary>
    /// <param name="binding"></param>
    /// <returns>�H���iŪ��r</returns>
    public string convertBindingNameToReadableName(InputBinding binding)
    {
        string readableName = InputControlPath.ToHumanReadableString(
        binding.effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames
        );

        return readableName;
    }

    /// <summary>
    /// ���o�ǤJ��InputActionMap���Ҧ��ιL������(��J)
    /// </summary>
    /// <param name="actionMap"></param>
    /// <returns>�@��List<InputBinding>�A�̭��O�Ҧ��ιL����J</returns>
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
