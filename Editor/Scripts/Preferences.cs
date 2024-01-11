using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace InversiveSdkEditor
{
    public class Preferences
    {
        public enum ShowOption
        {
            Always = 0,
            OnNewVersion = 1,
            Never = 2
        }

        private static readonly GUIContent StartUp = new GUIContent("Show manage screen on Unity launch", "You can set if you want to see the start screen everytime Unity launchs, only just when there's a new version available or never.");
        public static readonly string PrefStartUp = "InversiveSdkLastSession" + Application.productName;
        public static ShowOption GlobalStartUp = ShowOption.Always;
        private static bool PrefsLoaded = false;

#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider ImpostorsSettings()
        {
            var provider = new SettingsProvider("Preferences/Inversive SDK Editor", SettingsScope.User)
            {
                guiHandler = (string searchContext) =>
                {
                    PreferencesGUI();
                },

                keywords = new HashSet<string>(new[] { "start" }),

            };
            return provider;
        }
#else
		[PreferenceItem( "Inversive SDK Editor" )]
#endif
        public static void PreferencesGUI()
        {
            if (!PrefsLoaded)
            {
                LoadDefaults();
                PrefsLoaded = true;
            }

            var cache = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;
            {
                EditorGUI.BeginChangeCheck();
                GlobalStartUp = (ShowOption)EditorGUILayout.EnumPopup(StartUp, GlobalStartUp);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(PrefStartUp, (int)GlobalStartUp);
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset and Forget All"))
            {
                EditorPrefs.DeleteKey(PrefStartUp);
                GlobalStartUp = ShowOption.Always;       
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = cache;
        }

        public static void LoadDefaults()
        {
            GlobalStartUp = (ShowOption)EditorPrefs.GetInt(PrefStartUp, 0);
        }
    }

}
