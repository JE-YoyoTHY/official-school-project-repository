using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class VisualEffectManager : MonoBehaviour
{
    public enum VFXType
    {
        ThunderBloom
    }
    [Header("Fill In")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private VFXType currentVFXType;
    [SerializeField] private Volume m_volume;
    [SerializeField] private Vector3 margin;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    private Tweener currentTweener;

    // Start is called before the first frame update
    private void Awake()
    {
        if (m_volume == null)
        {
            Debug.LogWarning("Found no m_volume");
        }
        else 
        {
            m_volume.enabled = false;
        }
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void applyVFX()
    {
        print("apply vfx");
        forceEndTween();
        m_volume.enabled = true;
        if (currentVFXType == VFXType.ThunderBloom)
        {
            Vector3 marginFront = getMainCameraPositionXY() + margin;
            Vector3 marginBack = getMainCameraPositionXY() - margin;
            gameObject.transform.position = marginFront;
            currentTweener = gameObject.transform.DOMove(marginBack, _duration)
                .SetEase(_ease);
            currentTweener.onComplete = VFXCompleted;
            print("into vfx");
        }
        print("vfx end");
    }

    public void forceEndTween()
    {
        if (currentTweener != null)
        {
            currentTweener.Kill();
        }
    }

    public Vector3 getMainCameraPositionXY()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
        Vector3 _pos = mainCam.transform.position;
        _pos.z = 0;
        
        return _pos;
    }

    public void VFXCompleted()
    {
        if (currentTweener != null)
        {
            currentTweener.Kill();
        }
        m_volume.enabled = false;
    }
}
