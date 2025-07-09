namespace AndreaFrigerio.Core.Runtime.Events
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/Events/Event Binder Component")]
    public class EventBinderComponent : MonoBehaviour
    {
        [SerializeField]
        private EventBinderAsset m_binderAsset;

        private void OnEnable() => this.m_binderAsset?.BindAll();
        private void OnDisable() => this.m_binderAsset?.UnbindAll();
    }
}
