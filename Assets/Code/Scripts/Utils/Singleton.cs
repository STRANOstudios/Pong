using UnityEngine;
using Sirenix.OdinInspector;

namespace AndreaFrigerio.Service.Utils
{
    /// <summary>
    /// Generic MonoBehaviour-based Singleton pattern.
    /// Automatically ensures only one instance exists.
    /// Optionally persists across scene loads and logs debug information.
    /// </summary>
    [HideMonoScript]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        private static readonly object m_lock = new object();
        private static bool applicationIsQuitting = false;

        #region Fields ------------------------------------------------------------------

        [BoxGroup("Singleton Settings")]
        [Tooltip("If true, this instance will persist across scene loads.")]
        [SerializeField]
        private bool m_dontDestroyOnLoad = false;

        [BoxGroup("Singleton Settings")]
        [Tooltip("If true, logs singleton lifecycle events (Editor/Dev Build only).")]
        [SerializeField]
        private bool m_enableDebugLog = false;

        #endregion

        #region Public Methods ----------------------------------------------------------

        /// <summary>
        /// Access to the singleton instance.
        /// If no instance exists, it tries to find one in the scene or create a new GameObject.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogWarning($"[Singleton] Accessed {typeof(T)} after application quit.");
#endif
                    return null;
                }

                if (m_instance != null)
                {
                    return m_instance;
                }

                lock (m_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = FindFirstObjectByType<T>();

                        if (m_instance != null && FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            Debug.LogError($"[Singleton] Multiple instances of {typeof(T)} found.");
#endif
                            return m_instance;
                        }

                        if (m_instance == null)
                        {
                            GameObject singletonObj = new GameObject($"{typeof(T)} (Singleton)");
                            m_instance = singletonObj.AddComponent<T>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            Debug.LogWarning($"[Singleton] No instance of {typeof(T)} found. Created new.");
#endif
                        }
                    }

                    return m_instance;
                }
            }
        }

        #endregion

        #region Unity Lifecycle ---------------------------------------------------------

        /// <summary>
        /// Ensures only one instance exists. Optionally makes it persistent across scenes.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_instance == null || m_instance == this)
            {
                m_instance = this as T;

                if (m_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (m_enableDebugLog)
                {
                    Debug.Log($"[Singleton] Initialized: {typeof(T)}");
                }
#endif
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (m_enableDebugLog)
                {
                    Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} destroyed: {typeof(T)}");
                }
#endif
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Marks the singleton as shutting down to avoid access after quit.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        #endregion
    }
}
