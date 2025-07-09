namespace AndreaFrigerio.Core.Runtime.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using Sirenix.OdinInspector;

    [Serializable]
    public class ScriptEventGroup
    {
        [HideInInspector] public string ScriptName;
        [HideInInspector] public MonoScript Script;

        [TabGroup("Bindings", Icon = SdfIconType.GearWideConnected, TextColor = "orange")]
        [ShowInInspector, HideReferenceObjectPicker, ListDrawerSettings(DraggableItems = false)]
        public List<GenericEventBinding> Bindings = new();

        [TabGroup("Instance", Icon = SdfIconType.PersonCircle, TextColor = "green")]
        [ShowIf("HasInstanceEvents")]
        [LabelText("Instance Target (if required)")]
        [Required("Required for non-static events")]
        public UnityEngine.Object InstanceTarget;

#if UNITY_EDITOR
        private bool HasInstanceEvents => Bindings.Any(b => !b.IsStatic);
#endif
    }

}
