using System;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2
{
    internal static class LayerSettingsSelection
    {
        private static ILayerSettings _current;
        public static ILayerSettings current => _current ?? UnityLayerSettings.instance;

        public static event Action<ILayerSettings, ILayerSettings> changed;

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _current = null;
        }
#endif

        public static void SetCurrent(ILayerSettings layerSettings)
        {
            if (_current == layerSettings) return;

            var old = _current;
            _current = layerSettings;
            current.ApplyCollisionMatrix();
            changed?.Invoke(old, layerSettings);
        }

        private static void ApplyCollisionMatrix(this ILayerSettings settings)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                for (var j = 0; j < Layer.MaxCount - i; j++)
                {
                    Physics.IgnoreLayerCollision(i, j, settings.GetIgnoreCollision(i, j));
                }
            }
        }
    }
}
