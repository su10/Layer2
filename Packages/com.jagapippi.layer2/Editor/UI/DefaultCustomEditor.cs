#if UNITY_EDITOR && !UNITY_2022_2_OR_NEWER && !LAYER2_DISABLE_DEFAULT_CUSTOM_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
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
                    VisualElement propertyField;

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    {
                        propertyField = CustomEditorHelper.CreateScriptReadonlyField(this.serializedObject);
                    }
                    else
                    {
                        propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
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
