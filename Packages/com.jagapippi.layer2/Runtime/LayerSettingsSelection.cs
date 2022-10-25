using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal static class LayerSettingsSelection
    {
#if UNITY_EDITOR
        private static readonly UnityEngine.Object PhysicsManager = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(PhysicsManager));
        private static readonly UnityEngine.Object Physics2DSettings = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(Physics2DSettings));
        private static readonly UnityEngine.Object TagManager = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(TagManager));
#endif
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
            void LocalApply()
            {
#if UNITY_EDITOR
                Undo.SetCurrentGroupName("Modified Properties in Project Settings");
                Undo.RecordObjects(new[] { PhysicsManager, Physics2DSettings, TagManager }, "");

                var tagManager = new SerializedObject(TagManager);
                var layersProperty = tagManager.FindProperty("layers");
#endif
                for (var i = 0; i < Layer.MaxCount; i++)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                    {
                        layersProperty.GetArrayElementAtIndex(i).stringValue = active.LayerToName(i);
                    }
#endif
                    for (var j = 0; j < Layer.MaxCount - i; j++)
                    {
                        Physics.IgnoreLayerCollision(i, j, active.GetIgnoreCollision(i, j));
                        Physics2D.IgnoreLayerCollision(i, j, active.GetIgnoreCollision2D(i, j));
                    }
                }
#if UNITY_EDITOR
                tagManager.ApplyModifiedProperties();

                EditorUtility.SetDirty(PhysicsManager);
                EditorUtility.SetDirty(Physics2DSettings);
                EditorUtility.SetDirty(TagManager);

                EditorApplication.ExecuteMenuItem("File/Save Project");
#endif
            }

            if (_active == layerSettings)
            {
                LocalApply();
                return;
            }

            var old = _active;
            _active = layerSettings;
            LocalApply();

#if UNITY_EDITOR
            changed?.Invoke(old, active);
#endif
        }
    }
}
