using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharacterBackgroundManager : MonoBehaviour
{
    Image image;
    private void Start()
    {
        image = GetComponent<Image>();

        TryCharacterButtonManager.ZoomBackground += TryCharacterButtonManager_ZoomBackground;
    }

    private void TryCharacterButtonManager_ZoomBackground(object sender, float time)
    {
        LeanTween.value(image.pixelsPerUnitMultiplier, 0.01f, time).setOnUpdate((float val) =>
        {
            image.pixelsPerUnitMultiplier = val;
        });
    }

    private void OnDestroy()
    {
        TryCharacterButtonManager.ZoomBackground -= TryCharacterButtonManager_ZoomBackground;
    }
}