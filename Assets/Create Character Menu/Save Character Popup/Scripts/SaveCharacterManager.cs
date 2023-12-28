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

    [Space]

    [SerializeField] TMP_Text[] contentTexts;

    [Space]

    [SerializeField] TMP_InputField fileNameInputField;
    [SerializeField] TMP_Dropdown sizeDropdown;
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

    public static event EventHandler OnBeforeCharacterSaved;
    public static event EventHandler OnAfterCharacterSaved;

    public event EventHandler<string> OnSpriteMissingErrorTriggered;

    public event EventHandler OnPopupOpened;


    private void Start()
    {
        //characterDropdownManager = CharacterDropdownManager.Instance;
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        characterPieceGrabber = CharacterPieceGrabber.Instance;
    }
    public void OpenPopup()
    {
        OnPopupOpened?.Invoke(this, EventArgs.Empty);
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.zero;
        background.sizeDelta = defaultBackgroundSize;

        saveCharacterContents.SetActive(true);
        characterSavedContents.SetActive(false);

        creatingCharacterOverlay.SetActive(false);

        fileNameInputField.interactable = true;
        sizeDropdown.interactable = true;
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

        if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Saved Characters"))
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Saved Characters");

        if (fileNameInputField.text == "")
            fileNameInputField.text = "Unnamed Character";

        File.WriteAllBytes(Directory.GetCurrentDirectory() + "/Saved Characters/" + fileNameInputField.text + ".png", bytes);

        if (LootlockerAuthenticationManager.LoggedIn)
        {
            await UniTask.WaitUntil(() => doneGrabbingServerData == true);

            personalStatsText.text = "You've saved " + charactersGeneratedPersonal.ToString("N0") + " characters total.";
            globalStatsText.text = charactersGeneratedGlobal.ToString("N0") + " characters have been saved globally.";

            savingCharacter = false;

            OnAfterCharacterSaved?.Invoke(this, EventArgs.Empty);

            creatingCharacterOverlay.SetActive(false);
            OpenCharacterSavedPopup();
        }
        else
        {
            personalStatsText.text = "";
            globalStatsText.text = "";

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
            List<Task> tasks = new()
            {
               UpdateGlobalScore(),
               UpdatePersonalScore()
            };

            await Task.WhenAll(tasks);

            doneGrabbingServerData = true;
        }

        async Task UpdateGlobalScore()
        {
            int score = 0;

            bool finished = false;

            bool succesful = true;
            LootLockerSDKManager.GetScoreList("19386", 1, 0, (response) =>
            {
                if (response.success)
                {
                    score = response.items[0].score;
                }
                else
                {
                    Debug.LogWarning("Failed to fetch leaderbaord data: " + response.errorData.message);
                    succesful = false;
                }

                finished = true;
            });

            await UniTask.WaitUntil(() => finished);

            if (!succesful) return;

            bool done = false;

            // Add to global score
            LootLockerSDKManager.SubmitScore("155", score + 1, "19386", (response) =>
            {
                if (response.success)
                {
                    charactersGeneratedGlobal = score + 1;
                    //Debug.Log(score + " Global");
                }
                else
                {
                    Debug.Log("Failed" + response.errorData.message);
                }

                done = true;
            });

            await UniTask.WaitUntil(() => done);
        }

        async Task UpdatePersonalScore()
        {
            int score = 0;

            bool finished = false;

            bool succesful = true;
            LootLockerSDKManager.GetMemberRank("19387", PlayerPrefs.GetString("PlayerID"), (response) =>
            {
                if (response.success)
                    score = response.score;
                else
                {
                    Debug.LogWarning("Failed to fetch leaderbaord data: " + response.errorData.message);
                    succesful = false;
                }

                finished = true;
            });

            await UniTask.WaitUntil(() => finished);

            if (!succesful) return;

            bool done = false;

            LootLockerSDKManager.SubmitScore(PlayerPrefs.GetString("PlayerID"), score + 1, "19387", (response) =>
            {
                if (response.success)
                {
                    charactersGeneratedPersonal = score + 1;
                    //Debug.Log(score + " Pesonal");
                }
                else
                    Debug.LogWarning("Unable to upload personal score: " + response.errorData.message);

                done = true;
            });

            await UniTask.WaitUntil(() => done);
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

            string filePath = "";

            for (int i = 0; i < characterPieceDatabase.ActiveCharacterType.CharacterPieces.Length; i++)
            {
                if (characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite != null)
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        CharacterPieceDatabase.CharacterPiecesFolderName,
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