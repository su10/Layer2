#if UNITY_EDITOR
using UnityEditor;

namespace Jagapippi.Layer2.Editor.Extensions
{
    internal static class SerializedObjectExtensions
    {
        public static SerializedProperty FindLayersProperty(this SerializedObject layerSettingObject)
        {
            return layerSettingObject.FindProperty(nameof(LayerSettingAsset._layers));
        }
    }
}
#endif
