#if UNITY_EDITOR
using Jagapippi.Layer2.Editor.Extensions;
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

            var valueProperty = property.FindValueProperty();
            var layerNames = new string[Layer.MaxCount];

            var imguiContainer = new IMGUIContainer(OnGUI);
            imguiContainer.AddToClassList(BaseField<int>.ussClassName);
            imguiContainer.AddToClassList(BaseField<int>.ussClassName + "__inspector-field");

            root.Add(imguiContainer);

            void OnGUI()
            {
                property.serializedObject.Update();

                var activeSettings = LayerSettingsSelection.activeSettings;

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var layerName = activeSettings.LayerToName(i);

                    if (0 < layerName.Length)
                    {
                        layerNames[i] = $"{i}: {layerName}".ReplaceSpaceForPopup();
                    }
                    else
                    {
                        layerNames[i] = null;
                    }
                }

                EditorGUI.BeginChangeCheck();

                var maskField = EditorGUILayout.MaskField(
                    property.displayName,
                    valueProperty.intValue,
                    layerNames
                );

                if (EditorGUI.EndChangeCheck())
                {
                    valueProperty.longValue = BitHelper.Int32ToUInt32(maskField);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            return root;
        }
    }
}
#endif
