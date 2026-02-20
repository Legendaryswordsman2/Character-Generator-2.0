using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public const string SavedCharactersFolderName = "Saved Characters";
    public static string SavedCharactersDirectory { get; private set; }
    public static string CharacterPiecesDirectory { get; private set; }

    public event EventHandler<CharacterTypeSO> OnActiveCharacterTypeChanged;

    private void Awake()
    {
        SavedCharactersDirectory = Path.Combine(Application.persistentDataPath, SavedCharactersFolderName);
        TryResolveCharacterPiecesDirectory(out _);

        foreach (CharacterTypeSO characterType in CharacterTypes)
        {
            characterType.Init();
        }

        ActiveCharacterType = CharacterTypes[0];

        Instance = this;
    }

    public static bool TryResolveCharacterPiecesDirectory(out string resolvedPath)
    {
        if (!string.IsNullOrWhiteSpace(CharacterPiecesDirectory) && Directory.Exists(CharacterPiecesDirectory))
        {
            resolvedPath = CharacterPiecesDirectory;
            return true;
        }

        string[] candidatePaths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), CharacterPiecesFolderName),
            Path.Combine(Application.dataPath, CharacterPiecesFolderName),
            Path.Combine(Application.dataPath, "Create Character Menu", CharacterPiecesFolderName),
            Path.Combine(Application.streamingAssetsPath, CharacterPiecesFolderName),
        };

        foreach (string candidatePath in candidatePaths)
        {
            if (Directory.Exists(candidatePath))
            {
                CharacterPiecesDirectory = candidatePath;
                resolvedPath = candidatePath;
                return true;
            }
        }

        resolvedPath = string.Empty;
        return false;
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
