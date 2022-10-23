using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal abstract class UnityLayerSettingsBase : ILayerSettings
    {
        protected internal UnityLayerSettingsBase()
        {
        }

        #region ILayerSettings

        public abstract PhysicsDimensions physicsDimensions { get; }
        public abstract string name { get; }
        public string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public int NameToLayer(string name) => LayerMask.NameToLayer(name);
        public abstract bool GetCollision(int layer1, int layer2);
#if UNITY_EDITOR
        public event Action<ILayerSettings> changedSerializedObject
        {
            add => _changedSerializedObject += value;
            remove => _changedSerializedObject -= value;
        }

        protected Action<ILayerSettings> _changedSerializedObject;
#endif

        #endregion
    }

    internal abstract class UnityLayerSettingsBase<T> : UnityLayerSettingsBase where T : UnityLayerSettingsBase<T>, new()
    {
        public static readonly T instance = new();

#if UNITY_EDITOR
        protected static void OnInitializeOnLoad()
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

                if (changed) instance._changedSerializedObject?.Invoke(instance);
            };
        }
#endif
    }
}
