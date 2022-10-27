using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Jagapippi.Layer2
{
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public sealed class LayerManager : MonoBehaviour
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void OnInitializedOnLoad()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredEditMode:
                    case PlayModeStateChange.ExitingEditMode:
                    {
                        break;
                    }
                    case PlayModeStateChange.EnteredPlayMode:
                    {
                        _instance = EditorHelper.FindComponentInCurrentHierarchy<LayerManager>();
                        break;
                    }
                    case PlayModeStateChange.ExitingPlayMode:
                    {
                        _instance = null;
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            {
                PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
                PrefabStage.prefabStageClosing += OnPrefabStageClosing;

                void OnPrefabStageClosing(PrefabStage stage)
                {
                    EditorApplication.delayCall += FindAndSetInstance;
                }

                void FindAndSetInstance()
                {
                    _instance = EditorHelper.FindComponentInCurrentHierarchy<LayerManager>();

                    if (_instance)
                    {
                        _instance.SubscribeChangedEvent();
                        ApplyCurrentSettingsAsset();
                    }
                }
            }
            {
                PrefabStage.prefabStageOpened -= FindAndSetInstance;
                PrefabStage.prefabStageOpened += FindAndSetInstance;

                void FindAndSetInstance(PrefabStage stage)
                {
                    if (_instance) _instance.UnsubscribeChangedEvent();
                    _instance = EditorHelper.FindComponentInCurrentHierarchy<LayerManager>();
                }
            }
        }

        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _instance = null;
        }
#endif

        private static readonly string DestroyedDuplicatedInstanceWarning = $"instance of {nameof(LayerManager)} was destroyed due to duplication.";
        private static readonly string DestroyedDuplicatedOldInstanceWarning = $"The old {DestroyedDuplicatedInstanceWarning}";
        private static readonly string DestroyedDuplicatedNewInstanceWarning = $"The new {DestroyedDuplicatedInstanceWarning}";

        private static LayerManager _instance;

        private static LayerManager instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(nameof(LayerManager));
                    _instance = go.AddComponent<LayerManager>();
                }

                return _instance;
            }
        }

        public static ILayerSettings currentSettings => LayerSettingsSelection.active;

        private static LayerSettingsAsset currentSettingsAsset
        {
            get
            {
                if (_instance == null) return null;
                if (_instance._settingsAsset) return _instance._settingsAsset;
                return _instance._defaultSettingsAsset;
            }
        }

        private bool IsInSameHierarchy(LayerManager other)
        {
            if ((this == null) || (other == null)) return false;
#if UNITY_EDITOR
            if (this.isPartOfPrefabAsset || other.isPartOfPrefabAsset) return false;
#endif
            if (this.gameObject.DontDestroyOnLoadActivated()) return true;
            if (other.gameObject.DontDestroyOnLoadActivated()) return true;

            return (this.gameObject.scene == other.gameObject.scene);
        }

        public static void ApplySettings(ILayerSettings settings)
        {
            instance._settingsAsset = settings as LayerSettingsAsset;

            SelectOrApply(settings);
        }

        private static void ApplyCurrentSettingsAsset()
        {
            SelectOrApply(currentSettingsAsset);
        }

        private static void ApplyDefaultSettingsAsset()
        {
            SelectOrApply(instance ? instance._defaultSettingsAsset : null);
        }

        private static void SelectOrApply(ILayerSettings layerSettings)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                LayerSettingsSelection.Select(layerSettings);
            }
            else
#endif
            {
                LayerSettingsSelection.Apply(layerSettings);
            }
        }

        [SerializeField] private LayerSettingsAsset _defaultSettingsAsset;
        [SerializeField] private LayerSettingsAsset _settingsAsset;
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private bool _destroyOlderIfDuplicated;
        [SerializeField] private bool _suppressDuplicateWarning;

#if UNITY_EDITOR
        private bool isPartOfPrefabAsset => (this.gameObject.scene.IsValid() == false);

        void OnValidate()
        {
            if (this.isPartOfPrefabAsset) return;
            if (_instance != this) return;

            this.UnsubscribeChangedEvent();
            {
                ApplyCurrentSettingsAsset();
            }
            this.SubscribeChangedEvent();
        }
#endif

        void Awake()
        {
            // NOTE: Awake() are not called in case of not reload scene when entering play mode, so we don't use it.
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if ((Application.isPlaying == false) && EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            if (this == _instance) return;

            if ((_instance == null) || (this.IsInSameHierarchy(_instance) == false))
            {
                _instance = this;

                ApplyCurrentSettingsAsset();
            }
            else
            {
                var destroyOlderIfDuplicated = _instance._destroyOlderIfDuplicated;
                var suppressDuplicateWarning = _instance._suppressDuplicateWarning;

                if (destroyOlderIfDuplicated)
                {
                    _instance.gameObject.Destroy();
                    _instance = this;

                    ApplyCurrentSettingsAsset();
                }
                else
                {
                    this.gameObject.Destroy();
                }

                if (suppressDuplicateWarning == false)
                {
                    var warning = destroyOlderIfDuplicated
                        ? DestroyedDuplicatedOldInstanceWarning
                        : DestroyedDuplicatedNewInstanceWarning;

                    Debug.LogWarning(warning);
                }
            }

            this.ApplyDontDestroyOnLoadIfNeed();
#if UNITY_EDITOR
            this.SubscribeChangedEvent();
#endif
        }

        void Update()
        {
            this.ApplyDontDestroyOnLoadIfNeed();
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            this.UnsubscribeChangedEvent();
#endif
            if (_instance == this) ApplyDefaultSettingsAsset();
        }

        void OnDestroy()
        {
            if (_instance == this) SelectOrApply(null);
        }

#if UNITY_EDITOR
        private void SubscribeChangedEvent()
        {
            LayerSettingsSelection.changed -= this.OnLayerSettingsSelectionChanged;
            LayerSettingsSelection.changed += this.OnLayerSettingsSelectionChanged;
        }

        private void UnsubscribeChangedEvent()
        {
            LayerSettingsSelection.changed -= this.OnLayerSettingsSelectionChanged;
        }

        private void OnLayerSettingsSelectionChanged(ILayerSettings oldSettings, ILayerSettings newSettings)
        {
            if ((ILayerSettings)_settingsAsset == newSettings) return;
            if (LayerSettingsSelection.active is not LayerSettingsAsset asset) return;
            if (_settingsAsset == asset) return;

            _settingsAsset = asset;
            EditorUtility.SetDirty(this);
        }
#endif

        private void ApplyDontDestroyOnLoadIfNeed()
        {
            if (Application.isPlaying == false) return;
            if (_dontDestroyOnLoad == false) return;
            if (this.gameObject.DontDestroyOnLoadActivated()) return;

            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
