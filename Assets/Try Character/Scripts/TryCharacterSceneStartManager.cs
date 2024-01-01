using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TryCharacterSceneStartManager : MonoBehaviour
{
    [SerializeField] Image backgroundFadeOut;
    [SerializeField] CanvasGroup backgroundFadeOutCanvasGroup;

    [Space]

    [SerializeField] Image backgroundFadeIn;
    [SerializeField] CanvasGroup backgroundFadeInCanvasGroup;

    Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;

        backgroundFadeOut.enabled = true;

        LeanTween.alphaCanvas(backgroundFadeOutCanvasGroup, 0, 0.25f);

        mainCamera.orthographicSize = 0.2f;

        LeanTween.value(mainCamera.orthographicSize, 6.5f, 0.5f).setOnUpdate((float val) =>
        {
            mainCamera.orthographicSize = val;
        });

        TryCharacterLeaveManager.OnBeforeSceneChanged += TryCharacterLeaveManager_OnBeforeSceneChanged;
    }

    private void TryCharacterLeaveManager_OnBeforeSceneChanged(object sender, float time)
    {
        //StartCoroutine(Delay());
        //IEnumerator Delay()
        //{
            LeanTween.value(mainCamera.orthographicSize, 0.2f, time).setOnUpdate((float val) =>
            {
                mainCamera.orthographicSize = val;
            });

            //yield return new WaitForSeconds(time / 2);

            backgroundFadeIn.enabled = true;

            LeanTween.alphaCanvas(backgroundFadeInCanvasGroup, 1, time);
        //}
    }

    private void OnDestroy()
    {
        TryCharacterLeaveManager.OnBeforeSceneChanged -= TryCharacterLeaveManager_OnBeforeSceneChanged;
    }
}