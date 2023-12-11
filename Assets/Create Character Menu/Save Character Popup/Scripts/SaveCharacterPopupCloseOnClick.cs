using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveCharacterPopupCloseOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SaveCharacterManager saveCharacterManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        saveCharacterManager.ClosePopup();
    }
}