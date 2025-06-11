using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StudioPresent : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease _ease;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            appear();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            disappear();
        }
    }

    public void appear()
    {
        gameObject.GetComponent<Image>().DOFade(1, duration);
        gameObject.GetComponent<Image>().raycastTarget = true;
        foreach (GameObject _obj in GameObjectMethods.GetAllChildren(gameObject))
        {
            _obj.GetComponent<Image>().DOFade(1, duration);
            if (_obj.GetComponent<TextMeshProUGUI>())
            {
                _obj.GetComponent<TextMeshProUGUI>().DOFade(1, duration);
            }
        }
    }

    public void disappear()
    {
        gameObject.GetComponent<Image>().DOFade(0, duration);
        gameObject.GetComponent<Image>().raycastTarget = false;
        foreach (GameObject _obj in GameObjectMethods.GetAllChildren(gameObject))
        {
            _obj.GetComponent<Image>().DOFade(0, duration);
            if (_obj.GetComponent<TextMeshProUGUI>())
            {
                _obj.GetComponent<TextMeshProUGUI>().DOFade(0, duration);
            }
        }
    }

}
