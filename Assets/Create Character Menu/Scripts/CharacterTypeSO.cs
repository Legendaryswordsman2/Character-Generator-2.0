using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Character Type", fileName = "New Character Type")]
public class CharacterTypeSO : ScriptableObject
{
    [field: SerializeField] public string CharacterTypeName { get; private set; }

    [field: SerializeField] public Sprite CharacterPreviewSpritesheet { get; private set; }

    [field: SerializeField] public CharacterPieceCollection[] CharacterPieces { get; private set; }

    public void ClearSprites()
    {
        foreach (CharacterPieceCollection characterPiece in CharacterPieces)
        {
            characterPiece.Sprites.Clear();
        }
    }

    [System.Serializable]
    public class CharacterPieceCollection
    {
        public string CollectionName;

        [Tooltip("The localation on the users computer where the sprites are located")]
        public string spriteLocation;

        [field: Space]

        [field: SerializeField] public bool IncludeNAOption { get; private set; } = false;
        [field: SerializeField, ShowIf("IncludeNAOption")] public bool NADefault { get; private set; } = true;

        [Space]

        [ReadOnly] public List<Sprite> Sprites;

        [field: SerializeField, ReadOnly] public Sprite ActiveSprite { get; private set; }
        public bool CanRandomize { get; set; } = true;

        public event EventHandler<int> OnRandomizeDropdown;

        public void SetActiveSprite(int index)
        {
            if (Sprites.Count == 0) return;

            if (IncludeNAOption)
            {
                if (index == 0)
                    ActiveSprite = null;
                else
                    ActiveSprite = Sprites[index - 1];
            }
            else
            {
                ActiveSprite = Sprites[index];
            }

        }

        public void Randomize()
        {
            if (CanRandomize && Sprites.Count > 0)
                OnRandomizeDropdown?.Invoke(this, UnityEngine.Random.Range(0, Sprites.Count));
            //dropdown.value = UnityEngine.Random.Range(0, Sprites.Count);
        }
    }
}