using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoMenuManager : MonoBehaviour
{
    public void OpenMenu()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.zero;

        transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, 0.1f);
    }

    public void CloseMenu()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.zero, 0.075f).setOnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
    }
}