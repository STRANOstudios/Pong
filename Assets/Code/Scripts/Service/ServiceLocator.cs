using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Utils;

namespace Code.Systems.Locator
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
        /// Registers a service of type T. Overwrites any existing service of the same type.
        /// </summary>
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
        /// Unregisters a service of type T and removes it from all service collections.
        /// </summary>
        public static void Unregister<T>()
        {
            var type = typeof(T);

            if (m_services.TryGetValue(type, out var service))
            {
                // Clean up from specialized dictionaries
                if (service is IUpdateService)
                {
                    m_updateServices.Remove(type);
                }

                if (service is IFixedUpdateService)
                {
                    m_FixedUpdateServices.Remove(type);
                }

                m_services.Remove(type);

#if UNITY_EDITOR
                Instance?.PopulateSerializedServices();
#endif
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// Retrieves a registered service of type T.
        /// Throws an exception if the service is not found.
        /// </summary>
        /// <remarks>
        /// Prefer using <see cref="TryGet{T}(out T)"/> when failure is expected or optional.
        /// </remarks>
        public static T Get<T>()
        {
            if (m_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new KeyNotFoundException($"Service of type {typeof(T).Name} not found.");
        }

        /// <summary>
        /// Retrieves a registered service of type T.
        /// Returns false if the service is not found.
        /// </summary>
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
        /// Retrieves all registered services.
        /// </summary>
        public static IEnumerable<object> GetAllServices() => m_services.Values;

        #endregion

        #region Setters
        
        /// <summary>
        /// Clears all registered services.
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
