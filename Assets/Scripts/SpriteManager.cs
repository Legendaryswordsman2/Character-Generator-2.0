using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static Sprite CombineTextures(List<Texture2D> texturesToBeCombined)
    {
        if (texturesToBeCombined.Count == 0)
        {
            Debug.LogError("No textures supplied to be combined");
            return null;
        }

        // If only one texture is provided convert it into a sprite and return it.
        if (texturesToBeCombined.Count == 1) return ConvertTextureToSprite(texturesToBeCombined[0]);

        Texture2D finalTexture = texturesToBeCombined[0];

        //Texture2D finalTexture = new(texturesToBeCombined[0].width, texturesToBeCombined[0].height);

        for (int i = 1; i < texturesToBeCombined.Count; i++)
        {
            finalTexture = CombineTwoTextures(finalTexture, texturesToBeCombined[i]);
        }

        return ConvertTextureToSprite(finalTexture);
    }

    public static Texture2D CombineTwoTextures(Texture2D _texture1, Texture2D texture2)
    {
        Texture2D texture1 = Instantiate(_texture1);

        Color32[] pixels1 = texture1.GetPixels32();
        Color32[] pixels2 = texture2.GetPixels32();

        int width = texture1.width;
        int height = texture1.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                pixels1[index] = Color32.Lerp(pixels1[index], pixels2[index], pixels2[index].a / 255.0f);
            }
        }

        texture1.SetPixels32(pixels1);
        texture1.Apply();

        return texture1;
    }

    public static void OverrideSprite(Texture2D spriteToOverride, Texture2D spriteToOverrideWith)
    {
        int startX = 0;
        int startY = 0;

        int width = spriteToOverride.width - startX;
        int height = spriteToOverride.height - startY;

        Color32[] pixelsOverride = spriteToOverride.GetPixels32();
        Color32[] pixelsOverrideWith = spriteToOverrideWith.GetPixels32();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int indexOverride = (y + startY) * spriteToOverride.width + (x + startX);
                int indexOverrideWith = y * width + x;

                pixelsOverride[indexOverride] = pixelsOverrideWith[indexOverrideWith];
            }
        }

        spriteToOverride.SetPixels32(pixelsOverride);
        spriteToOverride.Apply();
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static Texture2D CreateBlankTexture(int width, int height)
    {
        Texture2D blankTexture = new(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        // Create an array of Color32 with all pixels set to transparent
        Color32[] blankColors = new Color32[width * height];
        for (int i = 0; i < blankColors.Length; i++)
        {
            blankColors[i] = new Color32(0, 0, 0, 0); // Transparent color
        }

        // Set all pixels at once
        blankTexture.SetPixels32(blankColors);

        // Apply changes
        blankTexture.Apply();

        return blankTexture;
    }

    public static Texture2D ExtractTextureRegion(Texture2D sourceTexture, int startX, int startY, int width, int height)
    {
        Color32[] sourcePixels = sourceTexture.GetPixels32();
        Color32[] regionPixels = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int sourceIndex = (sourceTexture.height - 1 - (startY + y)) * sourceTexture.width + (startX + x);
                int destinationIndex = (height - 1 - y) * width + x; // Reverse the order for the y-coordinate
                regionPixels[destinationIndex] = sourcePixels[sourceIndex];
            }
        }

        Texture2D extractedTexture = new(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        extractedTexture.SetPixels32(regionPixels);
        extractedTexture.Apply();

        return extractedTexture;
    }

    public static void SaveTextureToFile(Texture2D texture, string filePath, string fileName)
    {
        if (texture == null)
        {
            Debug.LogWarning("Can't save texture, texture is null");
            return;
        }

        byte[] bytes = texture.EncodeToPNG();

        if (!Directory.Exists(Application.persistentDataPath + "/" + filePath))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + filePath);

        File.WriteAllBytes(Application.persistentDataPath + "/" + filePath + "/" + fileName + ".png", bytes);
    }
}