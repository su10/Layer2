using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [Serializable]
    internal class SerializableLayer : Layer
    {
        [SerializeField] internal string _name;
        [SerializeField] internal int _index;
        [SerializeField] internal int _collisionMatrix;

        public override string name => _name;
        public override int index => _index;
        protected override int collisionMatrix => _collisionMatrix;

        internal SerializableLayer(string name, int index, int collisionMatrix = -1) : base(name, index, collisionMatrix)
        {
            _name = base.name;
            _index = base.index;
            _collisionMatrix = base.collisionMatrix;
        }

        internal void SetCollision(Layer layer, bool enable) => this.SetCollision(layer.index, enable);
        internal void SetCollision(int index, bool enable) => BitHelper.SetBit(ref _collisionMatrix, index, enable);
    }
}
