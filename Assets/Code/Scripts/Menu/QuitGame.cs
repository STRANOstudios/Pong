using UnityEngine;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Menu.Scripts
{
    [HideMonoScript]
    public class QuitGame : MonoBehaviour
    {
        /// <summary>
        /// Quits the game
        ///  - In Editor: Quits the Editor
        ///  - In Build: Quits the Application
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
