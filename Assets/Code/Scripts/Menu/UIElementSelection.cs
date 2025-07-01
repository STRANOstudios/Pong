using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Menu.Scripts
{
    [HideMonoScript]
    public class UIElementSelection : MonoBehaviour
    {
        #region Fields

        [BoxGroup("Settings")]
        [Tooltip("The UI element to select when this GameObject starts.")]
        [SerializeField]
        private Selectable m_UIElement = null;

        #endregion

        private void Start()
        {
            if (m_UIElement != null)
            {
                StartCoroutine(SelectNextFrame(m_UIElement));
            }
        }

        /// <summary>
        /// Selects a UI element on the next frame to ensure UI is ready.
        /// </summary>
        private IEnumerator SelectNextFrame(Selectable selectable)
        {
            yield return null; // wait one frame
            selectable.Select();
        }

        /// <summary>
        /// Selects a given UI element immediately.
        /// </summary>
        public void Select(Selectable selectable)
        {
            if (selectable != null)
            {
                selectable.Select();
            }
        }
    }
}
