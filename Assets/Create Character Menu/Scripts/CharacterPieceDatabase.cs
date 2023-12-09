using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterPieceType { Body, Eyes, Outfit, Hairstyle, Accessory}
public class CharacterPieceDatabase : MonoBehaviour
{
    public static CharacterPieceDatabase Instance;

    [field: SerializeField, ReadOnly] public CharacterTypeSO ActiveCharacterType { get; private set; }
    [field: SerializeField] public CharacterTypeSO[] CharacterTypes { get; private set; }
    //[field: SerializeField] CharacterPieceCollection[] CharacterPieces { get; private set; }

    public const string CharacterPiecesFolderName = "Character Pieces";

    private void Awake()
    {
        foreach (CharacterTypeSO characterType in CharacterTypes)
        {
            characterType.ClearSprites();
        }

        ActiveCharacterType = CharacterTypes[1];

        Instance = this;
    }

    //public void AddCharacterPiece(Sprite piece, CharacterPieceType type)
    //{
    //    switch (type)
    //    {
    //        case CharacterPieceType.Body:
    //            CharacterPieces[0].Sprites.Add(piece);
    //            break;
    //        case CharacterPieceType.Eyes:
    //            CharacterPieces[1].Sprites.Add(piece);
    //            break;
    //        case CharacterPieceType.Outfit:
    //            CharacterPieces[2].Sprites.Add(piece);
    //            break;
    //        case CharacterPieceType.Hairstyle:
    //            CharacterPieces[3].Sprites.Add(piece);
    //            break;
    //        case CharacterPieceType.Accessory:
    //            CharacterPieces[4].Sprites.Add(piece);
    //            break;
    //    }
    //}

    private void OnDestroy()
    {
        foreach (CharacterTypeSO characterType in CharacterTypes)
        {
            characterType.ClearSprites();
        }
    }

    [System.Serializable]
    public class CharacterPieceCollection
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> Sprites;
    }
}