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
        [SerializeField] internal int _collisionMatrix2D;

        public override string name => _name;
        public override int index => _index;
        protected override int collisionMatrix => _collisionMatrix;
        protected override int collisionMatrix2D => _collisionMatrix2D;

        internal SerializableLayer(string name, int index, int collisionMatrix = -1, int collisionMatrix2D = -1) : base(name, index, collisionMatrix, collisionMatrix2D)
        {
            _name = base.name;
            _index = base.index;
            _collisionMatrix = base.collisionMatrix;
            _collisionMatrix2D = base.collisionMatrix2D;
        }
    }
}
