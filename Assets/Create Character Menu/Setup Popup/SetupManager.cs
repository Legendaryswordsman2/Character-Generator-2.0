using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    [SerializeField] GameObject setupPopup;
    [SerializeField] GameObject errorPopup;

    private void Awake()
    {
        setupPopup.SetActive(true);

        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, System.EventArgs e)
    {
        setupPopup.GetComponent<SetupMessage>().OnFinishedSettingUp();
    }
}