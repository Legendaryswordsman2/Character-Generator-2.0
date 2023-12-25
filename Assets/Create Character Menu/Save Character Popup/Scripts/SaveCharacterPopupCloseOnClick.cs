using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveCharacterPopupCloseOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SaveCharacterManager saveCharacterManager;
    [SerializeField] InfoMenuManager infoMenuManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (saveCharacterManager.gameObject.activeSelf)
            saveCharacterManager.ClosePopup();
        else if (infoMenuManager.gameObject.activeSelf)
            infoMenuManager.OnCloseMenuCalled();
    }
}