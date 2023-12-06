using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomizeToggle : MonoBehaviour
{
    [SerializeField] CharacterPieceType characterPiece;

    [Space]

    [SerializeField] UIToggle toggle;

    CharacterDropdownManager characterDropdownManager;

    private void Start()
    {
        characterDropdownManager = CharacterDropdownManager.Instance;

        toggle.isOn = characterDropdownManager.CharacterPiecesDropdownData.FirstOrDefault(character => character.CollectionName == characterPiece).CanRandomize;
    }

    public void UpdateToggle(bool state)
    {
        characterDropdownManager.CharacterPiecesDropdownData.FirstOrDefault(character => character.CollectionName == characterPiece).CanRandomize = state;
    }
}