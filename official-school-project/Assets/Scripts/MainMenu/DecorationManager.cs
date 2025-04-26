using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DecorationManager : MonoBehaviour
{
    public enum MainMenuDecorationOptions
    {
        _None, 
        PlaySelection, 
        SettingSelection, 
        CreditSelection
    }

    private GameObject currentDecorationObj;
    private MainMenuDecorationOptions currentDecorationEnum;
    private Tweener currentTweener;
    private float maxRadius = 1.5f;

    [Header("Colorize")]
    [SerializeField] private float colorizeDuration = 1;
    [SerializeField] private Ease colorizeEase = Ease.Linear;

    [Header("Materials")]
    [SerializeField] private Material grayToColored_PlaySelection;

    [Header("Decorations")]
    [SerializeField] private GameObject DecorPlaySelection;

    private Dictionary< MainMenuDecorationOptions, Tuple<GameObject, Material> > decorationDict;
    // Start is called before the first frame update
    private void Awake()
    {
        decorationDict = new Dictionary< MainMenuDecorationOptions, Tuple<GameObject, Material> >
        {
            {MainMenuDecorationOptions.PlaySelection, new Tuple<GameObject, Material>(DecorPlaySelection, grayToColored_PlaySelection) }
        };
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeDecoration(MainMenuDecorationOptions whichDecoratoin)
    {
        currentDecorationObj.SetActive(false);
        currentDecorationEnum = whichDecoratoin;
        currentDecorationObj = decorationDict[whichDecoratoin].Item1;
    }

    public void performDecorationColorize()
    {
        Material currentMAT = decorationDict[currentDecorationEnum].Item2;
        currentMAT.SetFloat("_Mode", 1f);  // colorize mode, shows color when radius > 0
        currentTweener = currentMAT.DOFloat(maxRadius, "_Radius", colorizeDuration).SetEase(colorizeEase);
    }

    public void performDecorationToGray()
    {
        Material currentMAT = decorationDict[currentDecorationEnum].Item2;
        currentTweener.Kill();
        currentMAT.SetFloat("_Mode", 0f);  // gray mode
        currentMAT.SetFloat("_Radius", 0f);  // prepare for future use

        if (currentDecorationEnum == MainMenuDecorationOptions.PlaySelection)
        {
            DecorationCloudSpawner decorCloudSpawner = transform.Find("PlaySelection").transform.GetComponent<DecorationCloudSpawner>();
            decorCloudSpawner.canSpawn = false;
            decorCloudSpawner.cloudSpeedMultiplier = 0f;  // stop the clouds from moving
        }
    }

    public void performImmersiveDecoration()
    {
        if (currentDecorationEnum == MainMenuDecorationOptions.PlaySelection)
        {
            DecorationCloudSpawner decorCloudSpawner = transform.Find("PlaySelection").transform.GetComponent<DecorationCloudSpawner>();
            decorCloudSpawner.canSpawn = true;
            decorCloudSpawner.cloudSpeedMultiplier = 1f;  // make the clouds move again
        }
    }
}
