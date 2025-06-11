using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;
using TMPro;
using static InstructionUI;

public class InstructionUIManager : MonoBehaviour
{
    public static InstructionUIManager instance;
    [SerializeField] private List<GameObject> instructionUIList = new List<GameObject>();  // 用於在Inspector拖曳進去
    private Dictionary<InstructionTypeEnum, GameObject> availableInstructionsUIObj = new Dictionary<InstructionTypeEnum, GameObject>();
    [SerializeField] public GameObject currentInstructionUIObj = null;

    [SerializeField] private float maskGrowDuration = 1.5f;
    [SerializeField] private Ease maskGrowEaseType = Ease.InOutExpo;
    [SerializeField] private float maskShrinkDuration = 1.5f;
    [SerializeField] private Ease maskShrinkEaseType = Ease.InOutExpo;


    [SerializeField] private float keyCodeVisualDisplayAnimScale = 1.1f;
    [SerializeField] private float keyCodeVisualDisplayAnimDuration = 1;
    [SerializeField] private Ease keyCodeDisplayAnimEaseType = Ease.InOutSine;


    public bool isProcessingDisappearUI = false;
    private Tweener maskGrowTweener = null;
    private Tweener maskShrinkTweener = null;

    [Header("Drag")]
    [SerializeField] private RebindingManager rebindingManager;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        /*
        maskMaxSizeDict.Add("MoveInstruction", new Vector2(370, 213));
        maskMaxSizeDict.Add("JumpInstruction", new Vector2(370, 213));
        maskMaxSizeDict.Add("ShootFireballInstruction_1", new Vector2(700, 300));
        maskMaxSizeDict.Add("ShootFireballInstruction_2", new Vector2(477.5f, 269.4f));
        */

