namespace AndreaFrigerio.Core.Runtime.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Event binder asset.
    /// </summary>
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [CreateAssetMenu(menuName = "Events/Multi Script Event Binder")]
    public class EventBinderAsset : ScriptableObject
    {
        [InfoBox("Assign one or more scripts to scan for static or instance events.")]
        [Required, LabelText("Target Scripts")]
        [OnValueChanged("ScanAll")]
        public List<MonoScript> m_targetScripts = new();

        [PropertySpace(10)]
        [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "@ScriptName")]
        public List<ScriptEventGroup> m_eventGroups = new();

        private readonly Dictionary<(string, string), Delegate> boundDelegates = new();

#if UNITY_EDITOR
        #region Scanning

        [Button(ButtonSizes.Large)]
        private void ScanAll()
        {
            foreach (MonoScript monoScript in m_targetScripts)
            {
                if (monoScript == null)
                {
                    continue;
                }

                Type type = monoScript.GetClass();
                if (type == null)
                {
                    continue;
                }

                ScriptEventGroup group = m_eventGroups.FirstOrDefault(g => g.Script == monoScript);
                if (group == null)
                {
                    group = new ScriptEventGroup
                    {
                        Script = monoScript,
                        ScriptName = type.Name,
                        Bindings = new List<GenericEventBinding>()
                    };
                    m_eventGroups.Add(group);
                }

                EventInfo[] events = type.GetEvents(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance
                );

                foreach (EventInfo evt in events)
                {
                    Type handlerType = evt.EventHandlerType;

                    bool supported = handlerType == typeof(Action) ||
                                     handlerType == typeof(UnityAction) ||
                                     (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(Action<>));

                    if (!supported)
                    {
                        continue;
                    }

                    if (group.Bindings.Any(b => b.EventName == evt.Name))
                    {
                        continue;
                    }

                    group.Bindings.Add(new GenericEventBinding
                    {
                        EventName = evt.Name,
                        TargetEventTypeName = handlerType.AssemblyQualifiedName,
                        IsStatic = evt.GetAddMethod(true)?.IsStatic ?? false
                    });
                }
            }

            Debug.Log($"[EventBinderAsset] Scan complete. {m_eventGroups.Count} event groups updated.");
        }

        #endregion
#endif

        #region Binding

        /// <summary>
        /// Binds all events.
        /// </summary>
        public void BindAll()
        {
            foreach (ScriptEventGroup group in m_eventGroups)
            {
                Type type = group.Script?.GetClass();
                if (type == null)
                {
                    continue;
                }

                foreach (GenericEventBinding binding in group.Bindings)
                {
                    EventInfo evt = type.GetEvent(
                        binding.EventName, 
                        BindingFlags.Public | 
                        BindingFlags.NonPublic | 
                        BindingFlags.Static | 
                        BindingFlags.Instance
                    );

                    if (evt == null)
                    {
                        continue;
                    }

                    Type handlerType = Type.GetType(binding.TargetEventTypeName);
                    if (handlerType == null)
                    {
                        continue;
                    }

                    Delegate handler = null;

                    if (handlerType == typeof(Action) || handlerType == typeof(UnityAction))
                    {
                        handler = (Action)(() => binding.Response?.Invoke());
                    }
                    else if (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(Action<>))
                    {
                        Type argType = handlerType.GetGenericArguments()[0];
                        MethodInfo method = GetType().GetMethod(
                            nameof(CreateGenericAction), 
                            BindingFlags.NonPublic | 
                            BindingFlags.Instance
                        )?.MakeGenericMethod(argType);
                        handler = method?.Invoke(this, new object[] { binding }) as Delegate;
                    }

                    if (handler != null)
                    {
                        try
                        {
                            UnityEngine.Object target = binding.IsStatic ? null : group.InstanceTarget;

                            if (!binding.IsStatic && target == null)
                            {
                                Debug.LogWarning($"Instance event '{binding.EventName}' requires 'instanceTarget' on {group.ScriptName}");
                                continue;
                            }

                            MethodInfo addMethod = evt.GetAddMethod(true);
                            addMethod?.Invoke(target, new object[] { handler });

                            (string name, string eventName) key = (group.Script.name, binding.EventName);
                            boundDelegates[key] = handler;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error binding '{evt.Name}' in {group.ScriptName}: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unbinds all events.
        /// </summary>
        public void UnbindAll()
        {
            foreach (ScriptEventGroup group in m_eventGroups)
            {
                Type type = group.Script?.GetClass();
                if (type == null)
                {
                    continue;
                }

                foreach (GenericEventBinding binding in group.Bindings)
                {
                    EventInfo evt = type.GetEvent(
                        binding.EventName, 
                        BindingFlags.Public | 
                        BindingFlags.NonPublic | 
                        BindingFlags.Static | 
                        BindingFlags.Instance
                    );
                    if (evt == null)
                    {
                        continue;
                    }

                    (string name, string eventName) key = (group.Script.name, binding.EventName);
                    if (!boundDelegates.TryGetValue(key, out var handler) || handler == null)
                    {
                        continue;
                    }

                    try
                    {
                        UnityEngine.Object target = binding.IsStatic ? null : group.InstanceTarget;
                        MethodInfo removeMethod = evt.GetRemoveMethod(true);
                        removeMethod?.Invoke(target, new object[] { handler });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error unbinding '{evt.Name}' in {group.ScriptName}: {ex.Message}");
                    }
                }
            }

            boundDelegates.Clear();
        }

        #endregion

        #region Helpers

        private Delegate CreateGenericAction<T>(GenericEventBinding binding)
        {
            return (Action<T>)(_ => binding.Response.Invoke());
        }

        #endregion
    }
}
