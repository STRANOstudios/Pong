using UnityEditor;
using UnityEngine;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Custom editor for ColliderVisualizer to organize parameters inside a foldout.
    /// </summary>
    [CustomEditor(typeof(ColliderVisualizer), true)]
    public class ColliderVisualizerEditor : Editor
    {
        private bool showGizmoSettings = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw non-collider fields first (other fields from subclasses)
            DrawPropertiesExcluding(serializedObject, "m_displayMode", "m_showColliders", "m_showCollidersOnlyEnabled", "m_gizmoColor");

            // Foldout for collider visualization settings
            showGizmoSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGizmoSettings, "Gizmo Visualization Settings");

            if (showGizmoSettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_displayMode"), new GUIContent("Display Mode"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_showColliders"), new GUIContent("Show Colliders"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_showCollidersOnlyEnabled"), new GUIContent("Show Only Enabled Colliders"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_gizmoColor"), new GUIContent("Gizmo Color"));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
