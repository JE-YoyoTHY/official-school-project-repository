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
        StartCoroutine(delayAndDisappear());
    }

    void Update()
    {

    }

    public void appear()
    {
        if (gameObject.GetComponent<Image>() != null)
        {
            gameObject.GetComponent<Image>().DOFade(1, duration);
            gameObject.GetComponent<Image>().raycastTarget = true;
        }
        foreach (GameObject _obj in GameObjectMethods.GetAllChildren(gameObject))
        {
            if (_obj.GetComponent<Image>() != null)
            {
                _obj.GetComponent<Image>().DOFade(1, duration);
                _obj.GetComponent<Image>().raycastTarget = true;
            }

            if (_obj.GetComponent<TextMeshProUGUI>())
            {
                _obj.GetComponent<TextMeshProUGUI>().DOFade(1, duration);
            }
        }
    }

    public void disappear()
    {
        if (gameObject.GetComponent<Image>() != null)
        {
            gameObject.GetComponent<Image>().DOFade(0, duration);
            gameObject.GetComponent<Image>().raycastTarget = false;
        }

        foreach (GameObject _obj in GameObjectMethods.GetAllChildren(gameObject))
        {
            if (_obj.GetComponent<Image>() != null)
            {
                _obj.GetComponent<Image>().DOFade(0, duration);
                _obj.GetComponent<Image>().raycastTarget = false;
            }

            if (_obj.GetComponent<TextMeshProUGUI>())
            {
                _obj.GetComponent<TextMeshProUGUI>().DOFade(0, duration);
            }
        }
    }

    IEnumerator delayAndDisappear()
    {
        yield return new WaitForSeconds(3);
        disappear();
    }

}
