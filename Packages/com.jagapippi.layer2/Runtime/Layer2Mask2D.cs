using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [Serializable]
    public struct Layer2Mask2D
    {
        public static string LayerToName(int layer) => LayerSettingsSelection2D.current.LayerToName(layer);
        public static int NameToLayer(string layerName) => LayerSettingsSelection2D.current.NameToLayer(layerName);

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

        [SerializeField] internal int _value;

        public int value
        {
            get => _value;
            set => _value = value;
        }

        public static implicit operator int(Layer2Mask2D self) => self._value;

        public static implicit operator Layer2Mask2D(int value)
        {
            Layer2Mask2D layer2Mask2D;
            layer2Mask2D._value = value;
            return layer2Mask2D;
        }

        public static implicit operator LayerMask(Layer2Mask2D self) => self._value;
        public static implicit operator Layer2Mask2D(LayerMask self) => (int)self;
    }
}
