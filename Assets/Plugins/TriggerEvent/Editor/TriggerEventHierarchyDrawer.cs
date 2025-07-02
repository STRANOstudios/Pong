using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;

namespace PsychoGarden.TriggerEvents
{
    [InitializeOnLoad]
    public static class TriggerEventHierarchyDrawer
    {
        private static List<(GameObject owner, TriggerEvent triggerEvent, Color cachedColor)> triggerEventsCache = new();
        private static double lastCacheTime;
        private const double cacheRefreshInterval = 0.2f;

        static TriggerEventHierarchyDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowOnGUI;
            SceneView.duringSceneGui += DrawSceneView;
            EditorApplication.update += () =>
            {
                RefreshCacheIfNeeded();
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            };
        }

        private static void RefreshCacheIfNeeded()
        {
            if (EditorApplication.timeSinceStartup - lastCacheTime < cacheRefreshInterval)
            {
                return;
            }

            triggerEventsCache.Clear();

            foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go == null)
                {
                    continue;
                }

                foreach (MonoBehaviour comp in go.GetComponents<MonoBehaviour>())
                {
                    if (comp == null)
                    {
                        continue;
                    }

                    FieldInfo[] fields = comp.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType == typeof(TriggerEvent))
                        {
                            if (field.GetValue(comp) is TriggerEvent triggerEvent && triggerEvent != null)
                            {
                                triggerEvent.CheckAuto(comp);
                                triggerEventsCache.Add((go, triggerEvent, triggerEvent.EditorColor));
                            }
                        }
                    }
                }
            }

            TriggerEventHandles.RebuildTargetMapping(triggerEventsCache);
            lastCacheTime = EditorApplication.timeSinceStartup;
        }

        private static void HierarchyWindowOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                return;
            }

            // Show popup if this GameObject is the target of any TriggerEvent
            foreach ((GameObject owner, TriggerEvent triggerEvent, Color cachedColor) in triggerEventsCache)
            {
                if (triggerEvent == null || owner == null)
                {
                    continue;
                }

                int count = triggerEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    Object targetObj = triggerEvent.GetPersistentTarget(i);
                    if (targetObj == go || (targetObj is Component comp && comp.gameObject == go))
                    {
                        Rect iconRect = new Rect(selectionRect.x - 25f, selectionRect.y + 2f, 12f, 12f);
                        GUIContent icon = EditorGUIUtility.IconContent("BlendTree Icon");
                        Texture iconTexture = icon.image;

                        // Draw rotated icon
                        if (iconTexture != null)
                        {
                            Matrix4x4 oldMatrix = GUI.matrix;
                            Vector2 pivot = iconRect.center;

                            GUIUtility.RotateAroundPivot(180f, pivot);
                            GUI.DrawTexture(iconRect, iconTexture);
                            GUI.matrix = oldMatrix;
                        }

                        // Invisible button on top
                        if (GUI.Button(iconRect, GUIContent.none, GUIStyle.none))
                        {
                            PopupWindow.Show(iconRect, new TriggerEventConnectionPopup(go, triggerEventsCache));
                        }

                        return;
                    }
                }
            }
        }

        private static void DrawSceneView(SceneView sceneView)
        {
            foreach ((GameObject owner, TriggerEvent triggerEvent, Color _) in triggerEventsCache)
            {
                if (triggerEvent == null || owner == null)
                {
                    continue;
                }

                if (!TriggerEventDrawer.EvaluateShouldDrawFor(triggerEvent, owner))
                {
                    continue;
                }

                TriggerEventHandles.DrawConnectionGizmos(triggerEvent, owner);
            }
        }
    }

}
#endif
