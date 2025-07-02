#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PsychoGarden.Utils;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Utility class for smart drawing of TriggerEvent connections in the scene.
    /// </summary>
    public static class TriggerEventHandles
    {
        private static readonly Dictionary<Transform, List<Transform>> targetToOrigins = new();
        private const float parallelTolerance = 0.1f;

        /// <summary>
        /// Builds a mapping from each target to all visible origin transforms.
        /// </summary>
        public static void RebuildTargetMapping(List<(GameObject owner, TriggerEvent triggerEvent, Color cachedColor)> triggerEventsCache)
        {
            targetToOrigins.Clear();

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

                int count = triggerEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    Object targetObj = triggerEvent.GetPersistentTarget(i);
                    Transform targetTransform = SafeGetTargetTransform(targetObj);
                    if (targetTransform == null)
                        continue;

                    if (!targetToOrigins.TryGetValue(targetTransform, out var list))
                    {
                        list = new List<Transform>();
                        targetToOrigins[targetTransform] = list;
                    }

                    list.Add(owner.transform);
                }
            }
        }

        /// <summary>
        /// Draws all connections from the given owner if visible.
        /// </summary>
        public static void DrawConnectionGizmos(TriggerEvent triggerEvent, GameObject owner)
        {
            if (triggerEvent == null || owner == null)
            {
                return;
            }

            if (!TriggerEventDrawer.EvaluateShouldDrawFor(triggerEvent, owner))
            {
                return;
            }

            int count = triggerEvent.GetPersistentEventCount();
            if (count == 0)
            {
                return;
            }

            Transform origin = owner.transform;

            for (int i = 0; i < count; i++)
            {
                Object targetObj = triggerEvent.GetPersistentTarget(i);
                Transform targetTransform = SafeGetTargetTransform(targetObj);
                if (targetTransform == null)
                {
                    continue;
                }

                Color color = triggerEvent.EditorOverrideGlobalSettings 
                    ? triggerEvent.EditorColor 
                    : TriggerEventSettings.EditorColor;

                DrawSmartConnection(origin, targetTransform, color);
            }
        }

        /// <summary>
        /// Draws a connection between origin and target using smart layout logic.
        /// </summary>
        public static void DrawSmartConnection(
            Transform origin,
            Transform target,
            Color color,
            float angleStep = 30f,
            float baseTangentDistance = 0.05f)
        {
            if (!origin || !target)
            {
                return;
            }

            Vector3 originPos = origin.position;
            Vector3 targetPos = target.position;
            Handles.color = color;

            if (targetToOrigins.TryGetValue(target, out var origins))
            {
                List<Transform> nearbyOrigins = new();

                foreach (Transform other in origins)
                {
                    if (other == null || origin == null)
                    {
                        continue;
                    }

                    if (Vector3.Distance(other.position, origin.position) < parallelTolerance)
                    {
                        nearbyOrigins.Add(other);
                    }
                }

                nearbyOrigins.Sort((a, b) => origins.IndexOf(a).CompareTo(origins.IndexOf(b)));
                int nearbyIndex = nearbyOrigins.IndexOf(origin);
                bool isSingle = origins.Count == 1 || nearbyOrigins.Count == 1;

                if (isSingle || nearbyIndex == 0)
                {
                    Handles.DrawLine(originPos, targetPos, TriggerEventSettings.GizmoThickness);
                }
                else
                {
                    Vector3 direction = (targetPos - originPos).normalized;
                    Vector3 up = Vector3.up;
                    Vector3 right = Vector3.Cross(up, direction);
                    if (right == Vector3.zero)
                        right = Vector3.Cross(Vector3.forward, direction);
                    right.Normalize();
                    up = Vector3.Cross(direction, right).normalized;

                    float distance = Vector3.Distance(originPos, targetPos) * baseTangentDistance;
                    float startingAngle = 90f;
                    int stepsPerCircle = Mathf.RoundToInt(360f / angleStep);
                    int fullTurns = nearbyIndex / stepsPerCircle;
                    int stepIndex = nearbyIndex % stepsPerCircle;
                    float angle = startingAngle + stepIndex * angleStep;

                    Quaternion rotation = Quaternion.AngleAxis(angle, direction);
                    Vector3 rotatedRight = rotation * right;
                    float radius = distance + (fullTurns * distance);
                    Vector3 startTangent = originPos + rotatedRight * radius;
                    Vector3 endTangent = targetPos + rotatedRight * radius;

                    Handles.DrawBezier(
                        originPos, 
                        targetPos, 
                        startTangent, 
                        endTangent, 
                        color, 
                        null, 
                        TriggerEventSettings.GizmoThickness
                    );
                }
            }
            else
            {
                Handles.DrawLine(originPos, targetPos, TriggerEventSettings.GizmoThickness);
            }

            HandlesExtensions.DrawWireSphere(targetPos, 0.05f);
        }

        /// <summary>
        /// Resolves a transform from a general Unity object.
        /// </summary>
        public static Transform SafeGetTargetTransform(Object targetObj)
        {
            if (targetObj == null || targetObj.Equals(null))
                return null;

            return targetObj switch
            {
                Transform t => t,
                Component c => c.transform,
                GameObject g => g.transform,
                _ => null
            };
        }
    }
}
#endif
