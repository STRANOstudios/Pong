using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;

namespace PsychoGarden.TriggerEvents
{
    public class TriggerEventConnectionPopup : PopupWindowContent
    {
        private readonly GameObject target;
        private readonly List<(GameObject owner, TriggerEvent triggerEvent, Color cachedColor)> cache;
        private Vector2 scrollPos;

        private const float MaxHeight = 150f;
        private const float RowHeight = 22f;
        private const float ColorBoxWidth = 16f;
        private const float ColorBoxHeight = 16f;
        private List<(string display, Color color, GameObject owner)> filteredList;

        public TriggerEventConnectionPopup(GameObject target, List<(GameObject, TriggerEvent, Color)> cache)
        {
            this.target = target;
            this.cache = cache;
            BuildConnectionList();
        }

        private void BuildConnectionList()
        {
            filteredList = new();

            foreach ((GameObject owner, TriggerEvent triggerEvent, Color color) in cache)
            {
                if (triggerEvent == null || owner == null)
                {
                    continue;
                }

                int count = triggerEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    Object t = triggerEvent.GetPersistentTarget(i);
                    GameObject go = (t as Component)?.gameObject ?? t as GameObject;
                    if (go == target)
                    {
                        string targetMethod = triggerEvent.GetPersistentMethodName(i);

                        string caller = triggerEvent.GetType().Name;

                        string display = $"{owner.name} | {caller} | {targetMethod}";

                        Color colorSelected = triggerEvent.EditorOverrideGlobalSettings ? color : TriggerEventSettings.EditorColor;
                        filteredList.Add((display, colorSelected, owner));
                    }
                }
            }
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Incoming TriggerEvent Connections", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach ((string name, Color color, GameObject owner) in filteredList)
            {
                EditorGUILayout.BeginHorizontal();

                // Fixed 16x16 color box
                Rect colorRect = GUILayoutUtility.GetRect(ColorBoxWidth, ColorBoxHeight, GUILayout.Width(ColorBoxWidth), GUILayout.Height(ColorBoxHeight));
                EditorGUI.DrawRect(colorRect, color);

                // Name with interaction
                Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(name), EditorStyles.label, GUILayout.ExpandWidth(true));
                EditorGUI.LabelField(labelRect, name);

                if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.clickCount == 2)
                    {
                        Selection.activeGameObject = owner;

                        if (owner.TryGetComponent<Renderer>(out var r))
                        {
                            SceneView.lastActiveSceneView.Frame(r.bounds, true);
                        }
                        else
                        {
                            SceneView.lastActiveSceneView.Frame(new Bounds(owner.transform.position, Vector3.one * 0.5f), true);
                        }

                        Event.current.Use();
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(owner);
                        Event.current.Use();
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(2);
            }

            EditorGUILayout.Space(2);

            EditorGUILayout.EndScrollView();
        }

        public override Vector2 GetWindowSize()
        {
            float height = Mathf.Min(MaxHeight, filteredList.Count * RowHeight + 30f);
            return new Vector2(300, height);
        }
    }

}
#endif
