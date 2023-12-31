using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    [Space]

    [SerializeField] SpriteRenderer[] spriteRenderers;

    [Button]
    void Randomize()
    {
        if (sprites.IsNullOrEmpty() || spriteRenderers.IsNullOrEmpty()) return;

        for (int i = 0; i < sprites.Length; i++)
        {
            spriteRenderers[i].sprite = sprites[Random.Range(0, sprites.Length - 1)];
        }
    }
}