using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    GameObject activeTab;
    public GameObject ActiveTab
    {
        get
        {
            return activeTab;
        }

        set
        {
            if (activeTab != null)
                activeTab.SetActive(false);

            activeTab = value;
            activeTab.SetActive(true);
            OnActiveTabChanged?.Invoke(this, activeTab);
        }
    }

    public event EventHandler<GameObject> OnActiveTabChanged;

    [SerializeField] GameObject defaultTab;

    [Space]

    [SerializeField] Transform bottomPOS;

    private void Start()
    {
        if (defaultTab != null)
            ActiveTab = defaultTab;

        TryCharacterButtonManager.MoveAllUIOffScreen += TryCharacterButtonManager_MoveAllUIOffScreen;
    }

    private void TryCharacterButtonManager_MoveAllUIOffScreen(object sender, float time)
    {
        LeanTween.moveY(gameObject, bottomPOS.position.y, time);
    }

    private void OnDestroy()
    {
        TryCharacterButtonManager.MoveAllUIOffScreen -= TryCharacterButtonManager_MoveAllUIOffScreen;
    }
}