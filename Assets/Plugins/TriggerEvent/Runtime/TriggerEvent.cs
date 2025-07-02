using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Custom UnityEvent that auto-checks for recursion and manages connection metadata.
    /// </summary>
    [Serializable]
    public class TriggerEvent : UnityEvent<Transform>
    {
        #region Fields ---------------------------------------------------------------------------

#if UNITY_EDITOR
        [HideInInspector] public bool EditorOverrideGlobalSettings = true;
        [HideInInspector] public DisplayMode EditorDisplayMode = DisplayMode.Everything;
        [HideInInspector] public Color EditorColor = Color.green;
#endif

        private const bool HARD_BLOCK_SELF_REFERENCE = true;

        /// <summary>
        /// Defines how the event connections should be displayed.
        /// </summary>
        public enum DisplayMode
        {
            Everything, // Default - Show all connections everywhere
            None,       // Don't show any connections
            Selected    // Show connections only when selected
        }

        #endregion

        #region Public Methods -------------------------------------------------------------------

        /// <summary>
        /// Automatically checks and optionally removes self-references in the event listeners.
        /// </summary>
        /// <param name="callingInstance">The instance calling this method.</param>
        public void CheckAuto(Object callingInstance)
        {
            if (callingInstance == null)
                return;

            if (HARD_BLOCK_SELF_REFERENCE)
            {
                CheckAndRemoveSelfReference(callingInstance);
            }
            else
            {
                CheckSelfReference(callingInstance);
            }
        }

        #endregion

        #region Private Methods ------------------------------------------------------------------

        /// <summary>
        /// Logs a warning if self-references are detected.
        /// </summary>
        private void CheckSelfReference(Object callingInstance)
        {
            foreach (var target in GetPersistentTargetObjects())
            {
                if (target == callingInstance)
                {
                    Debug.LogWarning($"[TriggerEvent] '{callingInstance.name}' has a self-reference!", callingInstance);
                }
            }
        }

        /// <summary>
        /// Automatically removes self-referencing listeners to prevent recursion.
        /// </summary>
        private void CheckAndRemoveSelfReference(Object callingInstance)
        {
#if UNITY_EDITOR
            var entries = GetPersistentTargetEntries();
            List<int> toRemove = new();

            foreach (var (index, target) in entries)
            {
                if (target == callingInstance)
                    toRemove.Add(index);
            }

            if (toRemove.Count > 0)
            {
                Debug.LogWarning($"[TriggerEvent] Removed {toRemove.Count} self-referencing listener(s).", callingInstance);

                // Remove listeners from last to first to avoid index shifting
                toRemove.Sort((a, b) => b.CompareTo(a));
                foreach (var idx in toRemove)
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(this, idx);
                }

                UnityEditor.EditorUtility.SetDirty(callingInstance);
            }
#endif
        }

        /// <summary>
        /// Retrieves all unique persistent target objects.
        /// </summary>
        private HashSet<Object> GetPersistentTargetObjects()
        {
            HashSet<Object> targets = new();
            int count = GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                Object target = GetPersistentTarget(i);
                if (target != null)
                {
                    targets.Add(target);
                }
            }
            return targets;
        }

        /// <summary>
        /// Retrieves index-target pairs for all persistent listeners.
        /// </summary>
        private List<(int index, Object target)> GetPersistentTargetEntries()
        {
            List<(int, Object)> list = new();
            int count = GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                Object target = GetPersistentTarget(i);
                if (target != null)
                {
                    list.Add((i, target));
                }
            }
            return list;
        }

        #endregion
    }
}
