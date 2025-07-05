namespace AndreaFrigerio.Core.Runtime.SceneManagement
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Sirenix.OdinInspector;

    /// <summary>
    /// MonoBehaviour utility that loads a scene by exact name if it
    /// exists in the Build Settings; logs an error otherwise.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [AddComponentMenu("Andrea Frigerio/Scene Management/Load Scene")]
    public sealed partial class LoadScene : MonoBehaviour
    {
        /// <summary>
        /// Loads a scene that is present in Build Settings. Logs an
        /// error if the scene name is invalid.
        /// </summary>
        /// <param name="sceneName">
        ///   Exact, case-sensitive name of the scene (e.g. <c>MainMenu</c>).
        /// </param>
        public void Load(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError(
                    $"Scene '{sceneName}' not found in Build Settings. " +
                    "Add it via File ▶ Build Settings.");
            }
        }

    }
}
