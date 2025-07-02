using UnityEngine;

namespace PsychoGarden.Demo
{
    /// <summary>
    /// Simple target class used for demonstration purposes with TriggerEvent.
    /// </summary>
    public class ExampleTarget : MonoBehaviour
    {
        /// <summary>
        /// Example method that can be called by a TriggerEvent to simulate an interaction.
        /// </summary>
        public void Interact()
        {
            Debug.Log("Interacted with " + gameObject.name);
        }
    }
}
