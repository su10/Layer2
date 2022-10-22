using System;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2
{
    internal static class Layer2Core
    {
        private static ILayerSettings _currentSettings;
        public static ILayerSettings currentSettings => _currentSettings ?? UnityLayerSettings.instance;

        public static event Action<ILayerSettings, ILayerSettings> settingsChanged;

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _currentSettings = null;
        }
#endif

        public static void SetCurrentSettings(ILayerSettings layerSettings)
        {
            if (_currentSettings == layerSettings) return;

            var old = _currentSettings;
            _currentSettings = layerSettings;
            currentSettings.ApplyCollisionMatrix();
            settingsChanged?.Invoke(old, layerSettings);
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
