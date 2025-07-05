namespace AndreaFrigerio.Core.Runtime.Utils
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Generic MonoBehaviour-based singleton.
    /// Attempts to find an existing instance, otherwise creates one.
    /// Optionally survives scene changes and can log lifecycle events.
    /// </summary>
    [HideMonoScript]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        private static readonly object m_lock = new object();
        private static bool m_applicationIsQuitting = false;

        #region Inspector fields

        [BoxGroup("Singleton Settings")]
        [Tooltip("If true, the instance persists across scene loads.")]
        [SerializeField]
        private bool m_dontDestroyOnLoad = false;

        [BoxGroup("Singleton Settings")]
        [Tooltip("If true, writes debug logs in Editor or Development builds.")]
        [SerializeField]
        private bool m_enableDebugLog = false;

        #endregion

        #region Public API

        /// <summary>
        /// Gets the singleton instance. Creates one if none exists.
        /// Returns <c>null</c> after the application has begun quitting.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_applicationIsQuitting)
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

                        if (m_instance != null &&
                            FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            Debug.LogError($"[Singleton] Multiple instances of {typeof(T)} found.");
#endif
                            return m_instance;
                        }

                        if (m_instance == null)
                        {
                            var go = new GameObject($"{typeof(T)} (Singleton)");
                            m_instance = go.AddComponent<T>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            Debug.LogWarning($"[Singleton] No instance found; created new.");
#endif
                        }
                    }

                    return m_instance;
                }
            }
        }

        #endregion

        #region Unity callbacks

        /// <summary>
        /// Ensures uniqueness and handles <c>DontDestroyOnLoad</c>.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_instance == null || m_instance == this)
            {
                m_instance = this as T;

                if (this.m_dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(this.gameObject);
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (this.m_enableDebugLog)
                {
                    Debug.Log($"[Singleton] Initialized: {typeof(T)}");
                }
#endif
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (this.m_enableDebugLog)
                {
                    Debug.LogWarning($"[Singleton] Duplicate destroyed: {typeof(T)}");
                }
#endif
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Marks the singleton as shutting down.
        /// </summary>
        protected virtual void OnApplicationQuit() =>
            m_applicationIsQuitting = true;

        #endregion

    }
}
