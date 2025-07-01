using UnityEngine;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Menu.Scripts
{
    [HideMonoScript]
    public class FrameRateLimiter : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("Set the desired frame rate (ignored if VSync is active)"), LabelText("Target Frame Rate")]
        [OnValueChanged("ApplySettings")]
        [SerializeField, Range(15, 240)]
        private int targetFrameRate = 60;

        [BoxGroup("Settings")]
        [Tooltip("Set the desired VSync mode"), LabelText("VSync Mode")]
        [OnValueChanged("ApplySettings")]
        [SerializeField]
        private VSyncMode vSyncSetting = VSyncMode.Off;

        #endregion

        private void Start()
        {
            ApplySettings(); // auto-apply on play
        }

        private void ApplySettings()
        {
            QualitySettings.vSyncCount = (int)vSyncSetting;
            Application.targetFrameRate = targetFrameRate;

            Debug.Log($"[FrameRateSettings] VSync: {vSyncSetting}, TargetFrameRate: {targetFrameRate}");
        }

        public enum VSyncMode
        {
            Off = 0,
            EveryVBlank = 1,
            EverySecondVBlank = 2
        }
    }
}
