using UnityEngine;
using UnityEngine.UI;

public class HistoryTabManager : MonoBehaviour
{
    [SerializeField] Image characterImage;

    [Space]

    [SerializeField] CharacterBackupPreview[] characterPreviewImages;

    CharacterPieceDatabase characterPieceDatabase;
    CharacterDropdownManager characterDropdownManager;

    bool canTrackHistory = true;

    public void Init()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        characterDropdownManager = CharacterDropdownManager.Instance;

        CharacterDropdownManager.OnAfterCharacterRecreated += CharacterDropdownManager_OnAfterCharacterRecreated;
    }

    private void CharacterDropdownManager_OnAfterCharacterRecreated(object sender, System.EventArgs e)
    {
        if (!canTrackHistory) return;

        bool canContinue = false;
        if (characterPieceDatabase.ActiveCharacterType.CharacterHistory.Count < 1)
            canContinue = true;
        else
        {
            for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterHistory[0].CharacterPieceIndexes.Length; i++)
            {
                if (characterPieceDatabase.ActiveCharacterType.CharacterHistory[0].CharacterPieceIndexes[i] != characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].DropdownIndex)
                {
                    canContinue = true;
                    break;
                }
            }
        }

        if (!canContinue) return;

        Sprite newSprite = SpriteManager.ConvertTextureToSprite(SpriteManager.ExtractTextureRegion(characterPieceDatabase.ActiveCharacterType.CharacterPreviewSpritesheet.texture, 48, 0, 16, 32));

        characterImage.sprite = newSprite;

        int[] characterPieceIndexes = new int[characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length];

        for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
        {
            characterPieceIndexes[i] = characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].DropdownIndex;
        }

        if (characterPieceDatabase.ActiveCharacterType.CharacterHistory.Count >= characterPreviewImages.Length)
            characterPieceDatabase.ActiveCharacterType.CharacterHistory.Remove(characterPieceDatabase.ActiveCharacterType.CharacterHistory[^1]);

        characterPieceDatabase.ActiveCharacterType.CharacterHistory.Insert(0, new CharacterTypeSO.CharacterBackup(newSprite, characterPieceIndexes));

        RefreshCharacterList();
    }

    void RefreshCharacterList()
    {
        for (int i = 0; i < characterPreviewImages.Length; i++)
        {
            if (characterPieceDatabase.ActiveCharacterType.CharacterHistory.Count - 1 < i)
            {
                characterPreviewImages[i].CharacterPreviewImage.gameObject.SetActive(false);
                continue;
            }
            else
                characterPreviewImages[i].CharacterPreviewImage.gameObject.SetActive(true);

            characterPreviewImages[i].CharacterPreviewImage.sprite = characterPieceDatabase.ActiveCharacterType.CharacterHistory[i].CharacterPreviewSprite;
            characterPreviewImages[i].CharacterPreviewController.SetCharacterBackup(characterPieceDatabase.ActiveCharacterType.CharacterHistory[i]);
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

    private void OnDestroy()
    {
        CharacterDropdownManager.OnAfterCharacterRecreated -= CharacterDropdownManager_OnAfterCharacterRecreated;
    }

    [System.Serializable]
    class CharacterBackupPreview
    {
        public Image CharacterPreviewImage;
        public HistoryTabCharacterPreview CharacterPreviewController;
    }
}