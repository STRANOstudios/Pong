namespace AndreaFrigerio.Core.Runtime.Lifecycle
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Provides a single call to exit play-mode in the Editor or close the
    /// application in a built player.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Application/Quit Game")]
    public sealed class QuitGame : MonoBehaviour
    {
        /// <summary>
        /// Stops play-mode when running inside the Unity Editor,
        /// otherwise terminates the standalone build.
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
