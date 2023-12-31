using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryCharacterLeaveManager : MonoBehaviour
{
    public static event EventHandler OnBeforeSceneChanged;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBeforeSceneChanged?.Invoke(this, EventArgs.Empty);
            SceneManager.LoadScene(0);
        }
    }
}