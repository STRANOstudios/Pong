#if UNITY_EDITOR
namespace AndreaFrigerio.Core.Runtime.SceneManagement
{
    using System.IO;
    using UnityEngine.SceneManagement;
    using Sirenix.OdinInspector;

    /// <content>
    /// Odin Inspector helpers for <see cref="LoadScene"/>.
    /// </content>
    public sealed partial class LoadScene
    {
        /// <summary>Inspector button that calls <see cref="Load"/>.</summary>
        /// <param name="sceneName">Scene name chosen from the dropdown.</param>
        [Button("Load Scene")]
        private void LoadSceneOdin(
            [ValueDropdown(nameof(GetSceneNames))] string sceneName)
        {
            this.Load(sceneName);
        }

        /// <summary>
        /// Retrieves all scene names included in Build Settings.
        /// </summary>
        private static string[] GetSceneNames()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] results = new string[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                results[i] = Path.GetFileNameWithoutExtension(
                    SceneUtility.GetScenePathByBuildIndex(i));
            }

            return results;
        }
    }
}
#endif
