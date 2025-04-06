using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BindingChangedEventBroadcaster : MonoBehaviour
{
    public UnityEvent BindingChangedEvent;

    public void broadCast()
    {
        BindingChangedEvent.Invoke();
    }


}
