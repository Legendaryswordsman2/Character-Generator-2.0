using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class UIDropdown : TMP_Dropdown
{
    //TMP_Dropdown dropdown;

    //bool pointerInside = false;
    public override void OnSelect(BaseEventData eventData)
    {

    }

    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    base.OnPointerEnter(eventData);
    //    pointerInside = true;
    //}

    //public override void OnPointerExit(PointerEventData eventData)
    //{
    //    base.OnPointerExit(eventData);
    //    pointerInside = false;
    //}

    //protected override async void DestroyDropdownList(GameObject dropdownList)
    //{
    //    base.DestroyDropdownList(dropdownList);

    //    //if (pointerInside)
    //    //{
    //    //    await UniTask.NextFrame();
    //    //    Show();
    //    //}
    //}
}