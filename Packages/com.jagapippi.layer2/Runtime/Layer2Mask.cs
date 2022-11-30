using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [Serializable]
    public struct Layer2Mask
    {
        public static string LayerToName(int layer) => LayerSettingSelection.activeSetting.LayerToName(layer);
        public static int NameToLayer(string layerName) => LayerSettingSelection.activeSetting.NameToLayer(layerName);

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

        [SerializeField] internal uint m_Bits;

        public int value
        {
            get => BitHelper.UInt32ToInt32(m_Bits);
            set => m_Bits = BitHelper.Int32ToUInt32(value);
        }

        public static implicit operator int(Layer2Mask self) => self.value;

        public static implicit operator Layer2Mask(int value)
        {
            Layer2Mask layer2Mask;
            layer2Mask.m_Bits = BitHelper.Int32ToUInt32(value);
            return layer2Mask;
        }

        public static implicit operator LayerMask(Layer2Mask self) => self.value;
        public static implicit operator Layer2Mask(LayerMask self) => (int)self;
    }
}
