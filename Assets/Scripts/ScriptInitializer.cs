using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptInitializer : MonoBehaviour
{
    [SerializeField] UnityEvent onStart;
    private void Start()
    {
        onStart.Invoke();
    }
}