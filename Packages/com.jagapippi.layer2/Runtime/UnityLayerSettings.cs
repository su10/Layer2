using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal sealed class UnityLayerSettings : ILayerSettings
    {
        public static readonly UnityLayerSettings instance = new();
        private const string Name = "Default Layer Settings";

#if UNITY_EDITOR
        private static readonly string[] _layers = instance.GetNames();

        static UnityLayerSettings()
        {
            EditorApplication.update += () =>
            {
                var changed = false;

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var layerName = LayerMask.LayerToName(i);
                    if (layerName == _layers[i]) continue;

                    _layers[i] = layerName;
                    changed = true;
                }

                if (changed) instance.changedSerializedObject?.Invoke(instance);
            };
        }
#endif

        private UnityLayerSettings()
        {
        }

        #region ILayerSettings

        public string name => Name;
        public string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public int NameToLayer(string name) => LayerMask.NameToLayer(name);
        public bool GetCollision(int layer1, int layer2) => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false);
#if UNITY_EDITOR
        public event Action<ILayerSettings> changedSerializedObject;
#endif

        #endregion
    }
}
