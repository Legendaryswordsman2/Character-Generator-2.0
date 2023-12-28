using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSyncManager : MonoBehaviour
{
    private void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
}