using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [Serializable]
    public struct Layer2Mask : ISerializationCallbackReceiver
    {
        public static string LayerToName(int layer) => LayerSettingsSelection.activeSettings.LayerToName(layer);
        public static int NameToLayer(string layerName) => LayerSettingsSelection.activeSettings.NameToLayer(layerName);

        public static int GetMask(params string[] layerNames)
        {
            if (layerNames == null) throw new ArgumentNullException(nameof(layerNames));

            var mask = 0;

            foreach (var layerName in layerNames)
            {
                var layerIndex = NameToLayer(layerName);
                if (0 < layerIndex) mask |= (1 << layerIndex);
            }

            return mask;
        }

        [SerializeField] internal int serializedVersion;
        [SerializeField] internal uint m_Bits;
        [SerializeField] internal bool _migrated;
        [SerializeField] internal int _value;

        public int value
        {
            get => _value;
            set => _value = value;
        }

        public static implicit operator int(Layer2Mask self) => self._value;

        public static implicit operator Layer2Mask(int value)
        {
            Layer2Mask layer2Mask;
            layer2Mask.serializedVersion = 0;
            layer2Mask.m_Bits = 0;
            layer2Mask._migrated = true;
            layer2Mask._value = value;
            return layer2Mask;
        }

        public static implicit operator LayerMask(Layer2Mask self) => self._value;
        public static implicit operator Layer2Mask(LayerMask self) => (int)self;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Bits = BitHelper.Int32ToUInt32(_value);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (serializedVersion == default) return;
            if (_migrated) return;

            _value = BitHelper.UInt32ToInt32(m_Bits);
            _migrated = true;
        }

        #endregion
    }
}
