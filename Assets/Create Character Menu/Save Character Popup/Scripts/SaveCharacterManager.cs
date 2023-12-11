using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveCharacterManager : MonoBehaviour
{
    [SerializeField] GameObject creatingCharacterOverlay;
    [SerializeField] TMP_Text[] contentTexts;

    [Space]

    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_Dropdown sizeDropdown;
    [SerializeField] Button saveCharacterButton;
    public void OpenPopup()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.zero;

        creatingCharacterOverlay.SetActive(false);

        nameInputField.interactable = true;
        sizeDropdown.interactable = true;
        saveCharacterButton.interactable = true;

        foreach (TMP_Text text in contentTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }

        transform.parent.gameObject.SetActive(true);

        LeanTween.scale(gameObject, Vector2.one, 0.1f);
    }

    public void ClosePopup()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.zero, 0.075f).setOnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
        });
    }

    public void SaveCharacter()
    {
        creatingCharacterOverlay.SetActive(true);

        nameInputField.interactable = false;
        sizeDropdown.interactable = false;
        saveCharacterButton.interactable = false;

        foreach (TMP_Text text in contentTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePopup();
    }
}