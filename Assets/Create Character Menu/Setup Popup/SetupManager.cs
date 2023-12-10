using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    [SerializeField] GameObject setupPopup;
    [SerializeField] GameObject errorPopup;

    [Space]

    [SerializeField] GameObject directoryNotFoundErrorGO;
    [SerializeField] GameObject unknownErrorGO;

    private void Awake()
    {
        setupPopup.SetActive(true);

        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
        CharacterPieceGrabber.OnFailedToLoadCharacterPieces += CharacterPieceGrabber_OnFailedToLoadCharacterPieces;
    }

    private void CharacterPieceGrabber_OnFailedToLoadCharacterPieces(object sender, LoadFailedType loadFailedType)
    {
        setupPopup.SetActive(false);
        errorPopup.SetActive(true);

        switch (loadFailedType)
        {
            case LoadFailedType.DirectoryMissing:
                directoryNotFoundErrorGO.SetActive(true);
                break;
            case LoadFailedType.UnknownError:
                unknownErrorGO.SetActive(true);
                break;
        }
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, System.EventArgs e)
    {
        setupPopup.GetComponent<SetupMessage>().OnFinishedSettingUp();
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
        CharacterPieceGrabber.OnFailedToLoadCharacterPieces -= CharacterPieceGrabber_OnFailedToLoadCharacterPieces;
    }
}