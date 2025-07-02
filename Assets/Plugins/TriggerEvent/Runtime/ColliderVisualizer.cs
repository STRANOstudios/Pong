using UnityEngine;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Base class that draws gizmos for Collider components in the editor for easier visualization.
    /// </summary>
    [DisallowMultipleComponent]
    public class ColliderVisualizer : MonoBehaviour
    {
#if UNITY_EDITOR

        #region Fields ----------------------------------------------------------------------------

        [Tooltip("Choose when to show collider gizmos in the editor.")]
        [SerializeField]
        private ColliderGizmoDisplayMode m_displayMode = ColliderGizmoDisplayMode.OnlyWhenSelected;

        [Tooltip("Show colliders in the editor.")]
        [SerializeField]
        private bool m_showColliders = true;

        [Tooltip("Only show enabled colliders.")]
        [SerializeField]
        private bool m_showCollidersOnlyEnabled = true;

        [Tooltip("The color used to draw collider gizmos.")]
        [SerializeField]
        private Color m_gizmoColor = Color.green;

        #endregion

        #region Unity Methods ---------------------------------------------------------------------

        /// <summary>
        /// Called by Unity to draw gizmos in the editor.
        /// Draws gizmos when the object is visible in Scene view.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (m_displayMode == ColliderGizmoDisplayMode.Always && m_showColliders)
            {
                DrawCollidersGizmo();
            }
        }

        /// <summary>
        /// Called by Unity when the object is selected in the editor.
        /// Draws gizmos only when selected.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (m_displayMode == ColliderGizmoDisplayMode.OnlyWhenSelected && m_showColliders)
            {
                DrawCollidersGizmo();
            }
        }

        #endregion

        #region Private Methods -------------------------------------------------------------------

        /// <summary>
        /// Draws the gizmos for all Collider components attached to this GameObject.
        /// </summary>
        private void DrawCollidersGizmo()
        {
            if (!enabled)
                return;

            Collider[] colliders = GetComponents<Collider>();

            if (colliders.Length <= 0)
                return;

            Gizmos.color = m_gizmoColor;

            foreach (Collider collider in colliders)
            {
                if (!collider.enabled && m_showCollidersOnlyEnabled)
                    continue;

                Gizmos.matrix = transform.localToWorldMatrix;

                // Draw the appropriate gizmo based on collider type
                if (collider is BoxCollider boxCollider)
                {
                    Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                    Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    DrawCapsuleGizmos(capsuleCollider);
                }
                else if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
                {
                    Gizmos.DrawMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
                }

                Gizmos.matrix = Matrix4x4.identity; // Always reset Gizmos matrix after drawing
            }
        }

        /// <summary>
        /// Helper method to draw a CapsuleCollider gizmo manually.
        /// </summary>
        /// <param name="capsule">The capsule collider to draw.</param>
        private void DrawCapsuleGizmos(CapsuleCollider capsule)
        {
            Vector3 center = capsule.center;
            float radius = capsule.radius;
            float height = Mathf.Max(0, capsule.height / 2f - radius);

            // Draw spheres for the capsule caps
            Gizmos.DrawWireSphere(center + Vector3.up * height, radius);
            Gizmos.DrawWireSphere(center - Vector3.up * height, radius);

            // Draw lines connecting the caps
            Gizmos.DrawLine(center + Vector3.up * height + Vector3.right * radius, center - Vector3.up * height + Vector3.right * radius);
            Gizmos.DrawLine(center + Vector3.up * height - Vector3.right * radius, center - Vector3.up * height - Vector3.right * radius);
            Gizmos.DrawLine(center + Vector3.up * height + Vector3.forward * radius, center - Vector3.up * height + Vector3.forward * radius);
            Gizmos.DrawLine(center + Vector3.up * height - Vector3.forward * radius, center - Vector3.up * height - Vector3.forward * radius);
        }

        #endregion

        #region Enums -----------------------------------------------------------------------------

        /// <summary>
        /// Controls when the collider gizmos should be displayed.
        /// </summary>
        public enum ColliderGizmoDisplayMode
        {
            Always,
            OnlyWhenSelected
        }

        #endregion

#endif
    }
}
