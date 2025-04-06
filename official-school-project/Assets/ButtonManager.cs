using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public enum ButtonTypes
    {
        _None,
        RebindSwitch
    }

    [Header("Drag-Needed")]
    // buttons
    [SerializeField] private ButtonTypes whichButton;  // inspector

    // targets
    [SerializeField] private GameObject rebindCanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (whichButton == ButtonTypes._None) { Debug.LogError("ButtonType 未設置"); }
        if (rebindCanvas == null) { Debug.LogError("Rebind Canvas 未設置"); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonOnClick()
    {
        if (whichButton == ButtonTypes.RebindSwitch)
        {
            rebindCanvas.SetActive(!rebindCanvas.activeSelf);
        }
    }
}
