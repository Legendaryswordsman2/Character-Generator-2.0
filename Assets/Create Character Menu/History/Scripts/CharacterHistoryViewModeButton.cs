using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterHistoryViewModeButton : UIButton
{
    [SerializeField] HistoryViewMode historyViewMode;

    [Space]

    [SerializeField] HistoryTabManager HistoryTabManager;

    protected override void Awake()
    {
        base.Awake();
        HistoryTabManager.OnHistoryViewModeChanged += HistoryTabManager_OnHistoryViewModeChanged;
    }

    private void HistoryTabManager_OnHistoryViewModeChanged(object sender, System.EventArgs e)
    {
        if (historyViewMode == HistoryTabManager.HistoryViewMode)
        {
            image.sprite = highlightedSprite;
        }
        else
            image.sprite = defaultSprite;

            //image.color = defaultColor;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (historyViewMode == HistoryTabManager.HistoryViewMode) return;

        base.OnPointerExit(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (historyViewMode == HistoryTabManager.HistoryViewMode) return;

        base.OnPointerUp(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (historyViewMode == HistoryTabManager.HistoryViewMode) return;

        base.OnPointerClick(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (historyViewMode == HistoryTabManager.HistoryViewMode) return;

        base.OnPointerDown(eventData);
    }

    public void SetHistoryViewMode()
    {
        HistoryTabManager.ChangeHistoryViewMode(historyViewMode);
    }

    private void OnDestroy()
    {
        HistoryTabManager.OnHistoryViewModeChanged -= HistoryTabManager_OnHistoryViewModeChanged;
    }
}