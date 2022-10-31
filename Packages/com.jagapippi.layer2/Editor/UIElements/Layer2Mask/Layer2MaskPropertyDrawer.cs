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
            var root = new VisualElement();

            var layer2Mask = new Layer2MaskField(property.displayName);
            root.Add(layer2Mask);

            return root;
        }
    }
}
#endif
