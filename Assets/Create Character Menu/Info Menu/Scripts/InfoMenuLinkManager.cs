using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMenuLinkManager : MonoBehaviour
{
    public void OpenAboutMeLink()
    {
        Application.OpenURL("https://linktr.ee/legendaryswordsman2");
    }

    public void OpenSourceCodeLink()
    {
        Application.OpenURL("https://github.com/Legendaryswordsman2/Character-Generator-2.0");
    }
}