using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomizeToggle : MonoBehaviour
{
    //[SerializeField] CharacterPieceType characterPiece;

    [Space]

    [SerializeField] UIToggle toggle;

    CharacterDropdownManager characterDropdownManager;

    [field: SerializeReference] public CharacterTypeSO.CharacterPieceCollection CharacterPiece { get; private set; }

    private void Start()
    {
        characterDropdownManager = CharacterDropdownManager.Instance;

        //toggle.toggleTransition = UnityEngine.UI.Toggle.ToggleTransition.None;
        //toggle.isOn = CharacterPiece.CanRandomize;
        //toggle.toggleTransition = UnityEngine.UI.Toggle.ToggleTransition.Fade;
    }

    public void SetToggle(CharacterTypeSO.CharacterPieceCollection characterPiece)
    {
        gameObject.SetActive(true);

        CharacterPiece = characterPiece;

        toggle.toggleTransition = UnityEngine.UI.Toggle.ToggleTransition.None;
        toggle.isOn = characterPiece.CanRandomize;
        toggle.toggleTransition = UnityEngine.UI.Toggle.ToggleTransition.Fade;
    }

    public void UpdateToggle(bool state)
    {
        CharacterPiece.CanRandomize = state;
        //characterDropdownManager.CharacterPiecesDropdownData.FirstOrDefault(character => character.CollectionName == characterPiece).CanRandomize = state;
    }
}