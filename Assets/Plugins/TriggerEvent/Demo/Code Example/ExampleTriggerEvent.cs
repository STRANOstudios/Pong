using UnityEngine;
using PsychoGarden.TriggerEvents;

namespace PsychoGarden.Demo
{
    /// <summary>
    /// Example MonoBehaviour that triggers a TriggerEvent when started.
    /// </summary>
    public class ExampleTriggerEvent : MonoBehaviour
    {
        /// <summary>
        /// The TriggerEvent invoked when this object is triggered.
        /// </summary>
        public TriggerEvent onTriggered;

        /// <summary>
        /// Automatically invokes the event on Start.
        /// </summary>
        private void Start()
        {
            onTriggered?.Invoke(this.transform);
        }

        /// <summary>
        /// Example method that can be called to simulate interaction.
        /// </summary>
        public void Interact()
        {
            Debug.Log("Interacted with " + gameObject.name);
        }
    }
}
