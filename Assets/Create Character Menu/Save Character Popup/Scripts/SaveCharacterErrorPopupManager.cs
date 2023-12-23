using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveCharacterErrorPopupManager : MonoBehaviour
{
    [SerializeField] SaveCharacterManager saveCharacterManager;

    [Space]

    [SerializeField] TMP_Text errorText;

    private void Awake()
    {
        saveCharacterManager.OnSpriteMissingErrorTriggered += OnSpriteMissingErrorTriggered;
        saveCharacterManager.OnPopupOpened += SaveCharacterManager_OnPopupOpened;

        gameObject.SetActive(false);
    }

    private void SaveCharacterManager_OnPopupOpened(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void OnSpriteMissingErrorTriggered(object sender, string errorMessage)
    {
        errorText.text = errorMessage;

        transform.localScale = Vector2.zero;

        gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, 0.15f);
    }

    private void OnDestroy()
    {
        saveCharacterManager.OnSpriteMissingErrorTriggered -= OnSpriteMissingErrorTriggered;
        saveCharacterManager.OnPopupOpened -= SaveCharacterManager_OnPopupOpened;
    }
}