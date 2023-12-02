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

        //    tasks = new List<Task>
        //{
        //    GetCharacterpieceCollection(CharacterPieceType.Body),
        //    GetCharacterpieceCollection(CharacterPieceType.Eyes),
        //    GetCharacterpieceCollection(CharacterPieceType.Outfit),
        //    GetCharacterpieceCollection(CharacterPieceType.Hairstyle),
        //    GetCharacterpieceCollection(CharacterPieceType.Accessory),
        //};

            // Wait for all portrait pieces to be added
            //await Task.WhenAll(tasks);
            //foreach (var task in tasks)
            //{
            //    await task;
            //    await UniTask.Delay(1000);
            //}

            await GetCharacterpieceCollection(CharacterPieceType.Body);
            //await UniTask.Delay(1000);
            await GetCharacterpieceCollection(CharacterPieceType.Eyes);
            //await UniTask.Delay(1000);
            await GetCharacterpieceCollection(CharacterPieceType.Outfit);
            //await UniTask.Delay(1000);
            await GetCharacterpieceCollection(CharacterPieceType.Hairstyle);
            //await UniTask.Delay(1000);
            await GetCharacterpieceCollection(CharacterPieceType.Accessory);
            //await UniTask.Delay(1000);

            Debug.Log("Successfully loaded all sprites");
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

    async Task GetCharacterpieceCollection(CharacterPieceType type)
    {
        string filePath = "";

        switch (type)
        {
            case CharacterPieceType.Body:
                filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Bodies/16x16";
                break;
            case CharacterPieceType.Eyes:
                filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Eyes/16x16";
                break;
            case CharacterPieceType.Outfit:
                filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Outfits/16x16";
                break;
            case CharacterPieceType.Hairstyle:
                filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Hairstyles/16x16";
                break;
            case CharacterPieceType.Accessory:
                filePath = Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName + "/Accessories/16x16";
                break;
        }

        if (!CheckDirectory(filePath)) return;

        DirectoryInfo d = new(filePath);

        foreach (var file in d.GetFiles("*.png"))
        {
            // file.FullName is the full path to the file
            string fileUrl = new Uri(file.FullName).AbsoluteUri;
            Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension, CharacterSize.Sixteen);
            if (sprite == null) continue;
            characterPieceDatabase.AddCharacterPiece(sprite, type);
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

    bool CheckDirectory(string path)
    {
        if (Directory.Exists(path))
            return true;
        else
            return false;
    }
}