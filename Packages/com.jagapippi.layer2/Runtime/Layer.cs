using System;

namespace Jagapippi.Layer2
{
    internal class Layer
    {
        public const int MaxCount = 32;

        public virtual string name { get; }
        public virtual int index { get; }
        protected virtual int collisionMatrix { get; }

        protected Layer(string name, int index, int collisionMatrix = -1)
        {
            if (index is < 0 or >= MaxCount) throw new ArgumentOutOfRangeException(nameof(index));

            this.name = name ?? "";
            this.index = index;
            this.collisionMatrix = collisionMatrix;
        }

        public bool GetCollision(Layer layer) => this.GetCollision(layer.index);
        public bool GetCollision(int index) => BitHelper.CheckBit(this.collisionMatrix, index);

        public override string ToString() => this.name;
        public static implicit operator string(Layer layer) => layer.name;
    }
}
