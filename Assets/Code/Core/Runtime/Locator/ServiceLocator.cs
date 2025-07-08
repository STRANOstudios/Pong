namespace AndreaFrigerio.Core.Runtime.Locator
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using AndreaFrigerio.Core.Runtime.Utils;

    /// <summary>
    /// Thread-safe, allocation-free Service Locator.<br/>
    /// </summary>
    [HideMonoScript]
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Andrea Frigerio/Framework/Service Locator")]
    public sealed class ServiceLocator : Singleton<ServiceLocator>
    {
        #region Inspector (Editor debug)

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        private readonly List<UnityEngine.Object> m_editorServicesView = new();
#endif

        #endregion

        #region Static storage

        private static readonly ConcurrentDictionary<Type, object> s_services = new();
        private static readonly ConcurrentDictionary<Type, IUpdateService> s_update = new();
        private static readonly ConcurrentDictionary<Type, IFixedUpdateService> s_fixed = new();
        private static readonly ConcurrentDictionary<Type, ILateUpdateService> s_late = new();

        /*  Cache arrays → Doesn't need to be locked. It's updated only by (Register / Unregister / Clear). */
        private static volatile IUpdateService[] s_updateCache = Array.Empty<IUpdateService>();
        private static volatile IFixedUpdateService[] s_fixedCache = Array.Empty<IFixedUpdateService>();
        private static volatile ILateUpdateService[] s_lateCache = Array.Empty<ILateUpdateService>();

        #endregion

        #region Events

        /// <summary>
        /// Raised after a service is successfully registered.
        /// </summary>
        public static event Action<Type, object> ServiceRegistered;

        /// <summary>
        /// Raised after a service is unregistered.
        /// </summary>
        public static event Action<Type, object> ServiceUnregistered;

        #endregion

        #region Unity callbacks

#if UNITY_EDITOR
        private void Start() => this.PopulateEditorView();
#endif

        private void Update()
        {
            foreach (IUpdateService svc in s_updateCache)
            {
                svc.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (IFixedUpdateService svc in s_fixedCache)
            {
                svc.OnFixedUpdate();
            }
        }

        private void LateUpdate()
        {
            foreach (ILateUpdateService svc in s_lateCache)
            {
                svc.OnLateUpdate();
            }
        }

        protected override void OnApplicationQuit() => Clear();
        private void OnDisable() => Clear();

        #endregion

        #region Registration

        /// <summary>
        /// Registers <paramref name="service"/> as the unique instance of
        /// type <typeparamref name="T"/>.  Overwrites any previous instance.
        /// </summary>
        public static void Register<T>(T service)
        {
            Type t = typeof(T);

            s_services[t] = service;

            if (service is IUpdateService u)
            {
                s_update[t] = u;
            }
            else
            {
                s_update.TryRemove(t, out _);
            }

            if (service is IFixedUpdateService f)
            {
                s_fixed[t] = f;
            }
            else
            {
                s_fixed.TryRemove(t, out _);
            }

            if (service is ILateUpdateService l)
            {
                s_late[t] = l;
            }
            else
            {
                s_late.TryRemove(t, out _);
            }

            RefreshCaches();

#if UNITY_EDITOR
            Instance?.PopulateEditorView();
#endif
            ServiceRegistered?.Invoke(t, service);
        }

        /// <summary>
        /// Creates a new <typeparamref name="T"/> with the parameter-lessctor and registers it.
        /// </summary>
        public static void RegisterComponent<T>() where T : Component
        {
            GameObject go = new GameObject(typeof(T).Name);
            Register(go.AddComponent<T>());
        }

        /// <summary>
        /// Creates a new <typeparamref name="T"/> with the provided factory method and registers it.
        /// </summary>
        public static void Register<T>(Func<T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Register(factory());
        }

        /// <summary>
        /// Unregisters the current instance of <typeparamref name="T"/>.
        /// </summary>
        public static void Unregister<T>(T service)
        {
            Type t = typeof(T);
            if (s_services.TryRemove(t, out object obj))
            {
                s_update.TryRemove(t, out _);
                s_fixed.TryRemove(t, out _);
                s_late.TryRemove(t, out _);

                RefreshCaches();

#if UNITY_EDITOR
                Instance?.PopulateEditorView();
#endif
                ServiceUnregistered?.Invoke(t, obj);
            }
        }

        /// <summary>
        /// Unregisters the current instance of <typeparamref name="T"/>.
        /// </summary>
        public static void Unregister<T>() => Unregister(Get<T>());

        #endregion

        #region Queries

        /// <summary>
        /// Returns the unique instance of type <typeparamref name="T"/>.
        /// </summary>
        public static T Get<T>() =>
            s_services.TryGetValue(typeof(T), out object o)
                ? (T)o
                : throw new KeyNotFoundException($"Service {typeof(T).Name} not found.");

        /// <summary>
        /// Returns the unique instance of type <typeparamref name="T"/> if it exists.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <param name="service">The service</param>
        /// <returns>True if the service was found</returns>
        public static bool TryGet<T>(out T service)
        {
            if (s_services.TryGetValue(typeof(T), out object o))
            {
                service = (T)o;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>
        /// Returns all registered services.
        /// </summary>
        public static IEnumerable<object> GetAllServices() => s_services.Values;

        #endregion

        #region Maintenance

        /// <summary>
        /// Removes every service and clears caches.
        /// </summary>
        public static void Clear()
        {
            if (s_services.IsEmpty)
            {
                return;
            }

            foreach (Type t in s_services.Keys)
            {
                ServiceUnregistered?.Invoke(t, s_services[t]);
            }

            s_services.Clear();
            s_update.Clear();
            s_fixed.Clear();
            s_late.Clear();

            RefreshCaches();

#if UNITY_EDITOR
            Instance?.PopulateEditorView();
#endif
        }

        private static void RefreshCaches()
        {
            s_updateCache = s_update.Values.ToArray();
            s_fixedCache = s_fixed.Values.ToArray();
            s_lateCache = s_late.Values.ToArray();
        }

#if UNITY_EDITOR
        private void PopulateEditorView()
        {
            this.m_editorServicesView.Clear();
            foreach (object s in s_services.Values)
            {
                if (s is UnityEngine.Object obj)
                {
                    this.m_editorServicesView.Add(obj);
                }
            }
        }
#endif

        #endregion
    }
}
