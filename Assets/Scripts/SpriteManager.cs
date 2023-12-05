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

        for (int i = 1; i < texturesToBeCombined.Count; i++)
        {
            finalTexture = CombineTwoTextures(finalTexture, texturesToBeCombined[i]);
        }

        return ConvertTextureToSprite(finalTexture);
    }

    public static Texture2D CombineTwoTextures(Texture2D _texture1, Texture2D texture2)
    {
        Texture2D texture1 = Instantiate(_texture1);

        int startX = 0;
        int startY = 0;

        for (int x = startX; x < texture1.width; x++)
        {
            for (int y = startY; y < texture1.height; y++)
            {
                Color s1Color = texture1.GetPixel(x, y);
                Color s2Color = texture2.GetPixel(x - startX, y - startY);

                Color final_color = Color.Lerp(s1Color, s2Color, s2Color.a / 1.0f);

                texture1.SetPixel(x, y, final_color);
            }
        }
        texture1.Apply();

        return texture1;
    }

    public static Sprite OverrideSprite(Sprite spriteToOverride, Sprite spriteToOverrideWith)
    {
        int startX = 0;
        int startY = 0;

        for (int x = startX; x < spriteToOverride.texture.width; x++)
        {

            for (int y = startY; y < spriteToOverride.texture.height; y++)
            {
                Color s2Color = spriteToOverrideWith.texture.GetPixel(x - startX, y - startY);

                spriteToOverride.texture.SetPixel(x, y, s2Color);
            }
        }
        spriteToOverride.texture.Apply();

        Sprite combinedSprite = Sprite.Create(spriteToOverride.texture, new Rect(0.0f, 0.0f, spriteToOverride.texture.width, spriteToOverride.texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        return combinedSprite;
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}