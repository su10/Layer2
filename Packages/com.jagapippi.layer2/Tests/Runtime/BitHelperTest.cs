using System;
using NUnit.Framework;
using UnityEngine;

namespace Jagapippi.Layer2.Tests
{
    public class BitHelperTest
    {
        [Test]
        public void CheckBit()
        {
            {
                const int target = 0;

                for (var i = 0; i < 8; i++)
                {
                    Assert.AreEqual(false, BitHelper.CheckBit(target, i));
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_11111111_11111111);

                for (var i = 0; i < 8; i++)
                {
                    Assert.AreEqual(true, BitHelper.CheckBit(target, i));
                }
            }
            {
                const int target = 0b_00000000_11111111_00000000_11111111;

                for (var i = 0; i < 8; i++)
                {
                    Assert.AreEqual(true, BitHelper.CheckBit(target, i));
                }

                for (var i = 9; i < 16; i++)
                {
                    Assert.AreEqual(false, BitHelper.CheckBit(target, i));
                }

                for (var i = 17; i < 24; i++)
                {
                    Assert.AreEqual(true, BitHelper.CheckBit(target, i));
                }

                for (var i = 25; i < 32; i++)
                {
                    Assert.AreEqual(false, BitHelper.CheckBit(target, i));
                }
            }
        }

        [Test]
        public void SetBit()
        {
            // True
            {
                const int target = 0;

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual((1 << i), temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_11111111_11111111);

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual(target, temp);
                }
            }
            {
                const int target = 0b_00000000_00000000_11111111_11111111;

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual(target, temp);
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual(target + (1 << i), temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_00000000_00000000);

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual(target + (1 << i), temp);
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    Assert.AreEqual(target, temp);
                }
            }
            {
                const int target = 0b_01010101_01010101_10101010_10101010;

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    if (i % 2 == 0)
                    {
                        Assert.AreEqual(target + (1 << i), temp);
                    }
                    else
                    {
                        Assert.AreEqual(target, temp);
                    }
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, true);

