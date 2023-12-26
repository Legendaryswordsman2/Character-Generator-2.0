using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryTabCharacterPreview : MonoBehaviour
{
    [SerializeField] GameObject characterPreviewSprite;
    [SerializeField] HistoryTabCharacterPreviewButton button;

    [Space]

    [SerializeReference] CharacterTypeSO.CharacterBackup characterBackup;

    [Space]

    [SerializeField] HistoryTabManager historyTabManager;

    public void SetCharacterBackup(CharacterTypeSO.CharacterBackup backup)
    {
        characterBackup = backup;
        characterPreviewSprite.SetActive(true);
        button.enabled = true;
    }

    public void DIsableCharacterBackup()
    {
        characterPreviewSprite.SetActive(false);
        button.enabled = false;
    }
    public void SelectCharacterPreview()
    {
        if (characterBackup != null)
            historyTabManager.LoadPreviousCharacter(characterBackup);
    }
}