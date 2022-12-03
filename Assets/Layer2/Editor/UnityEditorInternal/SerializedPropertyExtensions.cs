#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class SerializedPropertyExtensions
    {
        public static uint GetLayerMaskBits(SerializedProperty property) => property.layerMaskBits;
        public static Gradient GetGradientValue(SerializedProperty property) => property.gradientValue;
    }
}
#endif
