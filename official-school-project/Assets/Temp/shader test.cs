using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class shadertest : MonoBehaviour
{
    private string FADE_REF = "_Fade";
    public Material dissolveMaterial;
    // Start is called before the first frame update
    void Start()
    {
        dissolveMaterial = GetComponent<SpriteRenderer>().material;
        dissolveMaterial.DOFloat(1f, FADE_REF, 4).SetEase(Ease.OutBack);  // fade to 1 within 4 secs

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
