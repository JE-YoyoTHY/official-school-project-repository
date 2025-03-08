using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;

public class InstructionManager : MonoBehaviour
{
    public Transform instructionCanvasTransform;
    public string currentInstruction { get; private set; }
    public List<GameObject> prefabList = new List<GameObject>();  // �Ω�bInspector�즲�i�h


    // string:�ӫ��ܪ��W��; GameObject:�ϥܪ�panel(prefab)
    public Dictionary<string, GameObject> availablePrefabs = new Dictionary<string, GameObject>();

    [SerializeField] private float showDuration;
    [SerializeField] private Ease showEaseType;
    [SerializeField] private float showBackgroundAlpha;
    [SerializeField] private float disappearDuration;
    [SerializeField] private Ease disappearEaseType;

    [SerializeField] private float maskGrowDuration = 1.5f;
    [SerializeField] private Ease maskGrowEaseType = Ease.InOutExpo;

    [SerializeField] private float keyCodeImageAnimScale = 1.1f;
    [SerializeField] private float keyCodeImageAnimDuration = 1;
    [SerializeField] private Ease keyCodeImageAnimEaseType = Ease.InOutSine;

    #region Shader Ref
    public Shader dissolveShader;
    #endregion



        
    void Start()
    {
        foreach (GameObject prefab in prefabList)
        {
            availablePrefabs[prefab.name] = prefab;
        }

        


    }

    // Update is called once per frame
    void Update()
    {

    }

    #region FUNCTION: showImagePanel()
    public GameObject showImagePanel(string prefabName, float duration, Ease easeType, float backgroundAlpha)
    {
        if (availablePrefabs.ContainsKey(prefabName))
        {
            GameObject background = Instantiate(availablePrefabs[prefabName]);
            Image imageComponent = background.GetComponent<Image>();
            UIPerforming.setImageTransparency(imageComponent, 0);  // �����L�ܥ��z��


            /* �� DissolveShader �ɶ}��
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
        else { Debug.LogError("[showImagePanel]: prefabName not match."); return null; } 
    }

    public GameObject showImagePanel(string prefabName)
    {
        float duration = showDuration;
        Ease easeType = showEaseType;
        if (availablePrefabs.ContainsKey(prefabName))
        {
            GameObject imagePanel = Instantiate(availablePrefabs[prefabName]);
            Image imageComponent = imagePanel.GetComponent<Image>();
            UIPerforming.setImageTransparency(imageComponent, 0);  // �����L�ܥ��z��


            /* �� DissolveShader �ɶ}��
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
        else { Debug.LogError("[showImagePanel]: prefabName not match."); return null; }
    }
    #endregion

    #region FUNCTION: showInstructionUI()
    public void showInstructionUI(string prefabName, Vector2 expandedMaskSize, float maskGrowDuration, Ease maskGrowEaseType)
    {
        
        if (availablePrefabs.ContainsKey(prefabName))
        {
            
            GameObject ui = Instantiate(availablePrefabs[prefabName]);

            if (ui.GetComponent<RectTransform>() != null)
            {
                #region Grow Mask
                RectTransform uiRect = ui.GetComponent<RectTransform>();
                Vector2 uiPos = uiRect.anchoredPosition;

                if (ui.GetComponent<Mask>() == null)
                {
                    Debug.LogError("[showInstructionUI()]: No Mask component found in the given GameObject.");
                }

                RectTransform rectTransform = ui.GetComponent<RectTransform>();
                UIPerforming.setUISize(ui, new Vector2(rectTransform.sizeDelta.x, 0));  // ���F���L�i�H�q0����i�}�A�ҥHheight���]��0

                // �o�ɤ~���L�i�H��ܦbCanvas��
                ui.transform.SetParent(GameObject.FindGameObjectWithTag("InstructionCanvas").transform);
                uiRect.anchoredPosition = uiPos;

                rectTransform.DOSizeDelta(expandedMaskSize, maskGrowDuration).SetEase(maskGrowEaseType);
                #endregion

                #region KeyCode Image Animation
                keyCodeImageAnim(ui.transform.GetChild(0).transform.GetChild(1).gameObject);  // Mask -> BackGround -> KeyCodeImage
                #endregion
            }

        }
        
    }

    public void showInstructionUI(string prefabName)
    {
        if (availablePrefabs.ContainsKey(prefabName))
        {
            GameObject ui = Instantiate(availablePrefabs[prefabName]);

            if (ui.GetComponent<RectTransform>() != null)
            {
                #region Grow Mask
                RectTransform uiRect = ui.GetComponent<RectTransform>();
                Vector2 uiPos = uiRect.anchoredPosition;

                if (ui.GetComponent<Mask>() == null)
                {
                    Debug.LogError("[showInstructionUI()]: No Mask component found in the given GameObject.");
                }

                RectTransform rectTransform = ui.GetComponent<RectTransform>();
                Vector2 expandedMaskSize = rectTransform.sizeDelta;
                UIPerforming.setUISize(ui, new Vector2(rectTransform.sizeDelta.x, 0));  // ���F���L�i�H�q0����i�}�A�ҥHheight���]��0

                // �o�ɤ~���L�i�H��ܦbCanvas��
                ui.transform.SetParent(GameObject.FindGameObjectWithTag("InstructionCanvas").transform);
                uiRect.anchoredPosition = uiPos;

                rectTransform.DOSizeDelta(expandedMaskSize, maskGrowDuration).SetEase(maskGrowEaseType);
                #endregion

                #region KeyCode Image Animation
                keyCodeImageAnim(ui.transform.GetChild(0).transform.GetChild(1).gameObject);  // Mask -> BackGround -> KeyCodeImage
                #endregion
            }
        }
        
    }
    #endregion

    #region FUNCTION: keyCodeImageAnim()
    public void keyCodeImageAnim(GameObject ui, float keyCodeImageAnimScale, float keyCodeImageAnimDuration, Ease keyCodeImageAnimEaseType)
    {
        if (ui.GetComponent<RectTransform>() != null)
        {
            ui.GetComponent<RectTransform>()
                .DOScale(keyCodeImageAnimScale, keyCodeImageAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)  // -1��ܵL����
                .SetEase(keyCodeImageAnimEaseType);
        }
    }
    public void keyCodeImageAnim(GameObject ui)
    {
        if (ui.GetComponent<RectTransform>() != null)
        {
            ui.GetComponent<RectTransform>()
                .DOScale(keyCodeImageAnimScale, keyCodeImageAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(keyCodeImageAnimEaseType);
        }
    }

    #endregion




#if UNITY_EDITOR

    [CustomEditor(typeof(InstructionManager))]
    public class InstructionManagerCustomInspector : Editor
    {

        InstructionManager instructionManager;
        public GameObject tesUI;

        private void OnEnable()
        {
            instructionManager = (InstructionManager)target;


        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();



            if (GUILayout.Button("Grow Mask", GUILayout.Width(180f)))
            {
                instructionManager.showInstructionUI("MoveInstruction_Mask");
            }
        }
    }

#endif








}
