using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectToRebindDataBase : MonoBehaviour
{
    public enum InjectTarget
    {
        Overlay
    }

    [SerializeField] private RebindSystemDataBase dataBase;
    [SerializeField] private InjectTarget whoAreYou;

    private void Awake()
    {
        switch(whoAreYou)
        {
            case InjectTarget.Overlay:
                dataBase.overlay = gameObject;
                break;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
