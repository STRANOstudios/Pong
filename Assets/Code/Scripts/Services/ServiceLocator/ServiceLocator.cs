using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using AndreaFrigerio.Service.Utils;

namespace AndreaFrigerio.Service.Locator
{
    [DefaultExecutionOrder(-100)]
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        #region Fields

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        private List<UnityEngine.Object> m_editorServicesView = new();
#endif

        #endregion

        #region Static Dictionaries

        private static readonly Dictionary<Type, object> m_services = new();

        // Services with specific interfaces
        private static readonly Dictionary<Type, IUpdateService> m_updateServices = new();
        private static readonly Dictionary<Type, IFixedUpdateService> m_FixedUpdateServices = new();

        #endregion

        #region Unity Callbacks

        private void Start()
        {
#if UNITY_EDITOR
            PopulateSerializedServices();
#endif
        }

        private void Update()
        {
            foreach (IUpdateService service in m_updateServices.Values)
            {
                service.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (IFixedUpdateService service in m_FixedUpdateServices.Values)
            {
                service.OnFixedUpdate();
            }
        }

        #endregion

        #region Registration & Unregistration

        /// <summary>
        /// Registers a service instance of type <typeparamref name="T"/>.
        /// If a service of the same type is already registered, it will be overwritten.
        /// Also adds the service to specialized update dictionaries if applicable.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        public static void Register<T>(T service)
        {
            var type = typeof(T);

            if (m_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type.Name} is already registered. Overwriting.");
            }

            m_services[type] = service;

            // Interface-based categorization
            if (service is IUpdateService updateService)
            {
                m_updateServices[type] = updateService;
            }

            if (service is IFixedUpdateService fixedUpdateService)
            {
                m_FixedUpdateServices[type] = fixedUpdateService;
            }

#if UNITY_EDITOR
            Instance?.PopulateSerializedServices();
#endif
        }

        /// <summary>
        /// Instantiates and registers a new service of type <typeparamref name="T"/> using its parameterless constructor.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        public static void Register<T>() where T : new() => Register(new T());

        /// <summary>
        /// Unregisters the given service instance of type <typeparamref name="T"/>.
        /// Removes it from all relevant service collections.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        /// <param name="service">The service instance to unregister.</param>
        public static void Unregister<T>(T service)
        {
            var type = typeof(T);

            if (m_services.TryGetValue(type, out var registered) && ReferenceEquals(registered, service))
            {
                if (registered is IUpdateService)
                {
                    m_updateServices.Remove(type);
                }

                if (registered is IFixedUpdateService)
                {
                    m_FixedUpdateServices.Remove(type);
                }

                m_services.Remove(type);

#if UNITY_EDITOR
                Instance?.PopulateSerializedServices();
#endif
            }
            else
            {
                Debug.LogWarning($"Attempted to unregister service of type {type.Name}, but it was not registered or did not match the provided instance.");
            }
        }

        /// <summary>
        /// Unregisters the currently registered service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        public static void Unregister<T>() => Unregister<T>(Get<T>());

        #endregion

        #region Getters

        /// <summary>
        /// Retrieves the registered service of type <typeparamref name="T"/>.
        /// Throws an exception if the service is not found.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The registered service instance of type <typeparamref name="T"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the service is not registered.</exception>
        public static T Get<T>()
        {
            if (m_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new KeyNotFoundException($"Service of type {typeof(T).Name} not found.");
        }

        /// <summary>
        /// Tries to retrieve the registered service of type <typeparamref name="T"/>.
        /// Returns true if found, otherwise false.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <param name="service">When this method returns, contains the service if found; otherwise, the default value for the type.</param>
        /// <returns><c>true</c> if the service is registered; otherwise, <c>false</c>.</returns>
        public static bool TryGet<T>(out T service)
        {
            if (m_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>
        /// Returns all currently registered services as an enumerable collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Object}"/> of all registered services.</returns>
        public static IEnumerable<object> GetAllServices() => m_services.Values;

        #endregion

        #region Setters

        /// <summary>
        /// Clears all registered services from the locator, including update-related services.
        /// </summary>
        public static void Clear()
        {
            m_services.Clear();
            m_updateServices.Clear();
            m_FixedUpdateServices.Clear();
#if UNITY_EDITOR
            Instance?.PopulateSerializedServices();
#endif
        }

        #endregion

#if UNITY_EDITOR
        #region Editor-Only

        private void PopulateSerializedServices()
        {
            m_editorServicesView.Clear();

            foreach (var entry in m_services)
            {
                if (entry.Value is UnityEngine.Object unityObj)
                {
                    m_editorServicesView.Add(unityObj);
                }
            }
        }

        #endregion
#endif
    }
}
