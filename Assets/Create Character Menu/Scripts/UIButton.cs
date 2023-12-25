using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image image;

    [Space]

    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Color pressedColor;

    [Space]

    [SerializeField] UnityEvent onClick;

    Sprite defaultSprite;
    Color defaultColor;

    private void Awake()
    {
        defaultSprite = image.sprite;
        defaultColor = image.color;
    }

    private void OnEnable()
    {
        image.sprite = defaultSprite;
        image.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightedSprite;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;
        StopAllCoroutines();
        OnPointerUp(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
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

    public virtual void OnPointerUp(PointerEventData eventData)
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

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            OnPointerExit(new PointerEventData(EventSystem.current));
    }
}