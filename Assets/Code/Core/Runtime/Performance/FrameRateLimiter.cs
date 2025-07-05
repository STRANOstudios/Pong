namespace AndreaFrigerio.Core.Runtime.Performance
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Input;

    /// <summary>
    /// Applies VSync mode and target frame-rate limits at runtime.
    /// Inspector fields auto-apply via Odin OnValueChanged.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Performance/Frame Rate Limiter")]
    public sealed class FrameRateLimiter : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("Desired frame rate (ignored when VSync > 0).")]
        [LabelText("Target Frame Rate")]
        [OnValueChanged(nameof(ApplySettings))]
        [SerializeField, Range(15, 240)]
        private int m_targetFrameRate = 60;

        [BoxGroup("Settings")]
        [Tooltip("VSync mode.")]
        [LabelText("VSync Mode")]
        [OnValueChanged(nameof(ApplySettings))]
        [SerializeField]
        private VSyncMode m_vSyncSetting = VSyncMode.Off;

        #endregion

        #region Unity Callbacks

        private void Start() => this.ApplySettings();

        #endregion

        #region Private Methods

        /// <summary>Applies the current inspector settings.</summary>
        private void ApplySettings()
        {
            QualitySettings.vSyncCount = (int)this.m_vSyncSetting;
            Application.targetFrameRate = this.m_targetFrameRate;

            Debug.Log($"[FrameRate] VSync: {this.m_vSyncSetting}, " +
                      $"Target: {this.m_targetFrameRate}");
        }

        #endregion

        #region Nested Types

        public enum VSyncMode
        {
            Off = 0,
            EveryVBlank = 1,
            EverySecondVBlank = 2
        }

        #endregion
    }
}
