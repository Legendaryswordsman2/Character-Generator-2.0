using Cysharp.Threading.Tasks;
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

    [field: SerializeField] public CharacterPieceCollectionDropdownData[] CharacterPiecesDropdownData { get; private set; }

    [SerializeField] CharacterDropdown[] dropdowns;

    CharacterPieceDatabase characterPieceDatabase;

    public event EventHandler OnDropdownValueChanged;

    bool eventTriggeredThisFrame = false;

    public bool CanRecreateCharacter = true;

    [Space]

    [SerializeField] Transform topPOS;

    public static event EventHandler OnBeforeCharacterRecreated;
    public static event EventHandler OnAfterCharacterRecreated;

    private void Awake()
    {
        Instance = this;

        CharacterPieceGrabber.OnAllCharacterPiecesLoaded += CharacterPieceGrabber_OnAllCharacterPiecesLoaded;

        TryCharacterButtonManager.MoveAllUIOffScreen += TryCharacterButtonManager_MoveAllUIOffScreen;
    }

    private void TryCharacterButtonManager_MoveAllUIOffScreen(object sender, float time)
    {
        LeanTween.moveY(gameObject, topPOS.position.y, time);
    }

    public void OnDropdownUpdated()
    {
        RecreateCharacter();
    }

    public async void RecreateCharacter()
    {
        if (eventTriggeredThisFrame || !CanRecreateCharacter) return;

        OnDropdownValueChanged?.Invoke(this, EventArgs.Empty);

        List<Texture2D> textureToBeCombined = new();

        for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
        {
            if (characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite != null)
                textureToBeCombined.Add(characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite.texture);
        }

        //for (int i = 0; i < CharacterPiecesDropdownData.Length; i++)
        //{
        //    if (CharacterPiecesDropdownData[i].ActiveSprite != null)
        //        textureToBeCombined.Add(CharacterPiecesDropdownData[i].ActiveSprite.texture);
        //}

        if (textureToBeCombined.Count <= 0) return;

        //Debug.Log("Recreating Character");
        OnBeforeCharacterRecreated?.Invoke(this, EventArgs.Empty);
        SpriteManager.OverrideSprite(characterPieceDatabase.ActiveCharacterType.CharacterPreviewSpritesheet.texture, SpriteManager.CombineTextures(textureToBeCombined).texture);

        eventTriggeredThisFrame = true;
        await Task.Yield();
        eventTriggeredThisFrame = false;

        OnAfterCharacterRecreated?.Invoke(this, EventArgs.Empty);
    }

    private void CharacterPieceGrabber_OnAllCharacterPiecesLoaded(object sender, EventArgs e)
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        characterPieceDatabase.OnActiveCharacterTypeChanged += CharacterPieceDatabase_OnActiveCharacterTypeChanged;

        RefreshDropdowns();

        //for (int i = 0; i < characterPieceDatabase.CharacterPieces.Length; i++)
        //{
        //    CharacterPiecesDropdownData[i].sprites = characterPieceDatabase.CharacterPieces[i].Sprites;
        //    CharacterPiecesDropdownData[i].SetActiveSprite(0);
        //    CharacterPiecesDropdownData[i].CanRandomize = PlayerPrefs.GetInt(CharacterPiecesDropdownData[i].CollectionName.ToString(), 1) == 1;
        //    InitializeDropdown(CharacterPiecesDropdownData[i]);
        //}

        OnDropdownUpdated();
    }

    private void CharacterPieceDatabase_OnActiveCharacterTypeChanged(object sender, CharacterTypeSO e)
    {
        RefreshDropdowns();
        OnDropdownUpdated();
    }

    void RefreshDropdowns()
    {
        if (characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length > dropdowns.Length)
            Debug.LogWarning("Not enough dropdowns!");

        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length - 1 >= i)
            {
                CharacterPiecesDropdownData[i].sprites = characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].Sprites;
                dropdowns[i].UpdateDropdown(characterPieceDatabase.ActiveCharacterType.CharacterPieces[i]);
            }
            else
                dropdowns[i].gameObject.SetActive(false);
        }

        //for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
        //{
        //    if (i > dropdowns.Length - 1)
        //    {
        //        Debug.LogWarning("Not enough dropdowns");
        //        return;
        //    }

        //    dropdowns[i].UpdateDropdown(characterPieceDatabase.ActiveCharacterType.CharacterPieces[i]);
        //}
    }

    public void RandomizeAllDropdowns()
    {
        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (dropdowns[i].gameObject.activeSelf && dropdowns[i].CharacterPiece.CanRandomize && dropdowns[i].CharacterPiece.Sprites.Count > 0)
                dropdowns[i].Dropdown.value = UnityEngine.Random.Range(0, dropdowns[i].CharacterPiece.Sprites.Count);
        }
    }

    public void SetDropdownValue(int dropdownIndex, int value)
    {
        dropdowns[dropdownIndex].Dropdown.value = value;
    }

    private void OnDestroy()
    {
        CharacterPieceGrabber.OnAllCharacterPiecesLoaded -= CharacterPieceGrabber_OnAllCharacterPiecesLoaded;

        TryCharacterButtonManager.MoveAllUIOffScreen -= TryCharacterButtonManager_MoveAllUIOffScreen;

        if (characterPieceDatabase != null)
            characterPieceDatabase.OnActiveCharacterTypeChanged -= CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }

    [System.Serializable]
    public class CharacterPieceCollectionDropdownData
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> sprites;

        [Space]

        public TMP_Dropdown dropdown;
    }
}