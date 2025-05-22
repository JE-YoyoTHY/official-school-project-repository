using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using Unity.VisualScripting;
using TMPro;

public class InstructionUIManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> instructionUIList = new List<GameObject>();  // 用於在Inspector拖曳進去
    private Dictionary<string, GameObject> availableInstructionsUIObj = new Dictionary<string, GameObject>();
    private Dictionary<string, Vector2> maskMaxSizeDict = new Dictionary<string, Vector2>();
    [SerializeField] public GameObject currentInstructionUIObj = null;

    #region Deprecated Ref

    [SerializeField] private float showDuration;
    [SerializeField] private Ease showEaseType;
    [SerializeField] private float showBackgroundAlpha;
    [SerializeField] private float disappearDuration;
    [SerializeField] private Ease disappearEaseType;

    public Shader dissolveShader;
    #endregion

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
            availableInstructionsUIObj[instructionUIObj.name] = instructionUIObj;
            maskMaxSizeDict[instructionUIObj.name] = instructionUIObj.GetComponent<InstructionUI>().getMaskMaxSize();
        }

        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region DEPRECATED FUNCTION: showImagePanel()
    public GameObject showImagePanel(string prefabName, float duration, Ease easeType, float backgroundAlpha)
    {
        if (availableInstructionsUIObj.ContainsKey(prefabName))
        {
            GameObject background = Instantiate(availableInstructionsUIObj[prefabName]);
            Image imageComponent = background.GetComponent<Image>();
            UIPerforming.setImageTransparency(imageComponent, 0);  // 先讓他變全透明


            /* 用 DissolveShader 時開啟
            Material imagePanelMaterial = imagePanel.GetComponent<Image>().material;
            if (imagePanelMaterial.shader == dissolveShader)
            {
                print("found shader");
                imagePanelMaterial.SetFloat("_Fade", 0);
                imagePanelMaterial.DOFloat(1, "_Fade", duration).SetEase(easeType);
                print("tween finish");
            }
            */
            if (backgroundAlpha < 0 || backgroundAlpha > 1) { Debug.LogError("[showImagePanel()]: alpha out of range, it should be between 0 to 1."); }
            imageComponent.DOFade(backgroundAlpha, duration).SetEase(easeType);

            return background;
        }
        else { Debug.LogError("[showImagePanel]: instructionName not match."); return null; } 
    }

    public GameObject showImagePanel(string prefabName)
    {
        float duration = showDuration;
        Ease easeType = showEaseType;
        if (availableInstructionsUIObj.ContainsKey(prefabName))
        {
            GameObject imagePanel = Instantiate(availableInstructionsUIObj[prefabName]);
            Image imageComponent = imagePanel.GetComponent<Image>();
            UIPerforming.setImageTransparency(imageComponent, 0);  // 先讓他變全透明


            /* 用 DissolveShader 時開啟
            Material imagePanelMaterial = imagePanel.GetComponent<Image>().material;
            if (imagePanelMaterial.shader == dissolveShader)
            {
                print("found shader");
                imagePanelMaterial.SetFloat("_Fade", 0);
                imagePanelMaterial.DOFloat(1, "_Fade", duration).SetEase(easeType);
                print("tween finish");
            }
            */

            imageComponent.DOFade(showBackgroundAlpha, duration).SetEase(easeType);

            return imagePanel;
        }
        else { Debug.LogError("[showImagePanel]: instructionName not match."); return null; }
    }
    #endregion

    #region FUNCTION: showInstructionUI()
    public void showInstructionUI(string instructionName, Vector2 expandedMaskSize, float maskGrowDuration, Ease maskGrowEaseType)
    {
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

        if (availableInstructionsUIObj.ContainsKey(instructionName))
        {

            GameObject ui = availableInstructionsUIObj[instructionName];
            ui.SetActive(true);  // 將UI變為可見
            currentInstructionUIObj = ui;

            if (ui.GetComponent<RectTransform>() != null)
            {
                #region Grow Mask
                RectTransform uiRect = ui.GetComponent<RectTransform>();
                Vector2 uiPos = uiRect.anchoredPosition;  // = local pos

                if (ui.GetComponent<Mask>() == null)
                {
                    Debug.LogError("[showInstructionUI()]: No Mask component found in the given GameObject.");
                }

                //RectTransform rectTransform = ui.GetComponent<RectTransform>();
                UIPerforming.setUISize(ui, new Vector2(uiRect.sizeDelta.x, 0));  // 為了讓他可以從0到全展開，所以height先設為0

                // 這時才讓他可以顯示在Canvas內
                ui.transform.SetParent(GameObject.FindGameObjectWithTag("InstructionCanvas").transform);
                uiRect.anchoredPosition = uiPos;

                maskGrowTweener = uiRect.DOSizeDelta(expandedMaskSize, maskGrowDuration).SetEase(maskGrowEaseType);
                maskGrowTweener.onComplete = maskGrowTweenFinished;
                #endregion

                #region KeyCode Image Animation
                keyCodeVisualDisplayAnim(ui.transform.GetChild(0).transform.GetChild(1).gameObject);  // Mask -> BackGround -> KeyCodeImage
                #endregion
            }

        }
        
    }

    public void showInstructionUI(string instructionName)
    {
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

        if (availableInstructionsUIObj.ContainsKey(instructionName))
        {
            GameObject ui = availableInstructionsUIObj[instructionName];
            ui.SetActive(true);  // 將UI變為可見
            currentInstructionUIObj = ui;
            print("set current instructionUIObj ui obj");

            if (ui.GetComponent<RectTransform>() != null)
            {
                #region Grow Mask
                RectTransform uiRect = ui.GetComponent<RectTransform>();
                Vector2 uiPos = uiRect.anchoredPosition;

                if (ui.GetComponent<Mask>() == null)
                {
                    Debug.LogError("[showInstructionUI()]: No Mask component found in the given GameObject.");
                }


                Vector2 expandedMaskSize = maskMaxSizeDict[instructionName];  // mask最大狀態
                UIPerforming.setUISize(ui, new Vector2(uiRect.sizeDelta.x, 0));  // 為了讓他可以從0到全展開，所以height先設為0

                // 這時才讓他可以顯示在Canvas內
                ui.transform.SetParent(GameObject.FindGameObjectWithTag("InstructionCanvas").transform);
                uiRect.anchoredPosition = uiPos;

                maskGrowTweener = uiRect.DOSizeDelta(expandedMaskSize, maskGrowDuration).SetEase(maskGrowEaseType);
                maskGrowTweener.onComplete = maskGrowTweenFinished;
                #endregion

                #region KeyCode Image Animation
                for (int i=1; i< ui.transform.GetChild(0).transform.childCount; i++)
                {
                    // 從i=1開始是因為第一個是ActionNameLabel，要跳過
                    // Instruction(即這裡的ui)
                    // -> Background
                    //    -> ActionNameLabel
                    //    -> ...(很多圖片)
                    keyCodeVisualDisplayAnim(ui.transform.GetChild(0).transform.GetChild(i).transform.gameObject);
                };
                #endregion
            }
        }
        
    }
    #endregion

    #region FUNCTION: disappearInstructionUI()
    public void disappearInstructionUI()
    {
        if (isProcessingDisappearUI == true)
        {
            Debug.Log("Already disappearing");
            return;
        }

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
        if (isProcessingDisappearUI == true)
        {
            Debug.Log("Already disappearing");
            return;
        }

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
    public void keyCodeVisualDisplayAnim(GameObject ui)
    {
        if (ui.GetComponent<RectTransform>() != null)
        {
            ui.GetComponent<RectTransform>()
                .DOScale(keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(keyCodeDisplayAnimEaseType);

            // 讓圖片底下的文字也會一起改變大小
            if (ui.transform.childCount > 0 && ui.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshProUGUI>() != null)
            {
                TextMeshProUGUI m_tmp = ui.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshProUGUI>();
                DOTween.To
                (
                    () => m_tmp.fontSize,
                    x => m_tmp.fontSize = x, m_tmp.fontSize * keyCodeVisualDisplayAnimScale, keyCodeVisualDisplayAnimDuration
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(keyCodeDisplayAnimEaseType);
                
            }
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
                instructionManager.showInstructionUI("MoveInstruction");
            }
        }
    }

#endif








}
