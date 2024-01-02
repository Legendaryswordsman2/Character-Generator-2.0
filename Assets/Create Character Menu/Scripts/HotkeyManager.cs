using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyManager : MonoBehaviour
{
    [SerializeField] SaveCharacterManager saveCharacterManager;
    [SerializeField] InfoMenuManager infoMenuManager;
    [SerializeField] RandomizeButton randomizeButton;

    private void Update()
    {
        if (!CharacterPieceGrabber.AllCharacterPiecesLoaded) return;

        if (Input.GetKeyDown(KeyCode.S) && !saveCharacterManager.gameObject.activeSelf)
            saveCharacterManager.OpenPopup();

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (infoMenuManager.gameObject.activeSelf)
                infoMenuManager.CloseMenu();
            else
                infoMenuManager.OpenMenu();
        }

        if (Input.GetKeyDown(KeyCode.R))
            randomizeButton.RandomizeCharacter();
    }
}