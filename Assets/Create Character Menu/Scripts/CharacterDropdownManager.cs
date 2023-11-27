using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDropdownManager : MonoBehaviour
{
    [SerializeField] CharacterPieceCollectionDropdownData[] characterPiecesDropdownData;

    CharacterPieceDatabase characterPieceDatabase;

    private void Awake()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, EventArgs e)
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        for (int i = 0; i < characterPieceDatabase.CharacterPieces.Length; i++)
        {
            characterPiecesDropdownData[i].sprites = characterPieceDatabase.CharacterPieces[i].Sprites;
            InitializeDropdown(characterPiecesDropdownData[i]);
        }
    }

    void InitializeDropdown(CharacterPieceCollectionDropdownData characterPiece)
    {
        characterPiece.dropdown.ClearOptions();

        if (characterPiece.includeNAOption)
            characterPiece.dropdown.options.Add(new TMP_Dropdown.OptionData() { text = characterPiece.CollectionName + " N/A" });

        for (int i = 0; i < characterPiece.sprites.Count; i++)
        {
            characterPiece.dropdown.options.Add(new TMP_Dropdown.OptionData() { text = characterPiece.sprites[i].name.Replace('_', ' ') });
        }

        characterPiece.dropdown.value = 0;
        characterPiece.dropdown.RefreshShownValue();
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    [System.Serializable]
    public class CharacterPieceCollectionDropdownData
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> sprites;

        [Space]

        public TMP_Dropdown dropdown;

        [Space]

        public bool includeNAOption = false;
        public bool NAOptionSelectedDefault = true;

        [Space]

        [ReadOnly] public Sprite activeSprite;
        [ReadOnly] public int activeSpriteIndex;
        [ReadOnly] public int index;

        //[Space]

        //public Image imageComponent;
        //public Toggle canRandomizeToggle;

        public event EventHandler OnActivePortraitPieceChanged;

        public Action<int, int, bool> OnDropdownChangedMethod;

        //public void Randomize()
        //{
        //    if (!canRandomizeToggle.isOn || sprites.Count == 0) return;

        //    int numb = UnityEngine.Random.Range(0, sprites.Count);

        //    dropdown.value = numb;
        //}

        public void InvokeOnActivePortraitPieceChanged()
        {
            OnActivePortraitPieceChanged?.Invoke(this, null);
        }
    }
}