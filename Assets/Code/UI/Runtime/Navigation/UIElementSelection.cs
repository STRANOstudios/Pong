namespace AndreaFrigerio.UI.Runtime.Navigation
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Selects a specified <see cref="Selectable"/> when the scene starts and
    /// exposes public helpers to change selection at runtime.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/UI/UI Element Selection")]
    public sealed class UIElementSelection : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("UI element automatically selected on Start.")]
        [SerializeField]
        private Selectable m_uiElement = null;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            if (this.m_uiElement != null)
            {
                this.StartCoroutine(this.SelectNextFrame(this.m_uiElement));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Waits one frame before selecting a UI element so the EventSystem
        /// has finished its initialisation.
        /// </summary>
        private IEnumerator SelectNextFrame(Selectable selectable)
        {
            yield return null;
            selectable.Select();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Immediately selects the given UI element.
        /// </summary>
        /// <param name="selectable">Target element to focus.</param>
        public void Select(Selectable selectable)
        {
            if (selectable != null)
            {
                selectable.Select();
            }
        }

        #endregion
    }
}
