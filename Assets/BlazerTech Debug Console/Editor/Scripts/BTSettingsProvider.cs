using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BlazerTech
{
#if UNITY_EDITOR
    public class BTSettingsProvider : SettingsProvider
    {
        private static BTSettingsSO settings;
        private SerializedObject m_CustomSettings;
        internal static SerializedObject GetSerializedSettings()
        {
            if (settings == null)
            {
                settings = BTSettingsSO.Get();
            }
            return new SerializedObject(settings);
        }

        public BTSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        { }

        public override void OnGUI(string searchContext)
        {
            //base.OnGUI(searchContext);

            m_CustomSettings.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("logCap"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.logCap = m_CustomSettings.FindProperty("logCap").intValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("fontSize"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.fontSize = m_CustomSettings.FindProperty("fontSize").floatValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("logFileCap"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.logFileCap = m_CustomSettings.FindProperty("logFileCap").intValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("backgroundColor"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.backgroundColor = m_CustomSettings.FindProperty("backgroundColor").colorValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("clearConsoleOnSceneChange"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.clearConsoleOnSceneChange = m_CustomSettings.FindProperty("clearConsoleOnSceneChange").boolValue;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("activeInputSystem"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.activeInputSystem = (ActiveInputSystem)m_CustomSettings.FindProperty("activeInputSystem").enumValueFlag;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("toggleConsoleKey"));
            if (EditorGUI.EndChangeCheck())
            {
                settings.toggleConsoleKey = (KeyCode)m_CustomSettings.FindProperty("toggleConsoleKey").enumValueFlag;
            }

            m_CustomSettings.ApplyModifiedProperties();
        }


        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if(settings == null)
            settings = BTSettingsSO.Get();

            m_CustomSettings = GetSerializedSettings();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new BTSettingsProvider("Project/BlazerTech Debug Console", SettingsScope.Project);
        }
    }

#endif
}
