using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    public partial class LayerSettingsAsset : ScriptableObject, ILayerSettings
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        [SerializeField] internal PhysicsDimensions _physicsDimensions;

        [SerializeField] internal SerializableLayer[] _layers =
        {
            BuiltinLayer.Default.AsSerializable(),
            BuiltinLayer.TransparentFX.AsSerializable(),
            BuiltinLayer.IgnoreRaycast.AsSerializable(),
            new("", 3),
            BuiltinLayer.Water.AsSerializable(),
            BuiltinLayer.UI.AsSerializable(),
            new("", 6),
            new("", 7),
            new("", 8),
            new("", 9),
            new("", 10),
            new("", 11),
            new("", 12),
            new("", 13),
            new("", 14),
            new("", 15),
            new("", 16),
            new("", 17),
            new("", 18),
            new("", 19),
            new("", 20),
            new("", 21),
            new("", 22),
            new("", 23),
            new("", 24),
            new("", 25),
            new("", 26),
            new("", 27),
            new("", 28),
            new("", 29),
            new("", 30),
            new("", 31),
        };

        #region ILayerSettings

        public PhysicsDimensions physicsDimensions => _physicsDimensions;

        public string LayerToName(int layer) => _layers[layer];

        public int NameToLayer(string name)
        {
            name ??= string.Empty;

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerName = _layers[i].name;

                if (name == layerName) return i;
            }

            return -1;
        }

        public bool GetCollision(int layer1, int layer2)
        {
            var a = _layers[layer1].GetCollision(layer2);
            var b = _layers[layer2].GetCollision(layer1);

            if (a != b) throw new DataInconsistencyException();

            return a;
        }

#if UNITY_EDITOR
        public event Action<ILayerSettings> changedSerializedObject;
#endif

        #endregion

#if UNITY_EDITOR

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() => changedSerializedObject?.Invoke(this);

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        #endregion

#endif
    }
}
