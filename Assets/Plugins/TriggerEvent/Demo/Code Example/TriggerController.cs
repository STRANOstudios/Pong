using UnityEngine;
using PsychoGarden.Utils;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Controller that automatically invokes TriggerEvents on trigger enter and exit.
    /// Inherits ColliderVisualizer to show collider gizmos in the editor.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerController : ColliderVisualizer
    {
        #region Fields ----------------------------------------------------------------------------

        [Header("Custom Events")]
        [Line]

        /// <summary>
        /// Event invoked when another collider enters the trigger.
        /// </summary>
        public TriggerEvent m_onTriggerEnter;

        [Line]

        /// <summary>
        /// Event invoked when another collider exits the trigger.
        /// </summary>
        public TriggerEvent m_onTriggerExit;

        #endregion

        #region Unity Methods ---------------------------------------------------------------------

        /// <summary>
        /// Ensures that the attached Collider is set as a trigger on Awake.
        /// </summary>
        private void Awake()
        {
            if (TryGetComponent(out Collider collider))
            {
                collider.isTrigger = true;
            }
        }

        /// <summary>
        /// Called automatically by Unity when another collider enters the trigger zone.
        /// Invokes the associated TriggerEvent.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            m_onTriggerEnter?.Invoke(this.transform);
        }

        /// <summary>
        /// Called automatically by Unity when another collider exits the trigger zone.
        /// Invokes the associated TriggerEvent.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            m_onTriggerExit?.Invoke(this.transform);
        }

        #endregion
    }
}
