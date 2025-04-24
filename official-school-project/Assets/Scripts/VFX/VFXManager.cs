using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VFXManager : MonoBehaviour
{
    public enum VFXOptions
    {
        _None, 
        SpriteShockWave, 
    }
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mPB;
    private Material mat;
    private Shader m_shader;


    [Header("Drag-Needed")]
    [SerializeField] private VFXOptions currentVFX;
    [SerializeField] private float shockWaveDuration = 3.0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mPB = new MaterialPropertyBlock();
        mat = spriteRenderer.material;
        m_shader = mat.shader;
    }
    void Start()
    {
        print(m_shader.name);
        print(mat.HasFloat("_Radius"));
        print(mat.GetFloat("_Radius"));
    }

    void Update()
    {
        
    }

    [ContextMenu("Perform VFX")]
    public void performVFX()
    {


        if (mat == null || spriteRenderer == null)
        {
            Debug.LogError("component null");
            return;
        }

        if (currentVFX == VFXOptions.SpriteShockWave)
        {
            //print("mat has radius:");
            //print(mat.HasFloat("_Radius"));
            //print(mat.GetFloat("_Radius"));
            mat.DOFloat(0.7f, "_Radius", shockWaveDuration)
                .SetEase(Ease.Linear)
                .onComplete = VFXReset;
            
        }
    }

    private void VFXReset()
    {
        if (currentVFX == VFXOptions.SpriteShockWave)
        {
            mat.SetFloat("_Radius", 0.0f);
        }
        print("Completed VFX");
    }
}
