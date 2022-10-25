using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal static class LayerSettingsSelection
    {
        private static ILayerSettings _active;
        public static ILayerSettings active => _active ?? EmptyLayerSettings.instance;

#if UNITY_EDITOR
        public static event Action<ILayerSettings, ILayerSettings> changed;

        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _active = null;
        }

        internal static void Select(ILayerSettings layerSettings)
        {
            if (_active == layerSettings) return;

            var old = _active;
            _active = layerSettings;

            changed?.Invoke(old, active);
        }
#endif

        public static void Apply(ILayerSettings layerSettings)
        {
            if (_active == layerSettings)
            {
                active.ApplyCollisionMatrix();
                return;
            }

            var old = _active;
            _active = layerSettings;
            active.ApplyCollisionMatrix();

            changed?.Invoke(old, active);
        }

        private static void ApplyCollisionMatrix(this ILayerSettings settings)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                for (var j = 0; j < Layer.MaxCount - i; j++)
                {
                    Physics.IgnoreLayerCollision(i, j, settings.GetIgnoreCollision(i, j));
                    Physics2D.IgnoreLayerCollision(i, j, settings.GetIgnoreCollision2D(i, j));
                }
            }
        }
    }
}
