using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDropdown : MonoBehaviour
{
    [SerializeField] CharacterDropdownManager dropdownManager;

    [Space]

    bool initialized = false;

    [field: SerializeReference] public CharacterTypeSO.CharacterPieceCollection CharacterPiece { get; private set; }

    public TMP_Dropdown Dropdown { get; private set; }

    private void Awake()
    {
        Dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    private async void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, System.EventArgs e)
    {
        await UniTask.NextFrame();

        initialized = true;
    }

    public void UpdateDropdown(CharacterTypeSO.CharacterPieceCollection newCharacterPiece)
    {
        CharacterPiece = newCharacterPiece;

        Dropdown.ClearOptions();

        if (CharacterPiece.IncludeNAOption)
            Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = CharacterPiece.CollectionName + " N/A" });

        for (int i = 0; i < CharacterPiece.Sprites.Count; i++)
            Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = CharacterPiece.Sprites[i].name.Replace('_', ' ') });

        if (CharacterPiece.IncludeNAOption && !CharacterPiece.NADefault)
        {
            Dropdown.value = 1;
        }
        else
            Dropdown.value = 0;

        CharacterPiece.SetActiveSprite(0);

        Dropdown.RefreshShownValue();
    }

    public void OnDropdownChanged(int index)
    {
        CharacterPiece.SetActiveSprite(index);
        //dropdownManager.CharacterPiecesDropdownData[dropdownIndex].SetActiveSprite(index);

        if (initialized)
            dropdownManager.OnDropdownUpdated();
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }
}