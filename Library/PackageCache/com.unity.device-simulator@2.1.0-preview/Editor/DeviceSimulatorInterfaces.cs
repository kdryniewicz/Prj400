using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.DeviceSimulator
{
    /// <summary>
    /// Interface which provides the functionality to extend the device simulator UI.
    /// </summary>
    public interface IDeviceSimulatorExtension
    {
        /// <summary>
        /// Title which is shown as the title of the extended UI.
        /// </summary>
        string extensionTitle { get; }

        /// <summary>
        /// Callback which is implemented by the users to extend the UI. It is called by Device Simulator.
        /// </summary>
        /// <param name="visualElement"></param>
        void OnExtendDeviceSimulator(VisualElement visualElement);
    }

    internal class SimulatorExtensions
    {
        private List<IDeviceSimulatorExtension> m_Extensions = new List<IDeviceSimulatorExtension>();
        public List<IDeviceSimulatorExtension> Extensions => m_Extensions;

        public  SimulatorExtensions()
        {
            foreach (var type in TypeCache.GetTypesDerivedFrom<IDeviceSimulatorExtension>())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                AddToList(type, ref m_Extensions);
            }

            m_Extensions.Sort(CompareExtensionOrder);
        }

        static void AddToList<T>(Type type, ref List<T> list) where T : class
        {
            T obj = Activator.CreateInstance(type) as T;
            list.Add(obj);
        }

        internal static int CompareExtensionOrder(IDeviceSimulatorExtension ext1, IDeviceSimulatorExtension ext2)
        {
            return string.Compare(ext1.extensionTitle, ext2.extensionTitle);
        }
    }
}
