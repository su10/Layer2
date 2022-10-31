using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [Serializable]
    public struct Layer2Layer
    {
        [SerializeField] internal int _value;

        public Layer2Layer(int value)
        {
            if (value is < 0 or >= Layer.MaxCount) throw new ArgumentOutOfRangeException(nameof(value));

            _value = value;
        }

        public static implicit operator int(Layer2Layer layer) => layer._value;
        public static implicit operator Layer2Layer(int integer) => new(integer);
    }
}