                    if (i % 2 == 0)
                    {
                        Assert.AreEqual(target, temp);
                    }
                    else
                    {
                        Assert.AreEqual(target + (1 << i), temp);
                    }
                }
            }
            // False
            {
                const int target = 0;

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(0, temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_11111111_11111111);

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(~(1 << i), temp);
                }
            }
            {
                const int target = 0b_00000000_00000000_11111111_11111111;

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(target - (1 << i), temp);
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(target, temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_00000000_00000000);

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(target, temp);
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    Assert.AreEqual(target - (1 << i), temp);
                }
            }
            {
                const int target = 0b_01010101_01010101_10101010_10101010;

                for (var i = 0; i < 16; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    if (i % 2 == 0)
                    {
                        Assert.AreEqual(target, temp);
                    }
                    else
                    {
                        Assert.AreEqual(target - (1 << i), temp);
                    }
                }

                for (var i = 17; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.SetBit(ref temp, i, false);

                    if (i % 2 == 0)
                    {
                        Assert.AreEqual(target - (1 << i), temp);
                    }
                    else
                    {
                        Assert.AreEqual(target, temp);
                    }
                }
            }
        }

        [Test]
        public void GetMask_index()
        {
            {
                const int target = 0;

                for (var i = 0; i < 32; i++)
                {
                    Assert.AreEqual(0, BitHelper.GetMask(target, i));
                }
            }
            {
                var target = BitConverter.ToInt32(BitConverter.GetBytes(0b_11111111_11111111_11111111_11111111));

                for (var i = 0; i < 32; i++)
                {
                    Assert.AreEqual((1 << i), BitHelper.GetMask(target, i));
                }
            }
            {
                var target = BitConverter.ToInt32(BitConverter.GetBytes(0b_00000000_00000000_11111111_11111111));

                for (var i = 0; i < 16; i++)
                {
                    Assert.AreEqual((1 << i), BitHelper.GetMask(target, i));
                }

                for (var i = 17; i < 32; i++)
                {
                    Assert.AreEqual(0, BitHelper.GetMask(target, i));
                }
            }
            {
                var target = BitConverter.ToInt32(BitConverter.GetBytes(0b_11111111_11111111_00000000_00000000));

                for (var i = 0; i < 16; i++)
                {
                    Assert.AreEqual(0, BitHelper.GetMask(target, i));
                }

                for (var i = 17; i < 32; i++)
                {
                    Assert.AreEqual((1 << i), BitHelper.GetMask(target, i));
                }
            }
            {
                const int target = 0b_01010101_01010101_01010101_01010101;

                for (var i = 0; i < 32; i++)
                {
                    if (i % 2 == 0)
                    {
                        Assert.AreEqual((1 << i), BitHelper.GetMask(target, i));
                    }
                    else
                    {
                        Assert.AreEqual(0, BitHelper.GetMask(target, i));
                    }
                }
            }
        }

        [Test]
        public void GetMask_from_to()
        {
            {
                const int target = 0;

                for (var i = 0; i < 32; i++)
                {
                    for (var j = i; j < 32; j++)
                    {
                        Assert.AreEqual(0, BitHelper.GetMask(target, i, j));
                    }
                }
            }
            {
                var target = BitConverter.ToInt32(BitConverter.GetBytes(0b_11111111_11111111_11111111_11111111));

                Assert.AreEqual(target, BitHelper.GetMask(target, 0, 31));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_00000000_00000000_00000000_00000011), BitHelper.GetMask(target, 0, 1));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_00000000_00000000_01111111_10000000), BitHelper.GetMask(target, 7, 14));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_11111111_00000000_00000000_00000000), BitHelper.GetMask(target, 24, 31));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_11000000_00000000_00000000_00000000), BitHelper.GetMask(target, 30, 31));
            }
            {
                var target = BitConverter.ToInt32(BitConverter.GetBytes(0b_01110000_10010011_00010101_11111100));

                Assert.AreEqual(target, BitHelper.GetMask(target, 0, 31));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_00000000_00000000_00000000_00000100), BitHelper.GetMask(target, 0, 2));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_00000000_00000000_00000001_11111100), BitHelper.GetMask(target, 1, 8));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_00000000_00000011_00010101_11000000), BitHelper.GetMask(target, 6, 17));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_01100000_00000000_00000000_00000000), BitHelper.GetMask(target, 29, 31));
                Assert.AreEqual(BitHelper.UInt32ToInt32(0b_01000000_00000000_00000000_00000000), BitHelper.GetMask(target, 30, 31));
            }
        }

        [Test]
        public void Int32ToUInt32()
        {
            {
                const int target = 0;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = 1;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = -1;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = int.MaxValue;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = int.MinValue;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = 5395739;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
            {
                const int target = -9462849;
                var expected = Convert.ToString(target, 2);
                var actual = Convert.ToString(BitHelper.Int32ToUInt32(target), 2);
                Debug.Log($"expected: {expected} actual: {actual}");
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UInt32ToInt32()
        {
            {
                const uint target = 0;
                Assert.AreEqual(Convert.ToString(target, 2), Convert.ToString(BitHelper.UInt32ToInt32(target), 2));
            }
            {
                const uint target = 1;
                Assert.AreEqual(Convert.ToString(target, 2), Convert.ToString(BitHelper.UInt32ToInt32(target), 2));
            }
            {
                const uint target = uint.MaxValue;
                Assert.AreEqual(Convert.ToString(target, 2), Convert.ToString(BitHelper.UInt32ToInt32(target), 2));
            }
            {
                const uint target = 5395739;
                Assert.AreEqual(Convert.ToString(target, 2), Convert.ToString(BitHelper.UInt32ToInt32(target), 2));
            }
        }

        [Test]
        public void Reorder()
        {
            {
                const int target = 0;

                for (var i = 0; i < 32; i++)
                {
                    for (var j = 0; j < 32; j++)
                    {
                        var temp = target;
                        BitHelper.Reorder(ref temp, i, j);

                        Assert.AreEqual(target, temp);
                    }
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_11111111_11111111);

                for (var i = 0; i < 32; i++)
                {
                    for (var j = 0; j < 32; j++)
                    {
                        var temp = target;
                        BitHelper.Reorder(ref temp, i, j);

                        Assert.AreEqual(target, temp);
                    }
                }
            }
            {
                const int target = 0b_00000000_00000000_00000000_00000001;

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.Reorder(ref temp, 0, i);

                    Assert.AreEqual(target << i, temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_10000000_00000000_00000000_00000000);

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.Reorder(ref temp, 31, i);

                    Assert.AreEqual((1 << i), temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_11111111_11111111_11111111_11111110);

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.Reorder(ref temp, 0, i);

                    Assert.AreEqual(~(1 << i), temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_01111111_11111111_11111111_11111111);

                for (var i = 0; i < 32; i++)
                {
                    var temp = target;
                    BitHelper.Reorder(ref temp, 31, i);

                    Assert.AreEqual(~(1 << i), temp);
                }
            }
            {
                var target = BitHelper.UInt32ToInt32(0b_00000000_11111111_00111100_01010101);
                {
                    var temp = target;

                    BitHelper.Reorder(ref temp, 2, 7);
                    Assert.AreEqual(0b_00000000_11111111_00111100_10101001, temp);

                    BitHelper.Reorder(ref temp, 7, 2);
                    Assert.AreEqual(target, temp);
                }
                {
                    var temp = target;

                    BitHelper.Reorder(ref temp, 7, 16);
                    Assert.AreEqual(0b_00000000_11111110_10011110_01010101, temp);

                    BitHelper.Reorder(ref temp, 16, 7);
                    Assert.AreEqual(target, temp);
                }
                {
                    var temp = target;

                    BitHelper.Reorder(ref temp, 16, 31);
                    Assert.AreEqual(BitHelper.UInt32ToInt32(0b_10000000_01111111_00111100_01010101), temp);

                    BitHelper.Reorder(ref temp, 31, 16);
                    Assert.AreEqual(target, temp);
                }
            }
        }
    }
}
