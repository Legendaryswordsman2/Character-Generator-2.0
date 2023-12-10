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

    public const string CharacterPiecesFolderName = "Character Pieces";

    public event EventHandler<CharacterTypeSO> OnActiveCharacterTypeChanged;

    private void Awake()
    {
        foreach (CharacterTypeSO characterType in CharacterTypes)
        {
            characterType.Init();
        }

        ActiveCharacterType = CharacterTypes[0];

        Instance = this;
    }

    public void SetActiveCharacterType(CharacterTypeSO characterType)
    {
        if (characterType == ActiveCharacterType) return;

        //Debug.Log("Set New Character Type");
        ActiveCharacterType = characterType;

        OnActiveCharacterTypeChanged?.Invoke(this, ActiveCharacterType);
    }

    private void OnDestroy()
    {
        foreach (CharacterTypeSO characterType in CharacterTypes)
        {
            characterType.ClearSprites();
            characterType.SaveRandomizeToggles();
        }
    }

    [System.Serializable]
    public class CharacterPieceCollection
    {
        public CharacterPieceType CollectionName;
        public List<Sprite> Sprites;
    }
}