using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class LayerManager : MonoBehaviour
    {
#if UNITY_EDITOR
        private static bool _shouldRefresh;

        [InitializeOnLoadMethod]
        static void OnInitializedOnLoad()
        {
            _shouldRefresh = true;
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

        public static ILayerSettings currentSettings
        {
            get
            {
                if (_instance == null)
                {
                    var _ = new GameObject(nameof(LayerManager), typeof(LayerManager));
                }

                return Layer2Core.currentSettings;
            }
        }

        public static void ApplySettings(ILayerSettings settings) => Layer2Core.SetCurrentSettings(settings);
        private static void ApplySettings() => ApplySettings(_instance ? _instance._settingsAsset : null);

        [SerializeField] private LayerSettingsAsset _settingsAsset;
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private bool _destroyOlderIfDuplicated;
        [SerializeField] private bool _suppressDuplicateWarning;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;

                ApplySettings();
            }
            else if (_instance != this)
            {
                var destroyOlderIfDuplicated = _instance._destroyOlderIfDuplicated;
                var suppressDuplicateWarning = _instance._suppressDuplicateWarning;

                if (destroyOlderIfDuplicated)
                {
                    _instance.gameObject.Destroy();
                    _instance = this;
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
        }

        void Update()
        {
            if (_instance == null) this.Awake();

#if UNITY_EDITOR
            if (_shouldRefresh)
            {
                _shouldRefresh = false;
                ApplySettings();
            }
#endif

            if (_dontDestroyOnLoad && Application.isPlaying)
            {
                if (this.gameObject.DontDestroyOnLoadActivated() == false)
                {
                    this.transform.SetParent(null);

                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

#if UNITY_EDITOR
        void OnValidate() => ApplySettings();
#endif
        void OnEnable()
        {
#if UNITY_EDITOR
            Layer2Core.settingsChanged += OnLayer2CoreSettingsChanged;
#endif
            ApplySettings();
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            Layer2Core.settingsChanged -= OnLayer2CoreSettingsChanged;
#endif
            ApplySettings(null);
        }

#if UNITY_EDITOR
        void OnLayer2CoreSettingsChanged(ILayerSettings oldSettings, ILayerSettings newSettings)
        {
            if ((ILayerSettings)_settingsAsset != newSettings)
            {
                if (Layer2Core.currentSettings is LayerSettingsAsset asset)
                {
                    _settingsAsset = asset;
                }
            }
        }
#endif

        void OnDestroy() => ApplySettings(null);
    }
}
