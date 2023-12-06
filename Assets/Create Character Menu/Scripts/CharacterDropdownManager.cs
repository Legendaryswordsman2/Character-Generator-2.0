using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDropdownManager : MonoBehaviour
{
    public static CharacterDropdownManager Instance;

    [SerializeField] Sprite characterSpritesheet;

    [field: SerializeField] public CharacterPieceCollectionDropdownData[] CharacterPiecesDropdownData { get; private set; }

    CharacterPieceDatabase characterPieceDatabase;

    public event EventHandler OnDropdownValueChanged;

    bool eventTriggeredThisFrame = false;

    public bool CanRecreateCharacter = true;

    private void Awake()
    {
        Instance = this;

        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;
    }

    public void OnDropdownUpdated(int index)
    {
        RecreateCharacter();
    }

    public async void RecreateCharacter()
    {
        if (eventTriggeredThisFrame || !CanRecreateCharacter) return;

        OnDropdownValueChanged?.Invoke(this, EventArgs.Empty);

        List<Texture2D> textureToBeCombined = new();

        //Texture2D[] texturesToBeCombined = new Texture2D[CharacterPiecesDropdownData.Length];

        for (int i = 0; i < CharacterPiecesDropdownData.Length; i++)
        {
            if (CharacterPiecesDropdownData[i].ActiveSprite != null)
                textureToBeCombined.Add(CharacterPiecesDropdownData[i].ActiveSprite.texture);
        }

        if (textureToBeCombined.Count <= 0) return;

        //Debug.Log("Recreating Character");
        SpriteManager.OverrideSprite(characterSpritesheet.texture, SpriteManager.CombineTextures(textureToBeCombined).texture);

        eventTriggeredThisFrame = true;
        await Task.Yield();
        eventTriggeredThisFrame = false;
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, EventArgs e)
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        for (int i = 0; i < characterPieceDatabase.CharacterPieces.Length; i++)
        {
            CharacterPiecesDropdownData[i].sprites = characterPieceDatabase.CharacterPieces[i].Sprites;
            CharacterPiecesDropdownData[i].SetActiveSprite(0);
            CharacterPiecesDropdownData[i].CanRandomize = PlayerPrefs.GetInt(CharacterPiecesDropdownData[i].CollectionName.ToString(), 1) == 1;
            InitializeDropdown(CharacterPiecesDropdownData[i]);

        }

        OnDropdownUpdated(0);
    }

    void InitializeDropdown(CharacterPieceCollectionDropdownData characterPiece)
    {
        characterPiece.dropdown.ClearOptions();

        if (characterPiece.IncludeNAOption)
            characterPiece.dropdown.options.Add(new TMP_Dropdown.OptionData() { text = characterPiece.CollectionName + " N/A" });

        for (int i = 0; i < characterPiece.sprites.Count; i++)
        {
            characterPiece.dropdown.options.Add(new TMP_Dropdown.OptionData() { text = characterPiece.sprites[i].name.Replace('_', ' ') });
        }

        if (characterPiece.IncludeNAOption && !characterPiece.NADefault)
            characterPiece.dropdown.value = 1;
        else
            characterPiece.dropdown.value = 0;

        characterPiece.dropdown.RefreshShownValue();
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;

        for (int i = 0; i < CharacterPiecesDropdownData.Length; i++)
        {
            PlayerPrefs.SetInt(CharacterPiecesDropdownData[i].CollectionName.ToString(), CharacterPiecesDropdownData[i].CanRandomize ? 1 : 0);
        }
    }

    [System.Serializable]
    public class CharacterPieceCollectionDropdownData
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> sprites;

        [Space]

        public TMP_Dropdown dropdown;

        [field: Space]

        [field: SerializeField] public bool IncludeNAOption { get; private set; } = false;
        [field: SerializeField, ShowIf("IncludeNAOption")] public bool NADefault { get; private set; } = true;

        [field: Space]

        [field: SerializeField, ReadOnly] public Sprite ActiveSprite { get; private set; }

        public bool CanRandomize { get; set; } = true;

        public void SetActiveSprite(int index)
        {
            if (IncludeNAOption)
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

        public void Randomize()
        {
            if (CanRandomize && sprites.Count > 0)
                dropdown.value = UnityEngine.Random.Range(0, sprites.Count);
        }
    }
}