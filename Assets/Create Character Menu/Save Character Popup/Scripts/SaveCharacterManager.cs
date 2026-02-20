using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using System;
using Sirenix.Utilities;
using Cysharp.Threading.Tasks;
using LootLocker.Requests;

public class SaveCharacterManager : MonoBehaviour
{
    [SerializeField] GameObject creatingCharacterOverlay;
    [SerializeField] RectTransform background;
    [SerializeField] GameObject saveCharacterContents;
    [SerializeField] GameObject characterSavedContents;

    [Space]

    [SerializeField] TMP_Text personalStatsText;
    [SerializeField] TMP_Text globalStatsText;
    [SerializeField] GameObject optionalStatusContainer;
    [SerializeField] TMP_Text optionalStatusText;

    [Space]

    [SerializeField] TMP_Text[] contentTexts;

    [Space]

    [SerializeField] TMP_InputField fileNameInputField;
    [SerializeField] TMP_Dropdown sizeDropdown;
    [SerializeField] TMP_Dropdown animationDropdown;
    [SerializeField] AnimationDataDropdown animationDataDropdown;
    [SerializeField] Button saveCharacterButton;

    [Space]

    [SerializeField] Vector2 defaultBackgroundSize;
    [SerializeField] Vector2 backgroundCharacterSavedSize;

    //CharacterDropdownManager characterDropdownManager;
    CharacterPieceDatabase characterPieceDatabase;
    CharacterPieceGrabber characterPieceGrabber;

    bool savingCharacter = false;
    int charactersGeneratedPersonal;
    int charactersGeneratedGlobal;
    bool doneGrabbingServerData = false;
    bool globalLeaderboardConfigured = true;
    bool personalLeaderboardConfigured = true;
    bool globalLeaderboardMissingLogged = false;
    bool personalLeaderboardMissingLogged = false;

    const string GlobalLeaderboardKey = "total_characters_generated";
    const string PersonalLeaderboardKey = "characters_generated";
    const string GlobalCounterMemberId = "global_counter";

    public static event EventHandler OnBeforeCharacterSaved;
    public static event EventHandler OnAfterCharacterSaved;

    public event EventHandler<string> OnSpriteMissingErrorTriggered;

    public event EventHandler OnPopupOpened;


