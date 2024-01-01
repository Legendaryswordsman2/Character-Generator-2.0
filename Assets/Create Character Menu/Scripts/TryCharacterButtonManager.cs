using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryCharacterButtonManager : MonoBehaviour
{
    [SerializeField] float moveAllUIOffScreenTime = 0.35f;
    [SerializeField] float zoomBackgroundTime = 0.5f;
    //[SerializeField] float delayBeforeTransition = 0.35f;

    //public static event EventHandler<float> OnBeforeTryCharacterSceneLoaded;

    public static event EventHandler<float> MoveAllUIOffScreen;
    public static event EventHandler<float> ZoomBackground;
    public void LoadTryCharacterScene()
    {
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            //OnBeforeTryCharacterSceneLoaded?.Invoke(this, delayBeforeTransition);

            MoveAllUIOffScreen?.Invoke(this, moveAllUIOffScreenTime);
            yield return new WaitForSeconds(moveAllUIOffScreenTime);
            ZoomBackground?.Invoke(this, zoomBackgroundTime);
            yield return new WaitForSeconds(zoomBackgroundTime);
            SceneManager.LoadScene(1);
        }

    }
}