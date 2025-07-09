namespace AndreaFrigerio.Core.Runtime.Events
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;

    [Serializable]
    public class GenericEventBinding
    {
        [HideInInspector] public string EventName;
        [HideInInspector] public string TargetEventTypeName;
        [HideInInspector] public bool IsStatic;

        [LabelText("@EventName")]
        public UnityEvent Response = new();
    }
}
