using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TabManager tabManager;
    [SerializeField] GameObject tabReference;

    [Space]

    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    [Space]

    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Color pressedColor;

    Sprite defaultSprite;
    Color defaultColor;

    private void Awake()
    {
        defaultSprite = image.sprite;
        defaultColor = image.color;

        tabReference.SetActive(false);

        tabManager.OnActiveTabChanged += TabManager_OnActiveTabChanged;

        //if (characterPreviewAnimation != null)
        //    text.text = characterPreviewAnimation.characterAnimationName;
        //else
        //{
        //    enabled = false;
        //}
    }

    private void TabManager_OnActiveTabChanged(object sender, GameObject g)
    {
        if (tabManager.ActiveTab == tabReference)
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
        //if (characterPreviewAnimationManager.CurrentCharacterAnimation != characterPreviewAnimation)

        if (tabManager.ActiveTab == tabReference) return;

        image.sprite = defaultSprite;
        StopAllCoroutines();
        OnPointerUp(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabManager.ActiveTab = tabReference;
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

    private void OnDestroy()
    {
        tabManager.OnActiveTabChanged -= TabManager_OnActiveTabChanged;
    }
}