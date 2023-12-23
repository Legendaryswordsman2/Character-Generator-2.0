using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEditor;

public enum CharacterSize { Sixteen, Thirtytwo, Fortyeight }
public enum LoadFailedType { DirectoryMissing, UnknownError }
public class CharacterPieceGrabber : MonoBehaviour
{
    public static CharacterPieceGrabber Instance;

    [SerializeField] CharacterPieceDatabase characterPieceDatabase;

    List<Task> tasks;

    public static event EventHandler OnAllCharacterPiecesLoaded;
    public static event EventHandler<LoadFailedType> OnFailedToLoadCharacterPieces;

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

    public string LastFailedToGetSpriteName { get; private set; }

    private void Awake() => Instance = this;
    private async void Start() => await Start_Task();

    async Task Start_Task()
    {
        try
        {
            if (!PerformErrorChecks()) return;

            try
            {
                foreach (CharacterTypeSO characterType in characterPieceDatabase.CharacterTypes)
                {
                    await LoadCharacterPiecesFromType(characterType);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                OnFailedToLoadCharacterPieces?.Invoke(this, LoadFailedType.UnknownError);
                throw;
            }

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
            Debug.LogError("Directory does not exist: " + Directory.GetCurrentDirectory() + "/" + CharacterPieceDatabase.CharacterPiecesFolderName);
            OnFailedToLoadCharacterPieces?.Invoke(this, LoadFailedType.DirectoryMissing);
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
                Debug.LogWarning("Filepath does not exist: " + filePath);
                continue;
            }

            DirectoryInfo d = new(filePath);

            foreach (var file in d.GetFiles("*.png"))
            {
                // file.FullName is the full path to the file
                string fileUrl = new Uri(file.FullName).AbsoluteUri;
                Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension, CharacterSize.Sixteen, characterType);
                if (sprite == null) continue;

                //if (sprite.texture.width != characterType.SpriteSize.x || sprite.texture.height != characterType.SpriteSize.y)
                //{
                //    Debug.Log("Sprite (" + sprite.name + file.Extension + ") has an incorrect size and cannot be loaded");
                //    continue;
                //}

                item.Sprites.Add(sprite);
                //characterPieceDatabase.AddCharacterPiece(sprite, type);
            }
        }
    }

    public async Task<Sprite> GetImage(string filepath, string fileName, string extenion, CharacterSize size, CharacterTypeSO characterType)
    {
        if (!File.Exists(Uri.UnescapeDataString(new Uri(filepath).LocalPath)))
        {
            string sizeString = "";
            switch (size)
            {
                case CharacterSize.Sixteen:
                    sizeString = "16x16";
                    break;
                case CharacterSize.Thirtytwo:
                    sizeString = "32x32";
                    break;
                case CharacterSize.Fortyeight:
                    sizeString = "48x48";
                    break;
            }

            Debug.LogWarning($"Can't find sprite ({fileName}) at: {filepath} ({sizeString})");
            LastFailedToGetSpriteName = fileName;
            return null;
        }

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

            switch (size)
            {
                case CharacterSize.Sixteen:
                    if (texture.width != characterType.SpriteSize_16x16.x || texture.height != characterType.SpriteSize_16x16.y)
                    {
                        Debug.Log("Sprite (" + fileName + extenion + ") has an incorrect size and cannot be loaded (16x16)");
                        return null;
                    }
                    break;
                case CharacterSize.Thirtytwo:
                    if (texture.width != characterType.SpriteSize_32x32.x || texture.height != characterType.SpriteSize_32x32.y)
                    {
                        Debug.Log("Sprite (" + fileName + extenion + ") has an incorrect size and cannot be loaded (32x32)");
                        return null;
                    }
                    break;
                case CharacterSize.Fortyeight:
                    if (texture.width != characterType.SpriteSize_48x48.x || texture.height != characterType.SpriteSize_48x48.y)
                    {
                        Debug.Log("Sprite (" + fileName + extenion + ") has an incorrect size and cannot be loaded (48x48)");
                        return null;
                    }
                    break;
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16f);

            sprite.name = fileName;
            sprite.texture.name = fileName;
            sprite.texture.filterMode = FilterMode.Point;

            OnNewSpriteLoaded?.Invoke(this, new OnNewSpriteLoadedEventArgs(sprite, extenion));

            return sprite;
        }
    }

    public async Task<Texture2D> GetImageAsTexture2D(string filepath, string fileName, string extenion, CharacterSize size, CharacterTypeSO characterType)
    {
        if (!File.Exists(Uri.UnescapeDataString(new Uri(filepath).LocalPath)))
        {
            string sizeString = "";
            switch (size)
            {
                case CharacterSize.Sixteen:
                    sizeString = "16x16";
                    break;
                case CharacterSize.Thirtytwo:
                    sizeString = "32x32";
                    break;
                case CharacterSize.Fortyeight:
                    sizeString = "48x48";
                    break;
            }

            Debug.LogWarning($"Can't find texture ({fileName}) at: {filepath} ({sizeString})");
            LastFailedToGetSpriteName = fileName;
            return null;
        }

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

            switch (size)
            {
                case CharacterSize.Sixteen:
                    if (texture.width != characterType.SpriteSize_16x16.x || texture.height != characterType.SpriteSize_16x16.y)
                    {
                        Debug.Log("Texture (" + fileName + extenion + ") has an incorrect size and cannot be loaded (16x16) || Current Size " + texture.width + "x" + texture.height + " || Must be Size : " + characterType.SpriteSize_16x16.x + "x" + characterType.SpriteSize_16x16.y);
                        return null;
                    }
                    break;
                case CharacterSize.Thirtytwo:
                    if (texture.width != characterType.SpriteSize_32x32.x || texture.height != characterType.SpriteSize_32x32.y)
                    {
                        Debug.Log("Texture (" + fileName + extenion + ") has an incorrect size and cannot be loaded (32x32) || Current Size " + texture.width + "x" + texture.height + " || Must be Size : " + characterType.SpriteSize_32x32.x + "x" + characterType.SpriteSize_32x32.y);
                        return null;
                    }
                    break;
                case CharacterSize.Fortyeight:
                    if (texture.width != characterType.SpriteSize_48x48.x || texture.height != characterType.SpriteSize_48x48.y)
                    {
                        Debug.Log("Texture (" + fileName + extenion + ") has an incorrect size and cannot be loaded (48x48) || Current Size " + texture.width + "x" + texture.height + " || Must be Size : " + characterType.SpriteSize_48x48.x + "x" + characterType.SpriteSize_48x48.y);
                        return null;
                    }
                    break;
            }

            return texture;
        }
    }
}