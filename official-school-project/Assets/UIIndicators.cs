using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIIndicators : MonoBehaviour
{
    public enum UIIndicatorsEnum
    {
        SelectionTriangle, 
    }

    public enum UIIndicatorReferencesEnum
    {
        playSelection, 
        settingSelection, 
        creditSelection
    } 
    [SerializeField] private UIIndicatorsEnum whichIndicator;

    [Header("SelectionTriangle - Selections")]
    [SerializeField] private GameObject playSelection;
    [SerializeField] private GameObject settingSelection;
    [SerializeField] private GameObject creditSelection;

    private void Awake()
    {
        if (whichIndicator == UIIndicatorsEnum.SelectionTriangle)
        {
            if (playSelection == null || settingSelection == null ||  creditSelection == null)
            {
                Debug.Log("Use selection triangle but one (or more) selection reference(s) not set.");
                return;
            }
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void showSelectionTriangle(UIIndicatorReferencesEnum reference)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 displacement;
        float margin = -20;
        

        if (reference == UIIndicatorReferencesEnum.playSelection)
        {
            displacement = new Vector2(-1.0f * playSelection.GetComponent<RectTransform>().sizeDelta.x / 2 + margin, 0);
            transform.position = playSelection.transform.position +  new Vector3(displacement.x, displacement.y, 0);
        }
        else if (reference == UIIndicatorReferencesEnum.settingSelection)
        {
            displacement = new Vector2(-1.0f * settingSelection.GetComponent<RectTransform>().sizeDelta.x / 2 + margin, 0);
            transform.position = settingSelection.transform.position + new Vector3(displacement.x, displacement.y, 0);
        }
        else if (reference == UIIndicatorReferencesEnum.creditSelection)
        {
            displacement = new Vector2(-1.0f * creditSelection.GetComponent<RectTransform>().sizeDelta.x / 2 + margin, 0);
            transform.position = creditSelection.transform.position + new Vector3(displacement.x, displacement.y, 0);
        }

        gameObject.SetActive(true);
    }

    public void disappear()
    {
        gameObject.SetActive(false);
    }
}