    private void Start()
    {
        //characterDropdownManager = CharacterDropdownManager.Instance;
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        characterPieceGrabber = CharacterPieceGrabber.Instance;
        charactersGeneratedPersonal = GetSavedCharactersCountOnDisk();
    }
    public void OpenPopup()
    {
        OnPopupOpened?.Invoke(this, EventArgs.Empty);
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.zero;
        background.sizeDelta = defaultBackgroundSize;

        saveCharacterContents.SetActive(true);
        characterSavedContents.SetActive(false);
        SetOptionalStatus(string.Empty);

        creatingCharacterOverlay.SetActive(false);

        fileNameInputField.interactable = true;
        sizeDropdown.interactable = true;
        animationDropdown.interactable = true;
        saveCharacterButton.interactable = true;

        foreach (TMP_Text text in contentTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }

        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, 0.1f);
    }

    public void ClosePopup()
    {
        if (savingCharacter) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.zero, 0.075f).setOnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    public async void SaveCharacter()
    {
        OnBeforeCharacterSaved?.Invoke(this, EventArgs.Empty);

        savingCharacter = true;

        creatingCharacterOverlay.SetActive(true);

        fileNameInputField.interactable = false;
        sizeDropdown.interactable = false;
        animationDropdown.interactable = false;
        saveCharacterButton.interactable = false;

        foreach (TMP_Text text in contentTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);
        }

        List<Texture2D> characterPiecesToBeCombined = await GetPiecesToBeCombined();

        if (characterPiecesToBeCombined.IsNullOrEmpty())
        {
            Debug.LogWarning("No character pieces to be combined");
            savingCharacter = false;
            return;
        }
        else if (characterPiecesToBeCombined.Count == 1)
        {
            SaveCharacterToFile(characterPiecesToBeCombined[0]);
            return;
        }

        Texture2D finalTexture = characterPiecesToBeCombined[0];

        for (int i = 1; i < characterPiecesToBeCombined.Count; i++)
        {
            finalTexture = SpriteManager.CombineTwoTextures(finalTexture, characterPiecesToBeCombined[i]);
        }

        AnimationSO animationData = animationDataDropdown.RefreshSelectedAnimation();
        if (animationData != null)
        {
            switch (sizeDropdown.value)
            {
                case 0:
                    // 16x16
                    finalTexture = SpriteManager.ExtractTextureRegion(finalTexture, animationData.AnimationStartPosition.x, animationData.AnimationStartPosition.y, animationData.AnimationPositionOffset.x, animationData.AnimationPositionOffset.y);
                    break;
                case 1:
                    // 32x32
                    finalTexture = SpriteManager.ExtractTextureRegion(finalTexture, animationData.AnimationStartPosition32x32.x, animationData.AnimationStartPosition32x32.y, animationData.AnimationPositionOffset32x32.x, animationData.AnimationPositionOffset32x32.y);
                    break;
                case 2:
                    // 48x48
                    finalTexture = SpriteManager.ExtractTextureRegion(finalTexture, animationData.AnimationStartPosition48x48.x, animationData.AnimationStartPosition48x48.y, animationData.AnimationPositionOffset48x48.x, animationData.AnimationPositionOffset48x48.y);
                    break;
            }
        }

        //finalTexture = SpriteManager.ExtractTextureRegion(finalTexture, 0, 32, 383, 32);

        SaveCharacterToFile(finalTexture);
        UpdateScores();
    }

    async void SaveCharacterToFile(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Can't save texture, texture is null");
            return;
        }

        byte[] bytes = texture.EncodeToPNG();

        if (!Directory.Exists(CharacterPieceDatabase.SavedCharactersDirectory))
            Directory.CreateDirectory(CharacterPieceDatabase.SavedCharactersDirectory);

        if (fileNameInputField.text == "")
            fileNameInputField.text = "Unnamed Character";

        string savedFilePath = Path.Combine(CharacterPieceDatabase.SavedCharactersDirectory, fileNameInputField.text + ".png");
        File.WriteAllBytes(savedFilePath, bytes);
        charactersGeneratedPersonal = GetSavedCharactersCountOnDisk();

        if (LootlockerAuthenticationManager.LoggedIn)
        {
            int completedTask = await UniTask.WhenAny(
                UniTask.WaitUntil(() => doneGrabbingServerData),
                UniTask.Delay(2500));

            personalStatsText.text = "You've saved " + charactersGeneratedPersonal.ToString("N0") + " character(s) total.";
            globalStatsText.text = charactersGeneratedGlobal.ToString("N0") + " characters have been saved globally.";

            if (completedTask == 0)
            {
                if (!personalLeaderboardConfigured && !globalLeaderboardConfigured)
                    SetOptionalStatus("Optional online stats are not configured.");
                else if (!personalLeaderboardConfigured)
                    SetOptionalStatus("Optional personal stats are not configured.");
                else if (!globalLeaderboardConfigured)
                    SetOptionalStatus("Optional global stats are not configured.");
                else
                    SetOptionalStatus(string.Empty);
            }
            else
            {
                SetOptionalStatus("Optional online stats sync is taking longer than expected.");
            }

            savingCharacter = false;

            OnAfterCharacterSaved?.Invoke(this, EventArgs.Empty);

            creatingCharacterOverlay.SetActive(false);
            OpenCharacterSavedPopup();
        }
        else
        {
            personalStatsText.text = "You've saved " + charactersGeneratedPersonal.ToString("N0") + " character(s) total.";
            globalStatsText.text = charactersGeneratedGlobal.ToString("N0") + " characters have been saved globally.";
            SetOptionalStatus("Optional online stats are unavailable while offline.");

            savingCharacter = false;

            OnAfterCharacterSaved?.Invoke(this, EventArgs.Empty);

            creatingCharacterOverlay.SetActive(false);
            OpenCharacterSavedPopup();
        }
    }

    async void UpdateScores()
    {
        doneGrabbingServerData = false;

        if (LootlockerAuthenticationManager.LoggedIn)
        {
            try
            {
                List<Task> tasks = new()
                {
                   UpdateGlobalScore(),
                   UpdatePersonalScore()
                };

                await Task.WhenAll(tasks);
            }
            finally
            {
                doneGrabbingServerData = true;
            }
        }

        async Task UpdateGlobalScore()
        {
            if (!globalLeaderboardConfigured) return;

            int score = 0;

            bool finished = false;

            bool succesful = true;
            LootLockerSDKManager.GetScoreList(GlobalLeaderboardKey, 1, 0, (response) =>
            {
                if (response.success)
                {
                    if (response.items != null && response.items.Length > 0)
                        score = response.items[0].score;
                    else
                        score = 0;
                }
                else
                {
                    if (IsLeaderboardMissing(response.errorData?.message))
                    {
                        globalLeaderboardConfigured = false;
                        if (!globalLeaderboardMissingLogged)
                        {
                            Debug.LogWarning($"Leaderboard '{GlobalLeaderboardKey}' is not configured in LootLocker. Global stats will be disabled.");
                            globalLeaderboardMissingLogged = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to fetch leaderboard data: " + response.errorData.message);
                    }
                    succesful = false;
                }

                finished = true;
            });

            await UniTask.WaitUntil(() => finished);

            if (!succesful) return;

            bool done = false;

            // Add to global score
            LootLockerSDKManager.SubmitScore(GlobalCounterMemberId, score + 1, GlobalLeaderboardKey, (response) =>
            {
                if (response.success)
                {
                    charactersGeneratedGlobal = score + 1;
                    //Debug.Log(score + " Global");
                }
                else
                {
                    if (IsLeaderboardMissing(response.errorData?.message))
                    {
                        globalLeaderboardConfigured = false;
                        if (!globalLeaderboardMissingLogged)
                        {
                            Debug.LogWarning($"Leaderboard '{GlobalLeaderboardKey}' is not configured in LootLocker. Global stats will be disabled.");
                            globalLeaderboardMissingLogged = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Unable to upload global score: " + response.errorData.message);
                    }
                }

                done = true;
            });

            await UniTask.WaitUntil(() => done);
        }

        async Task UpdatePersonalScore()
        {
            if (!personalLeaderboardConfigured) return;

            int score = 0;

            bool finished = false;

            bool succesful = true;
            LootLockerSDKManager.GetMemberRank(PersonalLeaderboardKey, PlayerPrefs.GetString("PlayerID"), (response) =>
            {
                if (response.success)
                    score = response.score;
                else
                {
                    if (IsLeaderboardMissing(response.errorData?.message))
                    {
                        personalLeaderboardConfigured = false;
                        if (!personalLeaderboardMissingLogged)
                        {
                            Debug.LogWarning($"Leaderboard '{PersonalLeaderboardKey}' is not configured in LootLocker. Personal stats will be disabled.");
                            personalLeaderboardMissingLogged = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Failed to fetch leaderboard data: " + response.errorData.message);
                    }
                    succesful = false;
                }

                finished = true;
            });

            await UniTask.WaitUntil(() => finished);

            if (!succesful) return;

            bool done = false;

            LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("PlayerID"), score + 1, PersonalLeaderboardKey, (response) =>
            {
                if (response.success)
                {
                    charactersGeneratedPersonal = score + 1;
                    //Debug.Log(score + " Pesonal");
                }
                else
                {
                    if (IsLeaderboardMissing(response.errorData?.message))
                    {
                        personalLeaderboardConfigured = false;
                        if (!personalLeaderboardMissingLogged)
                        {
                            Debug.LogWarning($"Leaderboard '{PersonalLeaderboardKey}' is not configured in LootLocker. Personal stats will be disabled.");
                            personalLeaderboardMissingLogged = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Unable to upload personal score: " + response.errorData.message);
                    }
                }

                done = true;
            });

            await UniTask.WaitUntil(() => done);
        }
    }

    static bool IsLeaderboardMissing(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        return message.IndexOf("leaderboard does not exist", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    void SetOptionalStatus(string message)
    {
        bool hasMessage = !string.IsNullOrWhiteSpace(message);

        if (optionalStatusContainer != null)
            optionalStatusContainer.SetActive(hasMessage);

        if (optionalStatusText != null)
            optionalStatusText.text = hasMessage ? message : string.Empty;
    }

    int GetSavedCharactersCountOnDisk()
    {
        if (string.IsNullOrWhiteSpace(CharacterPieceDatabase.SavedCharactersDirectory))
            return 0;

        if (!Directory.Exists(CharacterPieceDatabase.SavedCharactersDirectory))
            return 0;

        try
        {
            return Directory.GetFiles(CharacterPieceDatabase.SavedCharactersDirectory, "*.png", SearchOption.TopDirectoryOnly).Length;
        }
        catch
        {
            return 0;
        }
    }

    async void OpenCharacterSavedPopup()
    {
        saveCharacterContents.SetActive(false);
        LeanTween.size(background, backgroundCharacterSavedSize, 0.25f)
            .setOnComplete(() =>
            {
                //characterSavedContents.SetActive(true);
            });

        await UniTask.Delay(200);
        characterSavedContents.SetActive(true);
    }

    async Task<List<Texture2D>> GetPiecesToBeCombined()
    {
        List<Texture2D> characterPiecesToBeCombined = new();

        CharacterSize characterSize = CharacterSize.Sixteen;

        switch (sizeDropdown.value)
        {
            case 0:
                characterSize = CharacterSize.Sixteen;
                break;
            case 1:
                characterSize = CharacterSize.Thirtytwo;
                break;
            case 2:
                characterSize = CharacterSize.Fortyeight;
                break;
        }

        if (characterSize == CharacterSize.Sixteen)
        {
            for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
            {
                if (characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite != null)
                    characterPiecesToBeCombined.Add(characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite.texture);
            }
        }
        else
        {
            // Load other size of sprites

            for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
            {
                if (characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite != null)
                {
                    if (!CharacterPieceDatabase.TryResolveCharacterPiecesDirectory(out string characterPiecesDirectory))
                    {
                        OnSpriteMissingErrorTriggered?.Invoke(this, "Character Pieces folder is missing.");
                        return null;
                    }

                    string filePath = Path.Combine(characterPiecesDirectory,
                        characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].spriteLocation,
                        GetCurrentSizeAsStringFromDropdown(),
                        characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite.name);

                    Texture2D newTexture = await characterPieceGrabber.GetImageAsTexture2D(new Uri(filePath).AbsoluteUri + ".png",
                        characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite.name,
                        ".png", characterSize, characterPieceDatabase.ActiveCharacterType);

                    if (newTexture != null)
                        characterPiecesToBeCombined.Add(newTexture);
                    else
                    {
                        TriggerSpriteMissingError();
                        return null;
                    }
                }
            }
        }

        return characterPiecesToBeCombined;
    }

    void TriggerSpriteMissingError()
    {
        OnSpriteMissingErrorTriggered?.Invoke(this, sizeDropdown.options[sizeDropdown.value].text + " version of sprite \"" + characterPieceGrabber.LastFailedToGetSpriteName + "\" is missing, can't create character");
    }

    string GetCurrentSizeAsStringFromDropdown()
    {
        switch (sizeDropdown.value)
        {
            case 0:
                return "16x16";
            case 1:
                return "32x32";
            case 2:
                return "48x48";
            default:
                return "";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePopup();
    }
}
