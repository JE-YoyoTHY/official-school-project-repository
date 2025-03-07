using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using DG.Tweening;

public class InstructionManager : MonoBehaviour
{
    public Transform instructionCanvasTransform;
    public string currentInstruction { get; private set; }
    public List<GameObject> prefabList = new List<GameObject>();  // 用於在Inspector拖曳進去

    // string:該指示的名稱; GameObject:圖示的panel(prefab)
    public Dictionary<string, GameObject> availabelImagePanels = new Dictionary<string, GameObject>();

    [SerializeField] private float showDuration;
    [SerializeField] private Ease showEaseType;
    [SerializeField] private float disappearDuration;
    [SerializeField] private Ease disappearEaseType;

    #region Shader Ref
    public Shader dissolveShader;
    #endregion



        
    void Start()
    {
        foreach (GameObject prefab in prefabList)
        {
            availabelImagePanels[prefab.name] = prefab;
        }

        


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            showImagePanel("TextPanel");
        }
    }

    #region FUNCTION: showImagePanel()
    public GameObject showImagePanel(string prefabName, float duration, Ease easeType)
    {
        if (availabelImagePanels.ContainsKey(prefabName))
        {
            GameObject imagePanel = Instantiate(availabelImagePanels[prefabName]);
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

            imageComponent.DOFade(1, duration).SetEase(easeType);

            return imagePanel;
        }
        else { Debug.LogError("[showImagePanel]: prefabName not match."); return null; } 
    }

    public GameObject showImagePanel(string prefabName)
    {
        float duration = showDuration;
        Ease easeType = showEaseType;
        if (availabelImagePanels.ContainsKey(prefabName))
        {
            GameObject imagePanel = Instantiate(availabelImagePanels[prefabName]);
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

            imageComponent.DOFade(1, duration).SetEase(easeType);

            return imagePanel;
        }
        else { Debug.LogError("[showImagePanel]: prefabName not match."); return null; }
    }
    #endregion








}
