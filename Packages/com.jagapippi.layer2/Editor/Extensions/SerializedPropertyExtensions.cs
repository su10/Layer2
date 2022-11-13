#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Jagapippi.Layer2.Editor.UnityEditorInternal;
using UnityEditor;

namespace Jagapippi.Layer2.Editor.Extensions
{
    internal static class SerializedPropertyExtensions
    {
        public static SerializedProperty ElementAt(this SerializedProperty self, int index)
        {
            return self.GetArrayElementAtIndex(index);
        }

        public static int IndexOf(this SerializedProperty self, SerializedProperty child)
        {
            for (var i = 0; i < self.arraySize; i++)
            {
                var prop = self.ElementAt(i);

                if (SerializedProperty.EqualContents(prop, child))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IEnumerable<SerializedProperty> ToEnumerable(this SerializedProperty self)
        {
            foreach (SerializedProperty property in self)
            {
                yield return property;
            }
        }

        public static List<SerializedProperty> ToList(this SerializedProperty self)
        {
            var list = new List<SerializedProperty>();

            for (var i = 0; i < self.arraySize; i++)
            {
                list.Add(self.ElementAt(i));
            }

            return list;
        }

        public static SerializedProperty FindNameProperty(this SerializedProperty layerProperty)
        {
            return layerProperty.FindPropertyRelative(nameof(SerializableLayer._name));
        }

        public static SerializedProperty FindIndexProperty(this SerializedProperty layerProperty)
        {
            return layerProperty.FindPropertyRelative(nameof(SerializableLayer._index));
        }

        public static SerializedProperty FindCollisionMatrixProperty(this SerializedProperty layerProperty)
        {
            return layerProperty.FindPropertyRelative(nameof(SerializableLayer._collisionMatrix));
        }

        public static SerializedProperty FindCollisionMatrix2DProperty(this SerializedProperty layerProperty)
        {
            return layerProperty.FindPropertyRelative(nameof(SerializableLayer._collisionMatrix2D));
        }

        public static SerializedProperty FindValueProperty(this SerializedProperty layer2MaskProperty)
        {
            return layer2MaskProperty.FindPropertyRelative(nameof(Layer2Mask.m_Bits));
        }

        public static SerializedProperty FindParentProperty(this SerializedProperty self)
        {
            var paths = self.propertyPath.Split(".");
            if (paths.Length == 1) return self;

            var i = 1;
            for (; i < paths.Length; i++)
            {
                if (Regex.IsMatch(paths[^i], @"^data\[[0-9]+\]$") == false)
                {
                    break;
                }
            }

            var path = string.Join(".", paths[..^i]);
            return self.serializedObject.FindProperty(path);
        }

        public static T GetValue<T>(this SerializedProperty self)
        {
            var paths = self.propertyPath.Split(".");
            var currentProperty = self;
            object obj = self.serializedObject.targetObject;

            for (var i = 0; i < paths.Length; i++)
            {
                var path = string.Join(".", paths[..(i + 1)]);
                var isArray = currentProperty.isArray;

                currentProperty = self.serializedObject.FindProperty(path);
                var fieldInfo = ScriptAttributeUtility.GetFieldInfoFromProperty(currentProperty, out _);

                if (fieldInfo == null) continue;

                if (isArray && fieldInfo.FieldType.IsArray)
                {
                    obj = ((IList)obj)[i];
                }
                else
                {
                    obj = currentProperty.propertyType switch
                    {
                        SerializedPropertyType.Generic => fieldInfo.GetValue(obj),
                        SerializedPropertyType.Integer => currentProperty.intValue,
                        SerializedPropertyType.Boolean => currentProperty.boolValue,
                        SerializedPropertyType.Float => currentProperty.floatValue,
                        SerializedPropertyType.String => currentProperty.stringValue,
                        SerializedPropertyType.Color => currentProperty.colorValue,
                        SerializedPropertyType.ObjectReference => currentProperty.objectReferenceValue,
                        SerializedPropertyType.LayerMask => UnityEditorInternal.SerializedPropertyExtensions.GetLayerMaskBits(currentProperty),
                        SerializedPropertyType.Enum => currentProperty.enumValueIndex,
                        SerializedPropertyType.Vector2 => currentProperty.vector2Value,
                        SerializedPropertyType.Vector3 => currentProperty.vector3Value,
                        SerializedPropertyType.Vector4 => currentProperty.vector4Value,
                        SerializedPropertyType.Rect => currentProperty.rectValue,
                        SerializedPropertyType.ArraySize => currentProperty.arraySize,
                        SerializedPropertyType.Character => (char)currentProperty.intValue,
                        SerializedPropertyType.AnimationCurve => currentProperty.animationCurveValue,
                        SerializedPropertyType.Bounds => currentProperty.boundsValue,
                        SerializedPropertyType.Gradient => UnityEditorInternal.SerializedPropertyExtensions.GetGradientValue(currentProperty),
                        SerializedPropertyType.Quaternion => currentProperty.quaternionValue,
                        SerializedPropertyType.ExposedReference => currentProperty.exposedReferenceValue,
                        SerializedPropertyType.FixedBufferSize => currentProperty.fixedBufferSize,
                        SerializedPropertyType.Vector2Int => currentProperty.vector2IntValue,
                        SerializedPropertyType.Vector3Int => currentProperty.vector3IntValue,
                        SerializedPropertyType.RectInt => currentProperty.rectIntValue,
                        SerializedPropertyType.BoundsInt => currentProperty.boundsIntValue,
                        SerializedPropertyType.ManagedReference => currentProperty.managedReferenceValue,
                        SerializedPropertyType.Hash128 => currentProperty.hash128Value,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                }
            }

            return (T)obj;
        }
    }
}
#endif
