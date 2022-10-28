using System;
using UnityEngine;
#if UNITY_EDITOR
using Jagapippi.Layer2.Editor.UnityEditorInternal;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
#endif

namespace Jagapippi.Layer2
{
    internal static class LayerSettingsSelection
    {
#if UNITY_EDITOR
        private static readonly string ShouldLoadActiveLayerSettingsAssetKey = nameof(ShouldLoadActiveLayerSettingsAssetKey);

        private static readonly UnityEngine.Object PhysicsManager = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(PhysicsManager));
        private static readonly UnityEngine.Object Physics2DSettings = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(Physics2DSettings));
        private static readonly UnityEngine.Object TagManager = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(TagManager));
#endif
        private static ILayerSettings _activeSettings;
        public static ILayerSettings activeSettings => _activeSettings ?? EmptyLayerSettings.instance;

#if UNITY_EDITOR
        public static event Action<ILayerSettings, ILayerSettings> changed;

        [InitializeOnLoadMethod]
        static void OnInitializeOnLoadMethod()
        {
            EventCallbackHelper.didReloadScriptsAfterCompilation -= OnCompilationFinished;
            EventCallbackHelper.didReloadScriptsAfterCompilation += OnCompilationFinished;
            void OnCompilationFinished() => LoadActiveAsset();

            EventCallbackHelper.initializeOnEnterPlayMode -= OnInitializeOnEnterPlayMode;
            EventCallbackHelper.initializeOnEnterPlayMode += OnInitializeOnEnterPlayMode;
            void OnInitializeOnEnterPlayMode() => _activeSettings = null;

            EventCallbackHelper.enteredEditMode -= OnEnteredEditMode;
            EventCallbackHelper.enteredEditMode += OnEnteredEditMode;
            void OnEnteredEditMode() => LoadActiveAsset();
        }

        private static void SaveActiveAssetGUIDIfEditMode()
        {
            if (Application.isPlaying) return;

            if (_activeSettings is LayerSettingsAsset settingsAsset)
            {
                var path = AssetDatabase.GetAssetPath(settingsAsset);
                var guid = AssetDatabase.AssetPathToGUID(path);

                SessionState.SetString(ShouldLoadActiveLayerSettingsAssetKey, guid);
            }
            else
            {
                SessionState.SetString(ShouldLoadActiveLayerSettingsAssetKey, null);
            }
        }

        private static void LoadActiveAsset()
        {
            var guid = SessionState.GetString(ShouldLoadActiveLayerSettingsAssetKey, null);

            if (string.IsNullOrEmpty(guid) == false)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                _activeSettings = AssetDatabase.LoadAssetAtPath<LayerSettingsAsset>(path);
            }
            else
            {
                _activeSettings = null;
            }
        }

        internal static void Select(ILayerSettings layerSettings)
        {
            if (_activeSettings == layerSettings) return;

            var old = _activeSettings;
            _activeSettings = layerSettings;

            SaveActiveAssetGUIDIfEditMode();

            changed?.Invoke(old, activeSettings);

            InspectorWindow.RepaintAllInspectors();
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
                        layersProperty.GetArrayElementAtIndex(i).stringValue = activeSettings.LayerToName(i);
                    }
#endif
                    for (var j = 0; j < Layer.MaxCount - i; j++)
                    {
                        Physics.IgnoreLayerCollision(i, j, activeSettings.GetIgnoreCollision(i, j));
                        Physics2D.IgnoreLayerCollision(i, j, activeSettings.GetIgnoreCollision2D(i, j));
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

            if (_activeSettings == layerSettings)
            {
                LocalApply();
                return;
            }

            var old = _activeSettings;
            _activeSettings = layerSettings;
            LocalApply();

#if UNITY_EDITOR
            SaveActiveAssetGUIDIfEditMode();

            changed?.Invoke(old, activeSettings);

            InspectorWindow.RepaintAllInspectors();
#endif
        }

#if UNITY_EDITOR
        private static class EventCallbackHelper
        {
            private static readonly string CompilationFinishedKey = nameof(CompilationFinishedKey);

            public static event Action initializeOnEnterPlayMode;
            public static event Action didReloadScriptsAfterCompilation;
            public static event Action enteredEditMode;

            [InitializeOnLoadMethod]
            static void OnInitializeOnLoadMethod()
            {
                CompilationPipeline.compilationFinished -= OnCompilationFinished;
                CompilationPipeline.compilationFinished += OnCompilationFinished;

                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }

            static void OnCompilationFinished(object obj)
            {
                SessionState.SetBool(CompilationFinishedKey, true);
            }

            [InitializeOnEnterPlayMode]
            static void OnInitializeOnEnterPlayMode()
            {
                initializeOnEnterPlayMode?.Invoke();
            }

            [DidReloadScripts]
            static void OnDidReloadScripts()
            {
                // NOTE: this function called in entering play mode if domain reloading is enabled.

                var isAfterCompilation = SessionState.GetBool(CompilationFinishedKey, false);
                if (isAfterCompilation) didReloadScriptsAfterCompilation?.Invoke();

                SessionState.SetBool(CompilationFinishedKey, false);
            }

            static void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredEditMode:
                    {
                        enteredEditMode?.Invoke();
                        break;
                    }
                    case PlayModeStateChange.ExitingEditMode:
                    case PlayModeStateChange.EnteredPlayMode:
                    case PlayModeStateChange.ExitingPlayMode:
                    {
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }
#endif
    }
}
