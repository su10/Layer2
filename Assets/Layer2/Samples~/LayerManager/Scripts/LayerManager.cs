using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Jagapippi.Layer2.Samples
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
                        _instance = FindComponentInCurrentHierarchy<LayerManager>();
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
                    _instance = FindComponentInCurrentHierarchy<LayerManager>();

                    if (_instance)
                    {
                        _instance.SubscribeChangedEvent();
                        ApplySettingAsset();
                    }
                }
            }
            {
                PrefabStage.prefabStageOpened -= FindAndSetInstance;
                PrefabStage.prefabStageOpened += FindAndSetInstance;

                void FindAndSetInstance(PrefabStage stage)
                {
                    _instance = FindComponentInCurrentHierarchy<LayerManager>();
                }
            }

            static T FindComponentInCurrentHierarchy<T>() where T : Component
            {
                var currentStageHandle = StageUtility.GetCurrentStageHandle();

                return currentStageHandle.IsValid()
                    ? currentStageHandle.FindComponentOfType<T>()
                    : FindObjectOfType<T>();
            }
        }

        [InitializeOnEnterPlayMode]
        static void OnInitializeOnEnterPlayMode()
        {
            _instance = null;
        }
#endif

        private const string DestroyedDuplicatedInstanceWarning = "The {0} instance \"{1}\" was destroyed due to duplication.";

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

        public static ILayerSetting activeSetting => LayerSettingSelection.activeSetting;

#if UNITY_EDITOR
        private bool isInCurrentHierarchy => StageUtility.GetCurrentStageHandle().Contains(this.gameObject);
#endif
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

        public static void ApplySetting(ILayerSetting setting)
        {
            instance._settingAsset = setting as LayerSettingAsset;

            SelectOrApply(setting);
        }

        private static void ApplySettingAsset()
        {
            var asset = _instance switch
            {
                _ when (_instance == null) => null,
                _ when (_instance._settingAsset) => _instance._settingAsset,
                _ => _instance._defaultSettingAsset,
            };

            SelectOrApply(asset);
        }

        private static void ApplyDefaultSettingAsset()
        {
            SelectOrApply(instance ? instance._defaultSettingAsset : null);
        }

        private static void SelectOrApply(ILayerSetting layerSetting)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                LayerSettingSelection.Select(layerSetting);
            }
            else
#endif
            {
                LayerSettingSelection.Apply(layerSetting);
            }
        }

        [SerializeField] private LayerSettingAsset _defaultSettingAsset;
        [SerializeField] private LayerSettingAsset _settingAsset;
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private DestroyDuplicatedInstance _destroyDuplicatedInstance;
        [SerializeField] private bool _suppressDuplicateWarning;

#if UNITY_EDITOR
        private bool isPartOfPrefabAsset => (this.gameObject.scene.IsValid() == false);

        void OnValidate()
        {
            if (this.isPartOfPrefabAsset) return;
            if (_instance != this) return;

            this.UnsubscribeChangedEvent();
            {
                ApplySettingAsset();
            }
            this.SubscribeChangedEvent();
        }
#endif

        void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            if ((_instance == null) || this.IsInSameHierarchy(_instance)) return;

            _instance.UnsubscribeChangedEvent();
#endif
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                if (this.isInCurrentHierarchy == false) return;
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            }
#endif
            if (this == _instance) return;

            if ((_instance == null) || (this.IsInSameHierarchy(_instance) == false))
            {
                _instance = this;

                ApplySettingAsset();
            }
            else
            {
                var destroyDuplication = _instance._destroyDuplicatedInstance;
                var suppressDuplicateWarning = _instance._suppressDuplicateWarning;

                switch (destroyDuplication)
                {
                    case DestroyDuplicatedInstance.Newer:
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying == false)
                        {
                            EditorApplication.delayCall += () => this.gameObject.Destroy();
                        }
                        else
#endif
                        {
                            this.gameObject.Destroy();
                        }

                        break;
                    }
                    case DestroyDuplicatedInstance.Older:
                    {
                        _instance.gameObject.Destroy();
                        _instance = this;

                        ApplySettingAsset();

                        break;
                    }
                    default: throw new ArgumentOutOfRangeException();
                }

                if (suppressDuplicateWarning == false)
                {
                    var oldOrNew = _destroyDuplicatedInstance switch
                    {
                        DestroyDuplicatedInstance.Newer => "new",
                        DestroyDuplicatedInstance.Older => "old",
                        _ => throw new ArgumentOutOfRangeException(),
                    };

                    Debug.LogWarning(string.Format(DestroyedDuplicatedInstanceWarning, oldOrNew, this.name));
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
            if (_instance == this) ApplyDefaultSettingAsset();
        }

        void OnDestroy()
        {
            if (_instance == this) SelectOrApply(null);
        }

#if UNITY_EDITOR
        private void SubscribeChangedEvent()
        {
            LayerSettingSelection.changed -= this.OnLayerSettingSelectionChanged;
            LayerSettingSelection.changed += this.OnLayerSettingSelectionChanged;
        }

        private void UnsubscribeChangedEvent()
        {
            LayerSettingSelection.changed -= this.OnLayerSettingSelectionChanged;
        }

        private void OnLayerSettingSelectionChanged(ILayerSetting oldSetting, ILayerSetting newSetting)
        {
            if (this == null) return;
            if ((ILayerSetting)_settingAsset == newSetting) return;
            if (LayerSettingSelection.activeSetting is not LayerSettingAsset asset) return;
            if (_settingAsset == asset) return;

            _settingAsset = asset;
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

        public enum DestroyDuplicatedInstance
        {
            Newer,
            Older,
        }
    }
}
