using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RebindingManager : MonoBehaviour
{
    public UnityEvent BindingChangedEvent;

    private void Awake()
    {

    }

    public void bindingChangedBroadcast()
    {
        BindingChangedEvent.Invoke();
    }



    
}
