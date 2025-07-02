#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PsychoGarden.TriggerEvents
{
    [CustomPropertyDrawer(typeof(TriggerEvent), true)]
    public class TriggerEventDrawer : PropertyDrawer
    {
        private object unityEventDrawer;
        private MethodInfo unityEventDrawerOnGUIMethod;
        private MethodInfo unityEventDrawerGetHeightMethod;
        private bool initialized;

        private static readonly Dictionary<string, bool> foldoutStates = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.HelpBox(position, "Multi-object editing is not supported.", MessageType.Info);
                return;
            }

            Initialize(property);

            string foldoutKey = property.propertyPath;
            if (!foldoutStates.ContainsKey(foldoutKey))
            {
                foldoutStates[foldoutKey] = false;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float padding = 2f;
            float y = position.y;

            Rect foldoutRect = new Rect(position.x, y, position.width, lineHeight);
            foldoutStates[foldoutKey] = EditorGUI.Foldout(foldoutRect, foldoutStates[foldoutKey], "Trigger Settings");

            y += lineHeight + padding;

            if (foldoutStates[foldoutKey])
            {
                SerializedProperty overrideProp = property.FindPropertyRelative("EditorOverrideGlobalSettings");
                SerializedProperty modeProp = property.FindPropertyRelative("EditorDisplayMode");
                SerializedProperty colorProp = property.FindPropertyRelative("EditorColor");

                Rect overrideRect = new Rect(position.x, y, position.width, lineHeight);
                overrideProp.boolValue = EditorGUI.ToggleLeft(overrideRect, "Override Global Settings", overrideProp.boolValue);
                y += lineHeight + padding;

                EditorGUI.BeginDisabledGroup(!overrideProp.boolValue);

                if (modeProp != null)
                {
                    Rect modeRect = new Rect(position.x, y, position.width, lineHeight);
                    EditorGUI.PropertyField(modeRect, modeProp, new GUIContent("Display Mode"));
                    y += lineHeight + padding;
                }

                if (colorProp != null)
                {
                    Rect colorRect = new Rect(position.x, y, position.width, lineHeight);
                    colorProp.colorValue = EditorGUI.ColorField(colorRect, new GUIContent("Connection Color"), colorProp.colorValue);
                    y += lineHeight + padding;
                }

                EditorGUI.EndDisabledGroup();

                y += 3f;
            }

            Rect eventRect = new Rect(position.x, y, position.width, position.yMax - y);

            if (unityEventDrawer != null && unityEventDrawerOnGUIMethod != null)
            {
                unityEventDrawerOnGUIMethod.Invoke(unityEventDrawer, new object[] { eventRect, property, label });
            }
            else
            {
                EditorGUI.PropertyField(eventRect, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            const float padding = 2f;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            float height = lineHeight + padding;

            string key = property.propertyPath;
            bool open = foldoutStates.TryGetValue(key, out var v) && v;
            if (open)
                height += (lineHeight + padding) * 3 + 3f;

            float eventHeight = unityEventDrawer != null && unityEventDrawerGetHeightMethod != null
                ? (float)unityEventDrawerGetHeightMethod.Invoke(unityEventDrawer, new object[] { property, label })
                : EditorGUI.GetPropertyHeight(property, label, true);

            return height + eventHeight;
        }

        private void Initialize(SerializedProperty property)
        {
            if (initialized)
                return;

            Type unityEventDrawerType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditorInternal.UnityEventDrawer");
            if (unityEventDrawerType != null)
            {
                try
                {
                    unityEventDrawer = Activator.CreateInstance(unityEventDrawerType, true);
                }
                catch
                {
                    Debug.LogWarning("[TriggerEventDrawer] Could not instantiate internal UnityEventDrawer.");
                }

                unityEventDrawerOnGUIMethod = unityEventDrawerType.GetMethod(
                    "OnGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new[] { typeof(Rect), typeof(SerializedProperty), typeof(GUIContent) }, null);

                unityEventDrawerGetHeightMethod = unityEventDrawerType.GetMethod(
                    "GetPropertyHeight", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new[] { typeof(SerializedProperty), typeof(GUIContent) }, null);
            }

            initialized = true;
        }

        // --- Evaluation Helpers ----------------------------------------------------------------

        public static bool EvaluateShouldDrawFor(TriggerEvent triggerEvent, GameObject owner)
        {
            return triggerEvent != null && (triggerEvent.EditorOverrideGlobalSettings
                ? ShouldDrawFor(triggerEvent.EditorDisplayMode, owner)
                : ShouldDrawFor(TriggerEventSettings.GizmoMode, owner));
        }

        private static bool ShouldDrawFor(TriggerEvent.DisplayMode mode, GameObject owner)
        {
            return mode switch
            {
                TriggerEvent.DisplayMode.Everything => true,
                TriggerEvent.DisplayMode.None => false,
                TriggerEvent.DisplayMode.Selected => Selection.activeGameObject == owner,
                _ => false,
            };
        }
    }
}
#endif
