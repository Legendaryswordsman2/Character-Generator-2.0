using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptInitializer : MonoBehaviour
{
    [SerializeField] UnityEvent OnStartEvent;
    [SerializeField] UnityEvent OnDestroyEvent;
    private void Start()
    {
        OnStartEvent.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyEvent.Invoke();
    }
}