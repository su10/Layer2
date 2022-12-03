using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Jagapippi.Layer2
{
    internal static class BitHelper
    {
        public static bool CheckBit(int @int, int index)
        {
            if (index is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(index));

            return _CheckBit(@int, index);
        }

        private static bool _CheckBit(int @int, int index)
        {
            return ((@int & (1 << index)) != 0);
        }

        public static void SetBit(ref int @int, int index, bool value)
        {
            if (index is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(index));

            _SetBit(ref @int, index, value);
        }

        private static void _SetBit(ref int @int, int index, bool value)
        {
            if (value)
            {
                @int |= 1 << index;
            }
            else
            {
                @int &= ~(1 << index);
            }
        }

        public static int GetMask(int @int, int index)
        {
            return GetMask(@int, index, index);
        }

        public static int GetMask(int @int, int from, int to)
        {
            if (from is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(from));
            if (to is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(to));
            if (to < from) throw new ArgumentException();

            return _GetMask(@int, from, to);
        }

        private static int _GetMask(int @int, int from, int to)
        {
            var @uint = Int32ToUInt32(@int);
            @uint = ((@uint << (32 - to - 1)) >> (32 - to - 1)) & ((@uint >> from) << from);
            return UInt32ToInt32(@uint);
        }

        internal static uint Int32ToUInt32(int @int)
        {
            return UnsafeUtility.As<int, uint>(ref @int);
        }

        internal static int UInt32ToInt32(uint @uint)
        {
            return UnsafeUtility.As<uint, int>(ref @uint);
        }

        public static void Reorder(ref int @int, int src, int dest)
        {
            if (src is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(src));
            if (dest is < 0 or >= 32) throw new ArgumentOutOfRangeException(nameof(dest));

            _Reorder(ref @int, src, dest);
        }

        private static void _Reorder(ref int @int, int src, int dest)
        {
            if (src == dest) return;

            var srcBit = CheckBit(@int, src);
            var smallerIndex = Mathf.Min(src, dest);
            var largerIndex = Mathf.Max(src, dest);

            var leftMask = (largerIndex < 31) ? _GetMask(@int, largerIndex + 1, 31) : 0;
            var rightMask = (0 < smallerIndex) ? _GetMask(@int, 0, smallerIndex - 1) : 0;

            var centerMask = _GetMask(@int, smallerIndex, largerIndex);
            SetBit(ref centerMask, src, false);
            LogicalShift(ref centerMask, (src < dest) ? ShiftDirection.Right : ShiftDirection.Left);

            @int = leftMask | rightMask | centerMask;

            if (srcBit) SetBit(ref @int, dest, true);
        }

        private static void LogicalShift(ref int @int, ShiftDirection direction)
        {
            switch (direction)
            {
                case ShiftDirection.Right:
                {
                    @int = UInt32ToInt32(Int32ToUInt32(@int) >> 1);
                    break;
                }
                case ShiftDirection.Left:
                {
                    @int <<= 1;
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private enum ShiftDirection
        {
            Right,
            Left,
        }
    }
}
