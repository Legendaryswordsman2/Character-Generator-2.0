using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RandomizeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] CharacterDropdownManager characterDropdownManager;

    [SerializeField] Image image;

    [Space]

    [SerializeField] Sprite highlightedSprite;

    Sprite defaultSprite;

    private void Awake()
    {
        defaultSprite = image.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RandomizeCharacter();
    }

    public void RandomizeCharacter()
    {
        characterDropdownManager.CanRecreateCharacter = false;
        characterDropdownManager.RandomizeAllDropdowns();
        characterDropdownManager.CanRecreateCharacter = true;

        characterDropdownManager.RecreateCharacter();
    }
}