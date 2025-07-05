namespace AndreaFrigerio.UI.Runtime.Controls
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// Slider that snaps its value to evenly-spaced segments.
    /// </summary>
    [HideMonoScript]
    [AddComponentMenu("Andrea Frigerio/UI/AF Slider")]
    public sealed class AFSlider : Slider
    {
        [Header("Segments")]
        [Min(1)]
        [SerializeField]
        private int m_segments = 1;

        #region Unity UI overrides – all routed through SnapToSegment

        public override float value
        {
            get => base.value;
            set => base.value = this.SnapToSegment(value);
        }

        public override void SetValueWithoutNotify(float value) =>
            base.value = this.SnapToSegment(value);

        public override void OnPointerUp(PointerEventData e)
        {
            base.OnPointerUp(e);
            base.value = this.SnapToSegment(value);
        }

        public override void OnMove(AxisEventData e)
        {
            base.OnMove(e);
            base.value = this.SnapToSegment(value);
        }

        public override void OnInitializePotentialDrag(PointerEventData e)
        {
            base.OnInitializePotentialDrag(e);
            base.value = this.SnapToSegment(value);
        }

        public override void OnDrag(PointerEventData e)
        {
            base.OnDrag(e);
            base.value = this.SnapToSegment(value);
        }

        #endregion

        #region Helpers

        private float SnapToSegment(float raw) =>
            Mathf.RoundToInt(raw * this.m_segments) / (float)this.m_segments;

        #endregion

    }
}
