using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum HistoryViewMode { Modifications, Saved }
public class HistoryTabManager : MonoBehaviour
{
    [field: SerializeField] public HistoryViewMode HistoryViewMode { get; private set; }

    [Space]

    [SerializeField] Transform characterPreviewsParent;

    [SerializeField] List<CharacterBackupPreview> characterPreviewImages;

    CharacterPieceDatabase characterPieceDatabase;
    CharacterDropdownManager characterDropdownManager;

    bool canTrackHistory = true;

    public event EventHandler OnHistoryViewModeChanged;

    public void Init()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        characterDropdownManager = CharacterDropdownManager.Instance;

        CharacterDropdownManager.OnAfterCharacterRecreated += CharacterDropdownManager_OnAfterCharacterRecreated;
        SaveCharacterManager.OnAfterCharacterSaved += SaveCharacterManager_OnAfterCharacterSaved;
    }

    private void Start()
    {
        OnHistoryViewModeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SaveCharacterManager_OnAfterCharacterSaved(object sender, System.EventArgs e)
    {
        if (!canTrackHistory) return;

        Sprite newSprite = SpriteManager.ConvertTextureToSprite(SpriteManager.ExtractTextureRegion(characterPieceDatabase.ActiveCharacterType.CharacterPreviewSpritesheet.texture, 48, 0, 16, 32));

        int[] characterPieceIndexes = new int[characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length];

        for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
        {
            characterPieceIndexes[i] = characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].DropdownIndex;
        }

        if (characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory.Count >= characterPreviewImages.Count)
            characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory.Remove(characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory[^1]);

        characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory.Insert(0, new CharacterTypeSO.CharacterBackup(newSprite, characterPieceIndexes));

        RefreshCharacterList();
    }

    private void CharacterDropdownManager_OnAfterCharacterRecreated(object sender, System.EventArgs e)
    {
        if (!canTrackHistory) return;

        bool canContinue = false;
        if (characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory.Count < 1)
            canContinue = true;
        else
        {
            for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory[0].CharacterPieceIndexes.Length; i++)
            {
                if (characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory[0].CharacterPieceIndexes[i] != characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].DropdownIndex)
                {
                    canContinue = true;
                    break;
                }
            }
        }

        if (!canContinue)
        {
            RefreshCharacterList();
            return;
        }

        Sprite newSprite = SpriteManager.ConvertTextureToSprite(SpriteManager.ExtractTextureRegion(characterPieceDatabase.ActiveCharacterType.CharacterPreviewSpritesheet.texture, 48, 0, 16, 32));

        int[] characterPieceIndexes = new int[characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length];

        for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
        {
            characterPieceIndexes[i] = characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].DropdownIndex;
        }

        if (characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory.Count >= characterPreviewImages.Count)
            characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory.Remove(characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory[^1]);

        characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory.Insert(0, new CharacterTypeSO.CharacterBackup(newSprite, characterPieceIndexes));

        RefreshCharacterList();
    }

    void RefreshCharacterList()
    {
        switch (HistoryViewMode)
        {
            case HistoryViewMode.Modifications:
                for (int i = 0; i < characterPreviewImages.Count; i++)
                {
                    if (characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory.Count - 1 < i)
                    {
                        characterPreviewImages[i].CharacterPreviewController.DisableCharacterBackup();
                        continue;
                    }

                    characterPreviewImages[i].CharacterPreviewImage.sprite = characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory[i].CharacterPreviewSprite;
                    characterPreviewImages[i].CharacterPreviewController.SetCharacterBackup(characterPieceDatabase.ActiveCharacterType.CharacterModificationHistory[i]);
                }
                break;
            case HistoryViewMode.Saved:
                for (int i = 0; i < characterPreviewImages.Count; i++)
                {
                    if (characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory.Count - 1 < i)
                    {
                        characterPreviewImages[i].CharacterPreviewController.DisableCharacterBackup();
                        continue;
                    }

                    characterPreviewImages[i].CharacterPreviewImage.sprite = characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory[i].CharacterPreviewSprite;
                    characterPreviewImages[i].CharacterPreviewController.SetCharacterBackup(characterPieceDatabase.ActiveCharacterType.CharacterSaveHistory[i]);
                }
                break;
        }
    }

    public void LoadPreviousCharacter(CharacterTypeSO.CharacterBackup characterBackup)
    {
        canTrackHistory = false;
        characterDropdownManager.CanRecreateCharacter = false;
        //characterDropdownManager.RandomizeAllDropdowns();

        for (int i = 0; i < characterBackup.CharacterPieceIndexes.Length; i++)
        {
            characterDropdownManager.SetDropdownValue(i, characterBackup.CharacterPieceIndexes[i]);
        }

        characterDropdownManager.CanRecreateCharacter = true;

        CharacterDropdownManager.OnAfterCharacterRecreated += CharacterDropdownManager_OnAfterCharacterRecreated1;

        characterDropdownManager.RecreateCharacter();
    }

    private void CharacterDropdownManager_OnAfterCharacterRecreated1(object sender, System.EventArgs e)
    {
        canTrackHistory = true;
        CharacterDropdownManager.OnAfterCharacterRecreated -= CharacterDropdownManager_OnAfterCharacterRecreated1;
    }

    public void ChangeHistoryViewMode(HistoryViewMode newMode)
    {
        if (newMode == HistoryViewMode) return;

        HistoryViewMode = newMode;
        OnHistoryViewModeChanged?.Invoke(this, EventArgs.Empty);
        RefreshCharacterList();

    }

    public void OnDestroyEventCalled()
    {
        CharacterDropdownManager.OnAfterCharacterRecreated -= CharacterDropdownManager_OnAfterCharacterRecreated;
        SaveCharacterManager.OnAfterCharacterSaved -= SaveCharacterManager_OnAfterCharacterSaved;

        foreach (CharacterTypeSO characterType in characterPieceDatabase.CharacterTypes)
        {
            List<int[]> characterbackupSaveData = new();

            for (int i = 0; i < characterType.CharacterSaveHistory.Count; i++)
            {
                characterbackupSaveData.Add(characterType.CharacterSaveHistory[i].CharacterPieceIndexes);
                SpriteManager.SaveTextureToFile(characterType.CharacterSaveHistory[i].CharacterPreviewSprite.texture, characterType.CharacterTypeName + " Character Save History", "Character " + (i + 1));
            }

            //for (int i = 0; i < characterbackupSaveData.Count; i++)
            //{
            //    for (int i2 = 0; i2 < characterbackupSaveData[i].Length; i2++)
            //    {
            //        Debug.Log(characterbackupSaveData[i][i2]);
            //    }
            //}
            if(characterbackupSaveData.Count > 0)
            SaveSystem.SaveFile(characterType.CharacterTypeName + " Character Save History", characterType.CharacterTypeName + " Character Piece Values", characterbackupSaveData);
        }
    }

    private void OnValidate()
    {
        if (characterPreviewsParent == null) return;

        characterPreviewImages.Clear();

        foreach (Transform child in characterPreviewsParent)
        {
            if (child.TryGetComponent(out HistoryTabCharacterPreview characterPreviewButton))
            {
                characterPreviewImages.Add(new CharacterBackupPreview(characterPreviewButton, child.GetChild(1).GetComponent<Image>()));
            }
        }
    }

    [System.Serializable]
    class CharacterBackupPreview
    {
        public Image CharacterPreviewImage;
        public HistoryTabCharacterPreview CharacterPreviewController;

        public CharacterBackupPreview(HistoryTabCharacterPreview characterPreviewController, Image characterPreviewImage) 
        { 
            CharacterPreviewController = characterPreviewController;
            CharacterPreviewImage = characterPreviewImage;
        }
    }
}