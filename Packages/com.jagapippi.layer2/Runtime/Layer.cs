using System;

namespace Jagapippi.Layer2
{
    internal class Layer
    {
        public const int MaxCount = 32;

        public virtual string name { get; }
        public virtual int index { get; }
        protected virtual int collisionMatrix { get; }
        protected virtual int collisionMatrix2D { get; }

        protected Layer(string name, int index, int collisionMatrix = -1, int collisionMatrix2D = -1)
        {
            if (index is < 0 or >= MaxCount) throw new ArgumentOutOfRangeException(nameof(index));

            this.name = name ?? "";
            this.index = index;
            this.collisionMatrix = collisionMatrix;
            this.collisionMatrix2D = collisionMatrix2D;
        }

        public bool GetCollision(Layer layer) => this.GetCollision(layer.index);
        public bool GetCollision(int layer) => BitHelper.CheckBit(this.collisionMatrix, layer);

        public bool GetCollision2D(Layer layer) => this.GetCollision2D(layer.index);
        public bool GetCollision2D(int layer) => BitHelper.CheckBit(this.collisionMatrix2D, layer);

        public override string ToString() => this.name;
        public static implicit operator string(Layer layer) => layer.name;
    }
}
