using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// How To Use: �N���}����J�C�@�� RebindUIPrefab, ��J:
/// inputAsset, targetActionRef, rebindingManager �Y�i�ϥ�
/// </summary>
public class RebindingUI : MonoBehaviour
{
    //// TO DO: �b�P�@map�������Ҧ�action

    #region Ref
    private GameInputControl gameInputControl;

    [Header("--- Drag-Needed ---")]
    [SerializeField] private string actionName;
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private InputActionReference targetActionRef;
    [SerializeField] private RebindSystemDataBase rebindSystemDataBase;

    [Header("--- Read Only ---")]
    [SerializeField] private GameObject rebindingParent;
    [SerializeField] private GameObject overlay;
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
    private List<string> keyImages_Keys;
    private List<Sprite> keyImages_Values;
    private Dictionary<string, Sprite> keyImagesDict = new Dictionary<string, Sprite>();

    // �j�w���ޭ�
    private int bindingIndex = 0;
    private HashSet<InputBinding> playerControl_AllUsedBindings = new HashSet<InputBinding>();

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;



    private void Awake()
    {
        rebindingParent = transform.parent.gameObject;
        promptStrings = new Dictionary<promptStringsNames, string>()
        {
            {promptStringsNames.waitForInput, "Press a new key... (Esc to cancel)" },
            {promptStringsNames.rebindCanceled, "Rebind canceled." }
        };
        readableNameToShorterOrImagePair = rebindSystemDataBase.readableNameToShorterOrImagePair;
        keyImages_Keys = rebindSystemDataBase.keyImages_Keys;
        keyImages_Values = rebindSystemDataBase.keyImages_Values;
        keyImagesDict = rebindSystemDataBase.keyImagesDict;

        #region Set Ref
        gameInputControl = new GameInputControl();

        if (targetActionRef == null) { Debug.LogError("targetActionRef is null"); }
        if (inputAsset == null) { Debug.LogError("inputAsset is null"); }


        

        // rebind UI prefab's
        rebindUIPrefab = gameObject;
        actionLabel = rebindUIPrefab.transform.Find("ActionLabel").transform.gameObject;
        startRebindButton = rebindUIPrefab.transform.Find("StartRebindButton").transform.gameObject;
        keyCodeText = startRebindButton.transform.Find("KeyCodeText").transform.gameObject;
        keyImage = startRebindButton.transform.Find("KeyImage").transform.gameObject;
        resetButton = rebindUIPrefab.transform.Find("ResetButton").transform.gameObject;
        resetLabel = resetButton.transform.GetChild(0).transform.gameObject;

        #endregion


        actionLabel.GetComponent<TextMeshProUGUI>().text = actionName;




    }
    void Start() { 
        if (overlay == null)
        {
            overlay = rebindSystemDataBase.overlay;
        }
        promptLabel = overlay.transform.GetChild(0).transform.gameObject;
        overlay.gameObject.GetComponent<Image>().enabled = false;
        promptLabel.SetActive(false);
        updateStartBindingButtonDisplay();
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
            rebindSystemDataBase.isRebinding = false;
        }

    }

    public void performRebind()
    {
        rebindSystemDataBase.isRebinding = true;
        if (targetActionRef == null || targetActionRef.action == null)
        {
            Debug.LogError("InputActionReference ���]�w");
            rebindSystemDataBase.isRebinding = false;
            return;
        }

        // ���ܪ��a���U�s����
        overlay.GetComponent<Image>().enabled = true;  
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
                updateStartBindingButtonDisplay();
                rebindingParent.GetComponent<RebindingManager>().bindingChangedBroadcast();

                promptLabel.SetActive(false);
                overlay.GetComponent <Image>().enabled = false;
                rebindSystemDataBase.isRebinding = false;

            })
            .OnCancel(operation =>
            {
                Debug.Log("�j�w����");
                operation.Dispose();
                targetActionRef.action.Enable();
                promptLabel.GetComponent<TextMeshProUGUI>().text = promptStrings[promptStringsNames.rebindCanceled];

                promptLabel.SetActive(false);
                overlay.GetComponent<Image>().enabled = false;
                rebindSystemDataBase.isRebinding = false;

            });
        rebindOperation.Start();

    }

    public void resetRebind()
    {
        targetActionRef.action.RemoveAllBindingOverrides();
        rebindingParent.GetComponent<RebindingManager>().bindingChangedBroadcast();
        updateStartBindingButtonDisplay();
    }

    public void updateStartBindingButtonDisplay()
    {
        if (targetActionRef == null || targetActionRef.action == null)
        {
            Debug.LogError("InputActionReference ���]�w");
            return;
        }
        InputBinding binding = targetActionRef.action.bindings[bindingIndex];
        string readableName = convertBindingNameToReadableName(binding);
        string shorterTerm = rebindSystemDataBase.getShorterTermFromReadableName(readableName);

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

                Sprite targetSprite = keyImagesDict[readableName];  // �q�o���o�Ϥ�
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

                // �N�u����Button���ñ�, �Ϥ���KeyImage
                /* �Ȯɨ������\��
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
        rebindingParent.GetComponent<RebindingManager>().bindingChangedBroadcast();
    }

    public void updateAllKeyDisplay()
    {
        foreach(GameObject rebindPrefab in GameObjectMethods.GetAllChildren(rebindingParent))
        {
            if (rebindPrefab.GetComponent<RebindingUI>())
            {
                rebindPrefab.GetComponent<RebindingUI>().updateStartBindingButtonDisplay();
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
