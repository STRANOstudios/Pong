using UnityEngine;
using static PsychoGarden.TriggerEvents.TriggerEvent;

#if UNITY_EDITOR
using UnityEditor;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Static helper class to manage global settings for TriggerEvent visualization using EditorPrefs.
    /// </summary>
    public static class TriggerEventSettings
    {
        private const string GizmoModeKey = "PsychoGarden.TriggerEvent.GizmoMode";
        private const string EditorColorKey = "PsychoGarden.TriggerEvent.EditorColor";
        private const string ThicknessKey = "PsychoGarden.TriggerEvent.Thickness";

        public static readonly DisplayMode DefaultGizmoMode = DisplayMode.Selected;
        public static readonly Color DefaultEditorColor = Color.green;
        public static readonly float DefaultGizmoThickness = 1f;

        public static DisplayMode GizmoMode
        {
            get => (DisplayMode)EditorPrefs.GetInt(GizmoModeKey, (int)DefaultGizmoMode);
            set => EditorPrefs.SetInt(GizmoModeKey, (int)value);
        }

        private static bool _cached;
        private static Color _cachedColor;

        public static Color EditorColor
        {
            get
            {
                if (_cached) return _cachedColor;

                string colorString = EditorPrefs.GetString(EditorColorKey, ColorUtility.ToHtmlStringRGBA(DefaultEditorColor));
                _cachedColor = ColorUtility.TryParseHtmlString($"#{colorString}", out Color c) ? c : DefaultEditorColor;
                _cached = true;
                return _cachedColor;
            }
            set
            {
                _cachedColor = value;
                _cached = true;
                EditorPrefs.SetString(EditorColorKey, ColorUtility.ToHtmlStringRGBA(value));
            }
        }

        public static float GizmoThickness
        {
            get => EditorPrefs.GetFloat(ThicknessKey, DefaultGizmoThickness);
            set => EditorPrefs.SetFloat(ThicknessKey, value);
        }

        public static void ResetToDefaults()
        {
            EditorPrefs.DeleteKey(GizmoModeKey);
            EditorPrefs.DeleteKey(EditorColorKey);
            EditorPrefs.DeleteKey(ThicknessKey);

            GizmoMode = DefaultGizmoMode;
            EditorColor = DefaultEditorColor;
            GizmoThickness = DefaultGizmoThickness;
        }

        [SettingsProvider]
        public static SettingsProvider CreateTriggerEventPreferences()
        {
            var provider = new SettingsProvider("Preferences/Psycho Garden/Trigger Event", SettingsScope.User)
            {
                label = "Trigger Event",
                guiHandler = (searchContext) =>
                {

                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("Gizmo Settings", EditorStyles.boldLabel);
                    DisplayMode mode = (DisplayMode)EditorGUILayout.EnumPopup("Mode", GizmoMode);
                    if (mode != GizmoMode)
                    {
                        GizmoMode = mode;
                    }

                    EditorGUILayout.Space(3);

                    Color color = EditorGUILayout.ColorField("Color", EditorColor);
                    if (color != EditorColor)
                    {
                        EditorColor = color;
                    }

                    EditorGUILayout.Space(3);

                    float thickness = EditorGUILayout.FloatField("Thickness", GizmoThickness);
                    if (!Mathf.Approximately(thickness, GizmoThickness))
                    {
                        GizmoThickness = thickness;
                    }

                    #region Reset Button

                    if (GUILayout.Button("Reset to Defaults"))
                    {
                        ResetToDefaults();
                    }

                    #endregion

                    EditorGUI.indentLevel--;
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] {
                    "trigger", "event", "gizmo", "psycho", "garden", "color", "thickness"
                })
            };

            return provider;
        }
    }
}
#endif
