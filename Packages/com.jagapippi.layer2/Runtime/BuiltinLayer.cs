using System.Collections.Generic;

namespace Jagapippi.Layer2
{
    internal class BuiltinLayer : Layer
    {
        public static readonly BuiltinLayer Default = new(nameof(Default), 0);
        public static readonly BuiltinLayer TransparentFX = new(nameof(TransparentFX), 1);
        public static readonly BuiltinLayer IgnoreRaycast = new("Ignore Raycast", 2);
        public static readonly BuiltinLayer Water = new(nameof(Water), 4);
        public static readonly BuiltinLayer UI = new(nameof(UI), 5);

        public static readonly IReadOnlyList<int> indexes = new[]
        {
            Default.index,
            TransparentFX.index,
            IgnoreRaycast.index,
            Water.index,
            UI.index,
        };

        private BuiltinLayer(string name, int index, int collisionMatrix = -1) : base(name, index, collisionMatrix)
        {
        }

        public SerializableLayer AsSerializable() => new(this.name, this.index, this.collisionMatrix);
    }
}
