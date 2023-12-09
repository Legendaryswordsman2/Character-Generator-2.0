using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] Toggle fullscreenToggle;

    //Vector2 windowedResolution;
    Resolution windowedResolution;
    private void Start()
    {
        windowedResolution = Screen.currentResolution;

        fullscreenToggle.isOn = Screen.fullScreen;
    }
    public void SetFullscreen(bool fullscreen)
    {
        if (fullscreen)
        {
            windowedResolution = Screen.currentResolution;
            Screen.SetResolution(Screen.resolutions[^1].width, Screen.resolutions[^1].height, fullscreen);
        }
        {
            Screen.SetResolution(windowedResolution.width, windowedResolution.height, fullscreen);
        }
    }
}