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

        var candidatePaths = new List<string>
        {
            Path.Combine(Directory.GetCurrentDirectory(), CharacterPiecesFolderName),
            Path.Combine(Application.dataPath, CharacterPiecesFolderName),
            Path.Combine(Application.dataPath, "Create Character Menu", CharacterPiecesFolderName),
            Path.Combine(Application.streamingAssetsPath, CharacterPiecesFolderName),
            Path.Combine(Application.persistentDataPath, CharacterPiecesFolderName),
        };

        // In player builds (especially macOS .app), Data is inside the app bundle.
        // Add parent locations so "Character Pieces" can live next to the app or inside Contents.
        string dataParent = Path.GetDirectoryName(Application.dataPath);
        if (!string.IsNullOrWhiteSpace(dataParent))
        {
            candidatePaths.Add(Path.Combine(dataParent, CharacterPiecesFolderName));

            string contentsParent = Path.GetDirectoryName(dataParent);
            if (!string.IsNullOrWhiteSpace(contentsParent))
            {
                candidatePaths.Add(Path.Combine(contentsParent, CharacterPiecesFolderName));
            }
        }

        var checkedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string candidatePath in candidatePaths)
        {
            if (string.IsNullOrWhiteSpace(candidatePath))
                continue;

            string normalizedPath = Path.GetFullPath(candidatePath);
            if (!checkedPaths.Add(normalizedPath))
                continue;

            if (Directory.Exists(normalizedPath))
            {
                CharacterPiecesDirectory = normalizedPath;
                resolvedPath = normalizedPath;
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
