#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    [CustomPropertyDrawer(typeof(Layer2Layer))]
    internal class Layer2LayerPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var layer2Field = new Layer2Field(property.displayName);
            root.Add(layer2Field);

            return root;
        }
    }
}
#endif
