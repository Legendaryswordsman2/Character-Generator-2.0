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
    [field: SerializeField] public CharacterPieceCollection[] CharacterPieces { get; private set; }

    public const string CharacterPiecesFolderName = "Character Pieces";

    public void AddCharacterPiece(Sprite piece, CharacterPieceType type)
    {
        switch (type)
        {
            case CharacterPieceType.Body:
                CharacterPieces[0].Sprites.Add(piece);
                break;
            case CharacterPieceType.Eyes:
                CharacterPieces[1].Sprites.Add(piece);
                break;
            case CharacterPieceType.Outfit:
                CharacterPieces[2].Sprites.Add(piece);
                break;
            case CharacterPieceType.Hairstyle:
                CharacterPieces[3].Sprites.Add(piece);
                break;
            case CharacterPieceType.Accessory:
                CharacterPieces[4].Sprites.Add(piece);
                break;
        }
    }

    [System.Serializable]
    public class CharacterPieceCollection
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> Sprites;
    }
}