using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotkeyManager : MonoBehaviour
{
    [SerializeField] SaveCharacterManager saveCharacterManager;
    [SerializeField] InfoMenuManager infoMenuManager;
    [SerializeField] RandomizeButton randomizeButton;
    [SerializeField] TryCharacterButtonManager tryCharacterButtonManager;

    private void Update()
    {
        if (!CharacterPieceGrabber.AllCharacterPiecesLoaded) return;

        if (Input.GetKeyDown(KeyCode.S) && !saveCharacterManager.gameObject.activeSelf && !infoMenuManager.gameObject.activeSelf)
            saveCharacterManager.OpenPopup();

        if (Input.GetKeyDown(KeyCode.I) && !saveCharacterManager.gameObject.activeSelf)
        {
            if (infoMenuManager.gameObject.activeSelf)
                infoMenuManager.OnCloseMenuCalled();
            else
                infoMenuManager.OpenMenu();
        }

        if (Input.GetKeyDown(KeyCode.R) && !saveCharacterManager.gameObject.activeSelf && !infoMenuManager.gameObject.activeSelf)
            randomizeButton.RandomizeCharacter();

        if (Input.GetKeyDown(KeyCode.C) && !saveCharacterManager.gameObject.activeSelf && !infoMenuManager.gameObject.activeSelf)
            tryCharacterButtonManager.LoadTryCharacterScene();
    }
}