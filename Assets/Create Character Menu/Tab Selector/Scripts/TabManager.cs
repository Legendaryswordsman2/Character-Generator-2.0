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

    private void Start()
    {
        if (defaultTab != null)
            ActiveTab = defaultTab;
    }
}