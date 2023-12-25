using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryTabCharacterPreview : MonoBehaviour
{
    [SerializeReference] CharacterTypeSO.CharacterBackup characterBackup;

    [Space]

    [SerializeField] HistoryTabManager historyTabManager;

    public void SetCharacterBackup(CharacterTypeSO.CharacterBackup backup)
    {
        characterBackup = backup;
    }
    public void SelectCharacterPreview()
    {
        if (characterBackup != null)
            historyTabManager.LoadPreviousCharacter(characterBackup);
    }
}