using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDropdownManager : MonoBehaviour
{
    [SerializeField] Sprite characterSpritesheet;

    [field: SerializeField] public CharacterPieceCollectionDropdownData[] CharacterPiecesDropdownData { get; private set; }

    CharacterPieceDatabase characterPieceDatabase;

    public event EventHandler OnDropdownValueChanged;

    private void Awake()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    public void OnDropdownUpdated(int index)
    {
        OnDropdownValueChanged?.Invoke(this, EventArgs.Empty);

        List<Texture2D> textureToBeCombined = new();

        //Texture2D[] texturesToBeCombined = new Texture2D[CharacterPiecesDropdownData.Length];

        for (int i = 0; i < CharacterPiecesDropdownData.Length; i++)
        {
            if (CharacterPiecesDropdownData[i].ActiveSprite != null)
                textureToBeCombined.Add(CharacterPiecesDropdownData[i].ActiveSprite.texture);
        }

        if (textureToBeCombined.Count <= 0) return;

        //SpriteManager.CombineTextures_Static(textureToBeCombined);
        SpriteManager.OverrideSprite(characterSpritesheet, SpriteManager.CombineTextures(textureToBeCombined));
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, EventArgs e)
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        for (int i = 0; i < characterPieceDatabase.CharacterPieces.Length; i++)
        {
            CharacterPiecesDropdownData[i].sprites = characterPieceDatabase.CharacterPieces[i].Sprites;
            InitializeDropdown(CharacterPiecesDropdownData[i]);
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

        [field: Space]

        [field: SerializeField, ReadOnly] public Sprite ActiveSprite { get; private set; }
        [ReadOnly] public int activeSpriteIndex;
        //[ReadOnly] public int index;

        public event EventHandler OnActivePortraitPieceChanged;

        public Action<int, int, bool> OnDropdownChangedMethod;

        public void SetActiveSprite(int index)
        {
            if (includeNAOption)
            {
                if (index == 0)
                    ActiveSprite = null;
                else
                    ActiveSprite = sprites[index - 1];
            }
            else
            {
                ActiveSprite = sprites[index];
            }

        }

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