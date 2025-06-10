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
    [SerializeField] private float duration;

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
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            applyVFX();
        }
    }

    public void applyVFX()
    {
        m_volume.enabled = true;
        if (currentVFXType == VFXType.ThunderBloom)
        {
            Vector3 marginFront = getMainCameraPositionXY() + margin;
            Vector3 marginBack = getMainCameraPositionXY() - margin;
            gameObject.transform.position = marginFront;
            gameObject.transform.DOMove(marginBack, duration);
        }

    }

    public Vector3 getMainCameraPositionXY()
    {
        Vector3 _pos = mainCam.transform.position;
        _pos.z = 0;
        return mainCam.transform.position;
    }


}
