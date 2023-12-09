using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterTypeSelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] CharacterTypeSO characterType;

    [Space]

    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    [Space]

    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Color pressedColor;

    Sprite defaultSprite;
    Color defaultColor;

    CharacterPieceDatabase characterPieceDatabase;

    private void Awake()
    {
        defaultSprite = image.sprite;
        defaultColor = image.color;
    }

    private void Start()
    {
        characterPieceDatabase = CharacterPieceDatabase.Instance;

        text.text = characterType.CharacterTypeName;

        if (characterType.Equals(characterPieceDatabase.ActiveCharacterType))
            image.sprite = highlightedSprite;
        else
            image.sprite = defaultSprite;

        characterPieceDatabase.OnActiveCharacterTypeChanged += CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }

    private void CharacterPieceDatabase_OnActiveCharacterTypeChanged(object sender, CharacterTypeSO e)
    {
        if (characterType.Equals(characterPieceDatabase.ActiveCharacterType))
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
        if (characterType.Equals(characterPieceDatabase.ActiveCharacterType))
            image.sprite = highlightedSprite;
        else
            image.sprite = defaultSprite;

        StopAllCoroutines();
        OnPointerUp(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        characterPieceDatabase.SetActiveCharacterType(characterType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            float lerpDuration = 0.05f;
            float elapsedTime = 0f;

            Color startColor = image.color;

            while (elapsedTime < lerpDuration)
            {
                image.color = Color.Lerp(startColor, pressedColor, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();

            float lerpDuration = 0.05f;
            float elapsedTime = 0f;

            Color startColor = image.color;

            while (elapsedTime < lerpDuration)
            {
                image.color = Color.Lerp(startColor, defaultColor, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            image.color = defaultColor;
        }
    }

    private void OnDestroy()
    {
        characterPieceDatabase.OnActiveCharacterTypeChanged -= CharacterPieceDatabase_OnActiveCharacterTypeChanged;
    }
}