using UnityEngine;

namespace PsychoGarden.TriggerEvents
{
    /// <summary>
    /// Example class that moves the object it's attached to.
    /// </summary>
    public class SlideMovement : MonoBehaviour
    {
        public float speed = 1;
        public Vector3 direction = Vector3.right;

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
