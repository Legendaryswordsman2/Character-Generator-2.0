using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryCharacterLeaveManager : MonoBehaviour
{
    public static event EventHandler<float> OnBeforeSceneChanged;

    bool switchingScenes = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !switchingScenes)
        {
            switchingScenes = true;
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                OnBeforeSceneChanged?.Invoke(this, 0.5f);
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(0);
            }
        }
    }
}