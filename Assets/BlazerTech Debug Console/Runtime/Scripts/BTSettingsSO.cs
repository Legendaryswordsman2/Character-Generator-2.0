using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlazerTech
{
    public enum ActiveInputSystem { None, OldInputSystem }
    public class BTSettingsSO : ScriptableObject
    {
        [Header("Settings")]
        [Tooltip("The max amount of logs that can be displayed at a given time, once the limit has been reached logs will begin getting replaced, the higher the number of logs the laggier the game")]
        [Range(1, 1000)] public int logCap = 250;
        [Tooltip("The font size of each log in the console")]
        [Min(1)] public float fontSize = 25;
        [Tooltip("The max amount of log files that can be generated, once this limit has been reached older logs will start being overwritten"), Min(0)]
        public int logFileCap = 5;
        [Tooltip("The color of the debug consoles background")]
        public Color backgroundColor = new Color(0, 0, 0, 0.55f);
        [Tooltip("Whether or not to clear the console of all logs when the current scene changes")]
        public bool clearConsoleOnSceneChange = true;

        [Space]

        [Tooltip("The input system to use to toggle the debug console on/off")]
        public ActiveInputSystem activeInputSystem = ActiveInputSystem.OldInputSystem;
        [Tooltip("The keybind used to open/close the debug console")]
        public KeyCode toggleConsoleKey = KeyCode.F2;

        //Behind the scenes
        private static BTSettingsSO settingsInstance;

        public static BTSettingsSO Get()
        {
            if (settingsInstance != null)
                return settingsInstance;
            else
            {
                //Try to load it
                settingsInstance = Resources.Load<BTSettingsSO>("BTSettings");

                if (settingsInstance != null)
                    return settingsInstance;
            }

            // Create a new Config
            settingsInstance = ScriptableObject.CreateInstance<BTSettingsSO>();

            // Folder needs to exist for Unity to be able to create an asset in it
            string dir = Application.dataPath + "/BlazerTech Debug Console/Resources";

            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Create config asset
#if UNITY_EDITOR
            string configAssetPath = "Assets/BlazerTech Debug Console/Resources/BTSettings.asset";
            AssetDatabase.CreateAsset(settingsInstance, configAssetPath);
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
            AssetDatabase.Refresh();
#endif

            return settingsInstance;
        }
    }
}
