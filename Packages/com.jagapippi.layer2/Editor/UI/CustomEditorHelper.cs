#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    internal static class CustomEditorHelper
    {
        public static VisualElement CreateScriptReadonlyField(SerializedObject serializedObject)
        {
            var scriptProperty = serializedObject.FindProperty("m_Script");

            var objectField = new ObjectField("Script") { name = "unity-input-m_Script" };
            objectField.BindProperty(scriptProperty);
            objectField.focusable = false;

            var propertyField = new VisualElement { name = $"PropertyField:{scriptProperty.propertyPath}" };
            propertyField.Add(objectField);
            propertyField.Q(null, "unity-object-field__selector")?.SetEnabled(false);
            propertyField.Q(null, "unity-base-field__label")?.AddToClassList("unity-disabled");
            propertyField.Q(null, "unity-base-field__input")?.AddToClassList("unity-disabled");

            return propertyField;
        }
    }
}
#endif
