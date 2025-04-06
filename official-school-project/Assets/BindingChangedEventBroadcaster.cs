using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BindingChangedEventBroadcaster : MonoBehaviour
{
    public UnityEvent BindingChangedEvent;

    private void Awake()
    {
        BindingChangedEvent = new UnityEvent();
    }
    private void Start()
    {
        
    }

    public void broadCast()
    {
        BindingChangedEvent.Invoke();
    }


}
