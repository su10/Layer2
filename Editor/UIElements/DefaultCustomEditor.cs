#if UNITY_EDITOR && !UNITY_2022_2_OR_NEWER && !LAYER2_DISABLE_DEFAULT_CUSTOM_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    internal class DefaultCustomEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                    if (iterator.propertyPath == "m_Script")
                    {
#if !UNITY_2022_1_OR_NEWER
                        if (serializedObject.targetObject != null)
                        {
                            propertyField.RegisterCallback<AttachToPanelEvent>(_ =>
                            {
                                var objectField = propertyField.Q<ObjectField>();
                                var objectFieldDisplay = objectField.Q(null, "unity-object-field-display");

                                objectField.AddToClassList("unity-disabled");
                                objectFieldDisplay.RegisterCallback<KeyDownEvent>(e =>
                                    {
                                        if (e.keyCode == KeyCode.Space) e.PreventDefault();
                                    },
                                    TrickleDown.TrickleDown
                                );
                                objectFieldDisplay.RegisterCallback<DragPerformEvent>(e => e.PreventDefault(), TrickleDown.TrickleDown);
                                objectField.Q(null, ObjectField.selectorUssClassName).SetEnabled(false);
                            });
                        }
                        else
#endif
                        {
                            InspectorElement.FillDefaultInspector(container, this.serializedObject, this);
                            return container;
                        }
                    }

                    container.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }

            return container;
        }
    }
}
#endif
