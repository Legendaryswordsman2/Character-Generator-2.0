using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HistoryTabCharacterPreviewButton : UIButton
{
    public override async void OnPointerExit(PointerEventData eventData)
    {
        await UniTask.NextFrame();
        base.OnPointerExit(eventData);
    }

    public override async void OnPointerUp(PointerEventData eventData)
    {
        await UniTask.NextFrame();
        base.OnPointerUp(eventData);
    }
}