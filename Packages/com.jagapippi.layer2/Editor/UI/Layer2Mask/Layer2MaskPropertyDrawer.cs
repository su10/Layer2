#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    [CustomPropertyDrawer(typeof(Layer2Mask), true)]
    internal class Layer2MaskPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var choices = Enumerable.Repeat("", Layer.MaxCount).ToList();
            var layer2Mask = (Layer2Mask)fieldInfo.GetValue(property.serializedObject.targetObject);

            var maskField = new MaskField(ObjectNames.NicifyVariableName(property.name), choices, layer2Mask);
            maskField.AddToClassList(LayerMaskField.ussClassName);
            maskField.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            maskField.BindProperty(property.FindValueProperty());
            root.Add(maskField);

            // Sync Choices
            {
                UpdateChoices(choices);

                void UpdateChoices(List<string> list)
                {
                    Layer2Core.currentSettings.GetNamesNonAlloc(list);
                    maskField.choices = list;
                }

                Layer2Core.settingsChanged += OnSettingsChanged;

                if (Layer2Core.currentSettings != null)
                {
                    Layer2Core.currentSettings.changed += OnCurrentSettingsChanged;
                }

                maskField.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

                void OnDetachFromPanel(DetachFromPanelEvent _)
                {
                    Layer2Core.settingsChanged -= OnSettingsChanged;

                    if (Layer2Core.currentSettings != null)
                    {
                        Layer2Core.currentSettings.changed -= OnCurrentSettingsChanged;
                    }
                }

                void OnSettingsChanged(ILayerSettings oldSettings, ILayerSettings newSettings)
                {
                    UpdateChoices(choices);

                    if (oldSettings != null) oldSettings.changed -= OnCurrentSettingsChanged;
                    if (newSettings != null) newSettings.changed += OnCurrentSettingsChanged;
                }

                void OnCurrentSettingsChanged(ILayerSettings settings)
                {
                    UpdateChoices(choices);
                }
            }

            var label = maskField.Q<Label>();
            label.AddToClassList(LayerMaskField.labelUssClassName);
            label.AddToClassList(PropertyField.labelUssClassName);

            return root;
        }
    }
}
#endif
