using UnityEngine;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Menu.Scripts
{
    [HideMonoScript]
    public class MouseVisibilityController : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("If the cursor should be visible or not")]
        [SerializeField] 
        private bool showCursor = true;

        [BoxGroup("Settings")]
        [Tooltip("If the cursor should be locked or not")]
        [SerializeField, ShowIf("showCursor")]
        private CursorLockMode lockCursor = CursorLockMode.None;

        #endregion

        private void Start()
        {
            Cursor.visible = showCursor;
            Cursor.lockState = lockCursor;
        }

        /// <summary>
        /// Shows or hides the cursor at runtime.
        /// </summary>
        /// <param Name="value">True to show, false to hide.</param>
        public void ShowCursor(bool value)
        {
            Cursor.visible = value;
        }

        /// <summary>
        /// Sets the cursor lock mode.
        /// </summary>
        /// <param Name="mode">Corresponds to UnityEngine.CursorLockMode enum.</param>
        public void SetLockMode(int mode)
        {
            Cursor.lockState = (CursorLockMode)Mathf.Clamp(mode, 0, 2);
        }
    }
}
