using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class UIDropdown : TMP_Dropdown
{
    public override void OnSelect(BaseEventData eventData)
    {
        
    }

    //protected override async void DestroyDropdownList(GameObject dropdownList)
    //{
    //    base.DestroyDropdownList(dropdownList);
    //    await UniTask.NextFrame();
    //    Show();
    //}
}