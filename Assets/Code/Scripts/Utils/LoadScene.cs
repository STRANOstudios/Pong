using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Service.SceneManagement
{
    [HideMonoScript]
    public class LoadScene : MonoBehaviour
    {
        /// <summary>
        /// Loads a scene by name if it exists in the build settings
        /// </summary>
        /// <param name="sceneName">Name of the scene to load (case-sensitive)</param>
        /// <example>
        /// <code>
        /// Load("MainMenu");
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Throws error in console if scene is not in build settings
        /// </exception>
        public void Load(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' not found in build settings! " +
                    "Please add it via File > Build Settings");
            }
        }

        #region Odin

        [Button("Load Scene")]
        private void LoadSceneOdin([ValueDropdown("GetSceneNames")] string sceneName)
        {
            Load(sceneName);
        }

        private string[] GetSceneNames()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] scenes = new string[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(
                    SceneUtility.GetScenePathByBuildIndex(i));
            }

            return scenes;
        }

        #endregion

    }
}