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
    [SerializeField]protected Image image;

    [Space]

    [SerializeField] protected Sprite highlightedSprite;
    [SerializeField] protected Color pressedColor;

    [Space]

    [SerializeField] UnityEvent onClick;

    protected Sprite defaultSprite;
    protected Color defaultColor;

    protected virtual void Awake()
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

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
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

            image.color = defaultColor;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            OnPointerExit(new PointerEventData(EventSystem.current));
    }
}