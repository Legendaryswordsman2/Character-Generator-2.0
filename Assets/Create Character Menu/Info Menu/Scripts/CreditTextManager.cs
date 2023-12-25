using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditTextManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TMP_Text creditText;

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL("https://linktr.ee/legendaryswordsman2");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        creditText.fontStyle = FontStyles.Underline;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        creditText.fontStyle = FontStyles.Normal;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            OnPointerExit(new PointerEventData(EventSystem.current));
    }
}