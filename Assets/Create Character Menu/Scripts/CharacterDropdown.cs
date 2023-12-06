using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDropdown : MonoBehaviour
{
    [SerializeField] CharacterDropdownManager dropdownManager;

    [Space]

    [SerializeField] int dropdownIndex = 0;

    bool initialized = false;

    private void Start()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    private async void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, System.EventArgs e)
    {
        await UniTask.NextFrame();

        initialized = true;
    }

    public void OnDropdownChanged(int index)
    {
        dropdownManager.CharacterPiecesDropdownData[dropdownIndex].SetActiveSprite(index);

        if (initialized)
            dropdownManager.OnDropdownUpdated(dropdownIndex);
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }
}