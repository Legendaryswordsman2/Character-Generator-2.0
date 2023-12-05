using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDropdown : MonoBehaviour
{
    [SerializeField] CharacterDropdownManager dropdownManager;

    [Space]

    [SerializeField] int dropdownIndex = 0;

    private void Start()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, System.EventArgs e)
    {
        OnDropdownChanged(0);
    }

    public void OnDropdownChanged(int index)
    {
        dropdownManager.CharacterPiecesDropdownData[dropdownIndex].SetActiveSprite(index);
        dropdownManager.OnDropdownUpdated(dropdownIndex);
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }
}