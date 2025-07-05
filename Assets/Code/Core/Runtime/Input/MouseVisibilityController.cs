namespace AndreaFrigerio.Core.Runtime.Input
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Toggles cursor visibility and lock state both at startup and
    /// via public methods.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Input/Mouse Visibility Controller")]
    public sealed class MouseVisibilityController : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("Show cursor at start.")]
        [SerializeField]
        private bool m_showCursor = true;

        [BoxGroup("Settings")]
        [Tooltip("Cursor lock state at start.")]
        [SerializeField, ShowIf(nameof(m_showCursor))]
        private CursorLockMode m_lockCursor = CursorLockMode.None;

        #endregion

        #region Unity Callbacks

        private void Start() => this.ApplySettings();

        #endregion

        #region Public API

        /// <summary>Shows or hides the cursor at runtime.</summary>
        /// <param name="value">True to show; false to hide.</param>
        public void ShowCursor(bool value) => Cursor.visible = value;

        /// <summary>Sets the cursor lock mode at runtime.</summary>
        /// <param name="mode">
        /// Integer cast of <see cref="UnityEngine.CursorLockMode"/>.
        /// </param>
        public void SetLockMode(int mode) =>
            Cursor.lockState = (CursorLockMode)Mathf.Clamp(mode, 0, 2);

        #endregion

        #region Private Methods

        private void ApplySettings()
        {
            Cursor.visible = this.m_showCursor;
            Cursor.lockState = this.m_lockCursor;
        }

        #endregion
    }
}
