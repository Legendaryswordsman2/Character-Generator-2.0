using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;

public class SaveCharacterManager : MonoBehaviour
{
    [SerializeField] GameObject creatingCharacterOverlay;
    [SerializeField] TMP_Text[] contentTexts;

    [Space]

    [SerializeField] TMP_InputField fileNameInputField;
    [SerializeField] TMP_Dropdown sizeDropdown;
    [SerializeField] Button saveCharacterButton;

    //CharacterDropdownManager characterDropdownManager;
    CharacterPieceDatabase characterPieceDatabase;
    CharacterPieceGrabber characterPieceGrabber;

    bool savingCharacter = false;

    private void Start()
    {
        //characterDropdownManager = CharacterDropdownManager.Instance;
        characterPieceDatabase = CharacterPieceDatabase.Instance;
        characterPieceGrabber = CharacterPieceGrabber.Instance;
    }
    public void OpenPopup()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.zero;

        creatingCharacterOverlay.SetActive(false);

        fileNameInputField.interactable = true;
        sizeDropdown.interactable = true;
        saveCharacterButton.interactable = true;

        foreach (TMP_Text text in contentTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }

        transform.parent.gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, 0.1f);
    }

    public void ClosePopup()
    {
        if (savingCharacter) return;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.zero, 0.075f).setOnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
        });
    }

    public async void SaveCharacter()
    {
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

        if (characterPiecesToBeCombined.Count <= 0)
        {
            Debug.LogWarning("No character pieces to be combined");
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

        savingCharacter = false;
    }

    void SaveCharacterToFile(Texture2D texture)
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

        ClosePopup();
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

                    characterPiecesToBeCombined.Add(await characterPieceGrabber.GetImageAsTexture2D(filePath + ".png",
                        characterPieceDatabase.ActiveCharacterType.CharacterPieces[i].ActiveSprite.name,
                        ".png", characterSize, characterPieceDatabase.ActiveCharacterType));
                }
            }
        }

        return characterPiecesToBeCombined;
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