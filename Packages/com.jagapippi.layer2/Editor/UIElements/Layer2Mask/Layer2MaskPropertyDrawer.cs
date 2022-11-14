#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    [CustomPropertyDrawer(typeof(Layer2Mask))]
    internal class Layer2MaskPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new Layer2MaskField(property.displayName);
        }
    }
}
#endif
