using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Extension methods for Handles.
    /// </summary>
    public static class HandlesExtensions
    {
        /// <summary>
        /// Draws a wireframe sphere in the editor scene view using Handles.
        /// </summary>
        public static void DrawWireSphere(Vector3 center, float radius)
        {
            // Draw circles in all 3 planes
            Handles.DrawWireDisc(center, Vector3.up, radius);    // XZ plane
            Handles.DrawWireDisc(center, Vector3.right, radius); // YZ plane
            Handles.DrawWireDisc(center, Vector3.forward, radius); // XY plane

        }
    }
}
#endif