        foreach (GameObject instructionUIObj in instructionUIList)
        {
            InstructionTypeEnum targetObj_currentInstructionType = instructionUIObj.GetComponent<InstructionUI>().getCurrentInstructionType();
            availableInstructionsUIObj[targetObj_currentInstructionType] = instructionUIObj;
        }

        


    }

    // Update is called once per frame
    void Update()
    {

    }

    #region FUNCTION: showInstructionUI()
    public void showInstructionUI(InstructionTypeEnum instructionType)  // move or jump
    {
        if (currentInstructionUIObj != null)
        {
            // 已有UI顯示中
            return;
        }
        if (maskGrowTweener != null)
        {
            Debug.Log("Already growing mask");
            return;
        }

        // 如果mask正在縮小則停止縮小
        if (maskShrinkTweener != null)
        {
            maskShrinkTweener.Kill();
            maskShrinkTweener = null;
            isProcessingDisappearUI = false;
        }

        if (availableInstructionsUIObj.ContainsKey(instructionType))
        {
            GameObject instructionUIPrefab = availableInstructionsUIObj[instructionType];
            instructionUIPrefab.GetComponent<InstructionUI>().manageChangingKeyDisplay();
            instructionUIPrefab.SetActive(true);  // 將UI變為可見
            currentInstructionUIObj = instructionUIPrefab;
            print("set current instructionUIObj horizontalLayoutChild obj");

            if (instructionUIPrefab.GetComponent<RectTransform>() != null)
            {
                #region Grow Mask
                RectTransform prefabRect = instructionUIPrefab.GetComponent<RectTransform>();
                Vector2 prefabPos = prefabRect.anchoredPosition;

                if (instructionUIPrefab.GetComponent<Mask>() == null)
                {
                    Debug.LogError("[showInstructionUI()]: No Mask component found in the given GameObject.");
                }


                Vector2 expandedMaskSize = availableInstructionsUIObj[instructionType].GetComponent<InstructionUI>().getMaskMaxSize(); // mask最大狀態
                UIPerforming.setUISize(instructionUIPrefab, new Vector2(prefabRect.sizeDelta.x, 0));  // 為了讓他可以從0到全展開，所以height先設為0

                // 這時才讓他可以顯示在Canvas內
                instructionUIPrefab.transform.SetParent(GameObject.FindGameObjectWithTag("InstructionUIManager").transform);
                prefabRect.anchoredPosition = prefabPos;

                maskGrowTweener = prefabRect.DOSizeDelta(expandedMaskSize, maskGrowDuration).SetEase(maskGrowEaseType);
                maskGrowTweener.onComplete = maskGrowTweenFinished;
                #endregion

                #region KeyCode Image Animation
                GameObject background = instructionUIPrefab.transform.GetChild(0).gameObject;
                GameObject horizontalLayout = background.transform.Find("KeyHorizontalLayout").gameObject;
                for (int i = 0; i < horizontalLayout.transform.childCount; i++)
                {
                    // 從i=1開始是因為第一個是ActionNameLabel，要跳過
                    // Instruction(即這裡的ui)
                    // -> Background
                    //    -> ActionNameLabel
                    //    -> ...(很多圖片)
                    GameObject horizontalLayoutChild = horizontalLayout.transform.GetChild(i).gameObject;
                    keyCodeVisualDisplayAnim(horizontalLayoutChild);
                };
                #endregion
            }
        }
        
    }


    #endregion

    #region FUNCTION: disappearInstructionUI()
    public void disappearInstructionUI()
    {
        /*
        if (isProcessingDisappearUI == true)
        {
            Debug.Log("Already disappearing");
            return;
        }
        */

        // 如果mask正在擴大，停止擴大
        if (maskGrowTweener != null)
        {
            maskGrowTweener.Kill();
            maskGrowTweener = null;
        }

        isProcessingDisappearUI = true;
        #region Shrink Mask
        if (currentInstructionUIObj == null)
        {
            Debug.LogError("[disappearInstructionUI]: No existing instructionUIObj UI to delete.");
            return;
        }
        RectTransform uiRect = currentInstructionUIObj.GetComponent<RectTransform>();
        Vector2 uiPos = uiRect.anchoredPosition;

        //RectTransform rectTransform = currentInstructionUIObj.GetComponent<RectTransform>();

        maskShrinkTweener = uiRect.DOSizeDelta(new Vector2(uiRect.sizeDelta.x, 0), maskShrinkDuration).SetEase(maskShrinkEaseType);
        maskShrinkTweener.onComplete = maskShrinkTweenFinished;


        #endregion
    }

    public void disappearInstructionUI(string instructionName)
    {
        /*
        if (isProcessingDisappearUI == true)
        {
            Debug.Log("Already disappearing");
            return;
        }
        */

        // 如果mask正在擴大，停止擴大
        if (maskGrowTweener != null)
        {
            maskGrowTweener.Kill();
            maskGrowTweener = null;
        }

        isProcessingDisappearUI = true;
        #region Shrink Mask
        if (currentInstructionUIObj == null)
        {
            Debug.LogError("[disappearInstructionUI]: No existing instructionUIObj UI to delete.");
            return;
        }
        RectTransform uiRect = currentInstructionUIObj.GetComponent<RectTransform>();
        Vector2 uiPos = uiRect.anchoredPosition;

        //RectTransform rectTransform = currentInstructionUIObj.GetComponent<RectTransform>();

        maskShrinkTweener = uiRect.DOSizeDelta(new Vector2(uiRect.sizeDelta.x, 0), maskShrinkDuration).SetEase(maskShrinkEaseType);
        maskShrinkTweener.onComplete = maskShrinkTweenFinished;


        #endregion
    }
    #endregion

    public void maskGrowTweenFinished()
    {
        maskGrowTweener.Kill();
        maskGrowTweener = null;
    }
    public void maskShrinkTweenFinished()
    {   
        maskShrinkTweener.Kill();
        maskShrinkTweener = null;
        currentInstructionUIObj.SetActive(false);
        currentInstructionUIObj = null;
        isProcessingDisappearUI = false;

    }

    #region FUNCTION: keyCodeImageAnim()
    public void keyCodeImageAnim(GameObject ui, float keyCodeImageAnimScale, float keyCodeImageAnimDuration, Ease keyCodeImageAnimEaseType)
    {
        if (ui.GetComponent<RectTransform>() != null)
        {
            ui.GetComponent<RectTransform>()
                .DOScale(keyCodeImageAnimScale, keyCodeImageAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)  // -1表示無限次
                .SetEase(keyCodeImageAnimEaseType);
        }
    }
    public void keyCodeVisualDisplayAnim(GameObject horizontalLayoutChild)
    {
        if (horizontalLayoutChild.GetComponent<RectTransform>() != null)
        {
            horizontalLayoutChild.GetComponent<RectTransform>()
                .DOScale(keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(keyCodeDisplayAnimEaseType);
            
            if (horizontalLayoutChild.name.IndexOf("KeyboardKeyImage") != -1)
            {
                // 是鍵盤按鍵圖片
                if (horizontalLayoutChild.transform.Find("KeyCodeText").gameObject.activeSelf == true)
                {
                    // 是文字
                    GameObject keyCodeText = horizontalLayoutChild.transform.Find("KeyCodeText").gameObject;
                    keyCodeText.GetComponent<RectTransform>()
                        .DOScale(keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(keyCodeDisplayAnimEaseType);
                }
                if (horizontalLayoutChild.transform.Find("KeyImage").gameObject.activeSelf == true)
                {
                    // 是圖片
                    GameObject keyImage = horizontalLayoutChild.transform.Find("KeyImage").gameObject;
                    keyImage.GetComponent<RectTransform>()
                        .DOScale(keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(keyCodeDisplayAnimEaseType);
                }

            }

            else
            {
                // 不是鍵盤按鍵
                horizontalLayoutChild.GetComponent<RectTransform>()
                        .DOScale(keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(keyCodeDisplayAnimEaseType);
            }
        }
    }
    public void changeAction_ShootFireball_OneKey(ActionsEnum newAction)
    {
        GameObject instructionUIPrefab = availableInstructionsUIObj[InstructionTypeEnum.ShootFireball_OneKey];
        instructionUIPrefab.GetComponent<InstructionUI>().changeAction_ShootFireball_OneKey(newAction);
    }

    public void changeAction_ShootFireball_TwoKey(ActionsEnum newFirstAction, ActionsEnum newSecondAction)
    {
        GameObject instructionUIPrefab = availableInstructionsUIObj[InstructionTypeEnum.ShootFireball_OneKey];
        instructionUIPrefab.GetComponent<InstructionUI>().changeAction_ShootFireball_TwoKey(newFirstAction, newSecondAction);
    }


}

    #endregion







#if UNITY_EDITOR

    [CustomEditor(typeof(InstructionUIManager))]
    public class InstructionManagerCustomInspector : Editor
    {

        InstructionUIManager instructionManager;
        public GameObject tesUI;

        private void OnEnable()
        {
            instructionManager = (InstructionUIManager)target;


        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();



            if (GUILayout.Button("Grow Mask", GUILayout.Width(180f)))
            {
                instructionManager.showInstructionUI(InstructionTypeEnum.Move);
            }
        }
    }

#endif
