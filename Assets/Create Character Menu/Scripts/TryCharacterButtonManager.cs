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
    public async void LoadTryCharacterScene()
    {
        MoveAllUIOffScreen?.Invoke(this, moveAllUIOffScreenTime);
        await UniTask.WaitForSeconds(moveAllUIOffScreenTime);
        ZoomBackground?.Invoke(this, zoomBackgroundTime);
        await UniTask.WaitForSeconds(zoomBackgroundTime);
        SceneManager.LoadScene(1);
    }
}