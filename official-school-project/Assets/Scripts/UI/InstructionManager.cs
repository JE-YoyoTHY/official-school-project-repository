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

    #region Shader Ref
    public Shader dissolveShader;
    #endregion

    // string:該指示的名稱; GameObject:圖示的panel(prefab)
    public Dictionary<string, GameObject> availabelImagePanels = new Dictionary<string, GameObject>();

        
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
            print("R");
            showImagePanel("TextPanel");
        }
    }

    public void showImagePanel(string prefabName, float duration = 1, Ease easeType = Ease.OutBack)
    {
        print("enter showimagepanel.");
        if (availabelImagePanels.ContainsKey(prefabName))
        {
            print("contain key");
            GameObject imagePanel = Instantiate(availabelImagePanels[prefabName]);
            imagePanel.transform.SetParent(instructionCanvasTransform, false);

            Material imagePanelMaterial = imagePanel.GetComponent<Image>().material;
            if (imagePanelMaterial.shader == dissolveShader)
            {
                print("found shader");
                imagePanelMaterial.SetFloat("_Fade", 0);
                imagePanelMaterial.DOFloat(1, "_Fade", duration).SetEase(easeType);
                print("tween finish");
            }
        }
        else { Debug.LogError("[showImagePanel]: prefabName not match."); }

        
    }



}
