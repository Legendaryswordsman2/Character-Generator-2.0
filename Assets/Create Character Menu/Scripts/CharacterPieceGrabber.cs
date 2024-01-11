using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEditor;
using Sirenix.Utilities;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

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

    public static bool AllCharacterPiecesLoaded { get; private set; } = false;

    public static int totalSpritesInBatch;
    public static int loadedSpritesFromBatch;

    private void Awake() => Instance = this;
    private async void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        if (SceneManager.GetActiveScene().buildIndex == 0)
            await Start_Task();
    }

    private async void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        await UniTask.NextFrame();
        //await UniTask.Delay(1000);
        if (AllCharacterPiecesLoaded)
        {
            //Debug.Log("Calling Character Pieces Loaded Event");
            OnAllCharacterPiecesLoaded?.Invoke(this, EventArgs.Empty);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0)
            await Start_Task();
    }

    async Task Start_Task()
    {
        try
        {
            if (!PerformErrorChecks()) return;

            DateTime startTime = DateTime.Now;
            try
            {
                foreach (CharacterTypeSO characterType in characterPieceDatabase.CharacterTypes)
                {
                    await LoadCharacterPiecesFromType(characterType);
                }

                foreach (CharacterTypeSO characterType in characterPieceDatabase.CharacterTypes)
                {
                    await LoadSaveCharacterHistoryFromType(characterType);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                OnFailedToLoadCharacterPieces?.Invoke(this, LoadFailedType.UnknownError);
                throw;
            }

            Debug.Log("Successfully loaded all sprites with a total load time of: " + (DateTime.Now - startTime).TotalSeconds + " seconds");
            AllCharacterPiecesLoaded = true;
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

            FileInfo[] files = d.GetFiles("*.png");

            totalSpritesInBatch = files.Length;
            loadedSpritesFromBatch = 0;

            foreach (var file in files)
            {
                // file.FullName is the full path to the file
                string fileUrl = new Uri(file.FullName).AbsoluteUri;
                Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension, CharacterSize.Sixteen, characterType);
                if (sprite == null) continue;

                item.Sprites.Add(sprite);
            }
        }
    }

    async Task LoadSaveCharacterHistoryFromType(CharacterTypeSO characterType)
    {
        string filePath = "";

        filePath = Path.Combine(Application.persistentDataPath, characterType.CharacterTypeName + " Character Save History");

        if (!Directory.Exists(filePath))
        {
            //Debug.LogWarning("Filepath does not exist: " + filePath);
            return;
        }

        List<Sprite> loadedSprites = new();
        List<int[]> characterbackupSaveData = SaveSystem.LoadFile<List<int[]>>("/" + characterType.CharacterTypeName + " Character Save History/" + characterType.CharacterTypeName + " Character Piece Values");

        if (characterbackupSaveData.IsNullOrEmpty()) return;

        //Debug.Log(characterbackupSaveData.Count);

        DirectoryInfo d = new(filePath);

        var pngFiles = d.GetFiles("*.png")
    .OrderBy(f =>
        int.Parse(Regex.Match(f.Name, @"\d+").Value)
    );

        foreach (var file in pngFiles)
        {
            // file.FullName is the full path to the file
            string fileUrl = new Uri(file.FullName).AbsoluteUri;
            Sprite sprite = await GetImage(fileUrl, Path.GetFileNameWithoutExtension(file.Name), file.Extension);
            if (sprite == null) continue;

            loadedSprites.Add(sprite);
            //item.
            //item.Sprites.Add(sprite);
        }
        //Debug.Log(loadedSprites.Count);

        int count;

        if (characterbackupSaveData.Count > loadedSprites.Count)
            count = loadedSprites.Count;
        else
            count = characterbackupSaveData.Count;

        for (int i = 0; i < count; i++)
        {
            characterType.CharacterSaveHistory.Add(new CharacterTypeSO.CharacterBackup(loadedSprites[i], characterbackupSaveData[i]));
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

            loadedSpritesFromBatch++;

            OnNewSpriteLoaded?.Invoke(this, new OnNewSpriteLoadedEventArgs(sprite, extenion));

            return sprite;
        }
    }
    public async Task<Sprite> GetImage(string filepath, string fileName, string extenion)
    {
        if (!File.Exists(Uri.UnescapeDataString(new Uri(filepath).LocalPath)))
        {
            Debug.LogWarning($"Can't find sprite ({fileName}) at: {filepath}");
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

            //Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16f);
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

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }
}