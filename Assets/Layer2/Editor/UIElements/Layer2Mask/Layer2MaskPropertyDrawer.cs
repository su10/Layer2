#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    [CustomPropertyDrawer(typeof(Layer2Mask))]
    internal class Layer2MaskPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var dummyField = new PropertyField(property.FindPropertyRelative(nameof(Layer2Mask.m_Bits)))
            {
                style = { height = 0 },
            };
            root.Add(dummyField);

            var field = new Layer2MaskField(property.displayName);
            root.Add(field);

            Label dummyLabel = null;
            Label label = null;

            field.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                dummyLabel ??= dummyField.Q<Label>();
                label ??= field.Q<Label>();

                label.style.minWidth = dummyLabel.style.minWidth;
                label.style.width = dummyLabel.style.width;
            });

            return root;
        }
    }
}
#endif
