using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpSubmenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] pages;

    int index;

    private void OnEnable()
    {
        pages[index].SetActive(false);
        pages[0].SetActive(true);

        index = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextPage();
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PrevPage();
    }
    public void NextPage()
    {
        if (index == pages.Length - 1) return;

        pages[index].SetActive(false);
        pages[index + 1].SetActive(true);

        index++;
    }

    public void PrevPage()
    {
        if (index == 0) return;

        pages[index].SetActive(false);
        pages[index - 1].SetActive(true);

        index--;
    }
    public void JoinDiscord()
    {
        Application.OpenURL("https://discord.com/invite/2wB3RuAESb");
    }
}