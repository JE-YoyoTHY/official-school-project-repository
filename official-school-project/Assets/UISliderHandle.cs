using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISliderHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UISliderManager uiSliderManager;
    private void Awake()
    {
        if (uiSliderManager == null)
        {
            uiSliderManager = transform.parent.parent.GetComponent<UISliderManager>();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiSliderManager.onPointerEnterHandle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiSliderManager.onPointerExitHandle();
    }
}
