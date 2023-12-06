using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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

        //int startX = 0;
        //int startY = 0;

        //int startX = 0;
        //int startY = 0;

        //int endX = 896;
        //int endY = 32;

        //for (int x = startX; x < texture1.width; x++)
        //{
        //    for (int y = startY; y < texture1.height; y++)
        //    {
        //        Color s1Color = texture1.GetPixel(x, y);
        //        Color s2Color = texture2.GetPixel(x - startX, y - startY);

        //        Color final_color = Color.Lerp(s1Color, s2Color, s2Color.a / 1.0f);

        //        texture1.SetPixel(x, y, final_color);
        //    }
        //}

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

        //for (int x = startX; x < endX; x++)
        //{
        //    for (int y = startY; y < endY; y++)
        //    {
        //        Color s1Color = texture1.GetPixel(x, y);
        //        Color s2Color = texture2.GetPixel(x, y);

        //        Color final_color = Color.Lerp(s1Color, s2Color, s2Color.a / 1.0f);

        //        texture1.SetPixel(x, y, final_color);
        //    }
        //}
        //texture1.Apply();

        return texture1;
    }

    public static void OverrideSprite(Texture2D spriteToOverride, Texture2D spriteToOverrideWith)
    {
        int startX = 0;
        int startY = 0;

        //spriteToOverride = spriteToOverrideWith;

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

        //Sprite combinedSprite = Sprite.Create(spriteToOverride, new Rect(0.0f, 0.0f, spriteToOverride.width, spriteToOverride.height), new Vector2(0.5f, 0.5f), 100.0f);

        //return combinedSprite;
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}