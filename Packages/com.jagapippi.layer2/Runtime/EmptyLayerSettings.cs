using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal class EmptyLayerSettings : ILayerSettings
    {
        public static readonly EmptyLayerSettings instance = new();

#if UNITY_EDITOR
        static EmptyLayerSettings()
        {
            var layers = instance.GetNames();

            EditorApplication.update += () =>
            {
                var changed = false;

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var layerName = LayerMask.LayerToName(i);
                    if (layerName == layers[i]) continue;

                    layers[i] = layerName;
                    changed = true;
                }

                if (changed) instance.changedSerializedObject?.Invoke(instance);
            };
        }
#endif

        private EmptyLayerSettings()
        {
        }

        #region ILayerSettings

        public string name => "Project Settings";
        public string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public int NameToLayer(string layerName) => LayerMask.NameToLayer(layerName);
        public bool GetCollision(int layer1, int layer2) => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false);
        public bool GetCollision2D(int layer1, int layer2) => (Physics2D.GetIgnoreLayerCollision(layer1, layer2) == false);
#if UNITY_EDITOR
        public event Action<ILayerSettings> changedSerializedObject;
#endif

        #endregion
    }
}
