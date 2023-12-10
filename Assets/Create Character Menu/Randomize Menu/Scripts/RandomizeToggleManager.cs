using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeToggleManager : MonoBehaviour
{
    [SerializeField] RandomizeToggle[] toggles;

    CharacterPieceDatabase characterPieceDatabase;

    private void Start()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        RefreshToggles();

        characterPieceDatabase.OnActiveCharacterTypeChanged += CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }

    private void CharacterPieceDatabase_OnActiveCharacterTypeChanged(object sender, CharacterTypeSO e)
    {
        RefreshToggles();
    }

    void RefreshToggles()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length> i)
            {
                toggles[i].SetToggle(characterPieceDatabase.ActiveCharacterType.CharacterPieces[i]);
            }
            else
                toggles[i].gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        characterPieceDatabase.OnActiveCharacterTypeChanged -= CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }
}