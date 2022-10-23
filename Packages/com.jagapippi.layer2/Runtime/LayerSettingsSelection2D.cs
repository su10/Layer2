using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal static class LayerSettingsSelection2D
    {
        private static ILayerSettings _current;
        public static ILayerSettings current => _current ?? UnityLayerSettings2D.instance;

#if UNITY_EDITOR
        public static event Action<ILayerSettings, ILayerSettings> changed;

        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _current = null;
        }

        internal static void Select(ILayerSettings layerSettings)
        {
            if (_current == layerSettings) return;

            if ((layerSettings != null) && (layerSettings.physicsDimensions != PhysicsDimensions.Two))
            {
                throw new ArgumentException(nameof(layerSettings));
            }

            var old = _current;
            _current = layerSettings;

            changed?.Invoke(old, current);
        }
#endif

        public static void Apply(ILayerSettings layerSettings)
        {
            if (_current == layerSettings)
            {
                current.ApplyCollisionMatrix();
                return;
            }

            var old = _current;
            _current = layerSettings;
            current.ApplyCollisionMatrix();

            changed?.Invoke(old, current);
        }

        private static void ApplyCollisionMatrix(this ILayerSettings settings)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                for (var j = 0; j < Layer.MaxCount - i; j++)
                {
                    Physics2D.IgnoreLayerCollision(i, j, settings.GetIgnoreCollision(i, j));
                }
            }
        }
    }
}
