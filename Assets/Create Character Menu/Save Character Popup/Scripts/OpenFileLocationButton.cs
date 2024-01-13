using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenFileLocationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image image;

    [Space]

    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Color pressedColor;

    Sprite defaultSprite;
    Color defaultColor;

    private void Awake()
    {
        defaultSprite = image.sprite;
        defaultColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;
        StopAllCoroutines();
        OnPointerUp(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            if (!Directory.Exists(CharacterPieceDatabase.SavedCharactersDirectory))
                Directory.CreateDirectory(CharacterPieceDatabase.SavedCharactersDirectory);

            Application.OpenURL(CharacterPieceDatabase.SavedCharactersDirectory);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"Cannot open file explorer to '{CharacterPieceDatabase.SavedCharactersFolderName}': {exception}");
        }

        //Debug.LogWarning($"Cannot open file explorer to '{CharacterPieceDatabase.SavedCharactersFolderName}' folder because that folder does not exist");
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

    public async void OnPointerUp(PointerEventData eventData)
    {
        await UniTask.NextFrame();
        StopAllCoroutines();
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            float lerpDuration = 0.05f;
            float elapsedTime = 0f;

            Color startColor = image.color;

            while (elapsedTime < lerpDuration)
            {
                image.color = Color.Lerp(startColor, defaultColor, elapsedTime / lerpDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            OnPointerExit(new PointerEventData(EventSystem.current));
    }
}