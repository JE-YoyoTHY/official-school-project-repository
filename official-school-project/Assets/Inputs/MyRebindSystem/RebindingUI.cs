using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindingUI : MonoBehaviour
{
    //// How To Use: �N���}����J�C�@�� RebindUIPrefab, �Y�i���s�j�w��J

    [SerializeField] private GameObject inputManager;
    private GameInputControl gameInputControl;
    [SerializeField] private InputActionReference targetActionRef;

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
    [SerializeField] private Image overlay;
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private Button startRebindButton;
    [SerializeField] private TextMeshProUGUI bindingLabel;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button resetLabel;

    #endregion

    private void Awake()
    {
        // Set ref
        if (inputManager == null) { Debug.LogError("inputManager is null"); }
        else { gameInputControl = inputManager.GetComponent<GameInputControl>(); }

        if (targetActionRef == null) { Debug.LogError("targetActionRef is null"); }


       
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startRebind()
    {

    }

    public void performRebind()
    {

    }

    public void endRebind()
    {

    }
}
