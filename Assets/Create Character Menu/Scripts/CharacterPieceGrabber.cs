using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public enum CharacterSize { Sixteen, Thirtytwo, Fortyeight }
public class CharacterPieceGrabber : MonoBehaviour
{
    public static CharacterPieceGrabber Instance;

    [SerializeField] CharacterPieceDatabase characterPieceDatabase;

    List<Task> tasks;

    public static event EventHandler OnAllCharacterPiecesLoaded;
    public static event EventHandler OnFailedToLoadCharacterPieces;

    public static event EventHandler<OnNewSpriteLoadedEventArgs> OnNewSpriteLoaded;

    public class OnNewSpriteLoadedEventArgs
    {
        public Sprite Sprite;
        public string Extention;

        public OnNewSpriteLoadedEventArgs(Sprite sprite, string extention)
        {
            Sprite = sprite;
            Extention = extention;
        }
    }

    private void Awake() => Instance = this;
    private async void Start() => await Start_Task();

    async Task Start_Task()
    {
        try
        {
            if (!PerformErrorChecks()) return;

            foreach (CharacterTypeSO characterType in characterPieceDatabase.CharacterTypes)
            {
                await LoadCharacterPiecesFromType(characterType);
            }

            //await LoadCharacterPiecesFromType(characterPieceDatabase.CharacterTypes[1]);
            //await LoadCharacterPiecesFromType(characterPieceDatabase)

            //await GetCharacterpieceCollection(CharacterPieceType.Body);
            //await GetCharacterpieceCollection(CharacterPieceType.Eyes);
            //await GetCharacterpieceCollection(CharacterPieceType.Outfit);
            //await GetCharacterpieceCollection(CharacterPieceType.Hairstyle);
            //await GetCharacterpieceCollection(CharacterPieceType.Accessory);

            Debug.Log("Successfully loaded all sprites with a total load time of: " + Time.realtimeSinceStartup);
            OnAllCharacterPiecesLoaded?.Invoke(this, EventArgs.Empty);

        }
        catch
        {
            throw;
        }
    }

    bool PerformErrorChecks()
    {
        if (!Directory.Exists(Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName))
        {
            Debug.Log("Directory does not exist");
            OnFailedToLoadCharacterPieces?.Invoke(this, EventArgs.Empty);
            //SetupManager.Instance.DisplayError(ErrorType.MissingPortraitPiecesFolder);
            return false;
        }
        return true;
    }

    async Task LoadCharacterPiecesFromType(CharacterTypeSO characterType)
    {
        foreach (var item in characterType.CharacterPieces)
        {
            string filePath = "";

            filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, item.spriteLocation, "16x16");

            if (!Directory.Exists(filePath)) 
            {
                Debug.LogWarning("Filepath does not exist: " +  filePath);
                continue;
            } 

            DirectoryInfo d = new(filePath);

            foreach (var file in d.GetFiles("*.png"))
            {
                // file.FullName is the full path to the file
                string fileUrl = new Uri(file.FullName).AbsoluteUri;
                Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension, CharacterSize.Sixteen);
                if (sprite == null) continue;
                item.Sprites.Add(sprite);
                //characterPieceDatabase.AddCharacterPiece(sprite, type);
            }
        }
    }

    async Task GetCharacterpieceCollection(CharacterPieceType type)
    {
        string filePath = "";

        switch (type)
        {
            case CharacterPieceType.Body:
                filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, "Bodies", "16x16");
                //filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Bodies/16x16";
                break;
            case CharacterPieceType.Eyes:
                filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, "Eyes", "16x16");
                //filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Eyes/16x16";
                break;
            case CharacterPieceType.Outfit:
                filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, "Outfits", "16x16");
                //filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Outfits/16x16";
                break;
            case CharacterPieceType.Hairstyle:
                filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, "Hairstyles", "16x16");
                //filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Hairstyles/16x16";
                break;
            case CharacterPieceType.Accessory:
                filePath = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName, "Accessories", "16x16");
                //filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Accessories/16x16";
                break;
        }

        if (!Directory.Exists(filePath)) return;

        DirectoryInfo d = new(filePath);

        foreach (var file in d.GetFiles("*.png"))
        {
            // file.FullName is the full path to the file
            string fileUrl = new Uri(file.FullName).AbsoluteUri;
            Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension, CharacterSize.Sixteen);
            if (sprite == null) continue;
            //characterPieceDatabase.AddCharacterPiece(sprite, type);
        }
    }

    public async Task<Sprite> GetImage(string filepath, string fileName, string extenion, CharacterSize size)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filepath);

        await uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(uwr.error);
            return null;
        }
        else
        {
            // Get downloaded asset bundle
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

            if (!ConfirmImageSize(texture)) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16f);

            sprite.name = fileName;
            sprite.texture.name = fileName;
            sprite.texture.filterMode = FilterMode.Point;

            OnNewSpriteLoaded?.Invoke(this, new OnNewSpriteLoadedEventArgs(sprite, extenion));

            return sprite;
        }
    }

    bool ConfirmImageSize(Texture2D texture)
    {
        //switch (size)
        //{
        //    case CharacterSize.Sixteen:
        //        if (texture.width != sixteenXSixsteenImageSize.width || texture.height != sixteenXSixsteenImageSize.height)
        //        {
        //            Debug.LogWarning("IncorrectSizeException: Tried to get image of size 16x16 (Image name: " + fileName + ") but it's size is incorrect (" + texture.width + " | " + texture.height + ")");
        //            return false;
        //        }
        //        break;
        //    case CharacterSize.Thirtytwo:
        //        if (texture.width != thirtyTwoXThirtyTwoImageSize.width || texture.height != thirtyTwoXThirtyTwoImageSize.height)
        //        {
        //            Debug.LogWarning("IncorrectSizeException: Tried to get image of size 32x32 (Image name: " + fileName + ") but it's size is incorrect");
        //            return false;
        //        }
        //        break;
        //    case CharacterSize.Fortyeight:
        //        if (texture.width != fortyEightXFortyEightImageSize.width || texture.height != fortyEightXFortyEightImageSize.height)
        //        {
        //            Debug.LogWarning("IncorrectSizeException: Tried to get image of size 48x48 (Image name: " + fileName + ") but it's size is incorrect");
        //            return false;
        //        }
        //        break;
        //}

        return true;
    }
}