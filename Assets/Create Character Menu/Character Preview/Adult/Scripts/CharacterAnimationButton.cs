using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterAnimationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] CharacterPreviewAnimationSetSO characterPreviewAnimation;
    [SerializeField] CharacterPreviewAnimationManager characterPreviewAnimationManager;

    [Space]

    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    [SerializeField] Sprite highlightedSprite;

    Sprite defaultSprite;

    private void Awake()
    {
        defaultSprite = image.sprite;

        if (characterPreviewAnimation != null)
            text.text = characterPreviewAnimation.characterAnimationName;
        else
        {
            enabled = false;
        }
    }

    private void Start()
    {
        if (characterPreviewAnimationManager == null) return;

        if (characterPreviewAnimationManager.CurrentCharacterAnimation == characterPreviewAnimation)
            image.sprite = highlightedSprite;
        else
            image.sprite = defaultSprite;

        if (characterPreviewAnimation != null)
            characterPreviewAnimationManager.OnAnimationChanged += CharacterPreviewAnimationManager_OnAnimationChanged;
    }

    private void CharacterPreviewAnimationManager_OnAnimationChanged(object sender, CharacterPreviewAnimationSetSO _characterPreviewAnimation)
    {
        if (_characterPreviewAnimation == characterPreviewAnimation)
            image.sprite = highlightedSprite;
        else
            image.sprite = defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (characterPreviewAnimationManager.CurrentCharacterAnimation != characterPreviewAnimation)
            image.sprite = defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetCharacterAnimation();
    }

    public void SetCharacterAnimation()
    {
        if (characterPreviewAnimation != null && characterPreviewAnimationManager != null)
            characterPreviewAnimationManager.SetCharacterAnimation(characterPreviewAnimation);
    }

    private void OnDestroy()
    {
        if (characterPreviewAnimation != null && characterPreviewAnimationManager != null)
            characterPreviewAnimationManager.OnAnimationChanged -= CharacterPreviewAnimationManager_OnAnimationChanged;
    }
}