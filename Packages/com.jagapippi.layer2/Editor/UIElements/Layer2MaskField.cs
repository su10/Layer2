using System.Collections.Generic;
using System.Reflection;
using Jagapippi.Layer2.Editor.Extensions;
using UnityEditor.UIElements;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    public class Layer2MaskField : MaskField
    {
        public new class UxmlFactory : UxmlFactory<Layer2MaskField, UxmlTraits>
        {
        }

        public new class UxmlTraits : MaskField.UxmlTraits
        {
            private static readonly FieldInfo PropertyPathFieldInfo = typeof(BindableElement.UxmlTraits).GetField("m_PropertyPath", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo LabelFieldInfo = typeof(BaseField<int>.UxmlTraits).GetField("m_Label", BindingFlags.Instance | BindingFlags.NonPublic);

            public UxmlTraits()
            {
                var propertyPath = (UxmlStringAttributeDescription)PropertyPathFieldInfo.GetValue(this);
                propertyPath.defaultValue = nameof(Layer2Mask._value);

                var label = (UxmlStringAttributeDescription)LabelFieldInfo.GetValue(this);
                label.defaultValue = nameof(Layer2Mask);
            }
        }

        public new static readonly string ussClassName = "unity-layer2mask-field";
        public new static readonly string labelUssClassName = MaskField.ussClassName + "__label";
        public new static readonly string inputUssClassName = MaskField.ussClassName + "__input";

        private static readonly ObjectPool<List<string>> _choicesPool = new(
            createFunc: () => new List<string>(Layer.MaxCount),
            actionOnRelease: target => target.Clear(),
            collectionCheck: true
        );

        private static string OnFormatSelectedValue(string layerName)
        {
            return layerName;
        }

        private static string OnFormatListItem(string layerName)
        {
            var index = LayerSettingsSelection.activeSettings.NameToLayer(layerName);
            return (index < 0) ? null : $"{index}: {layerName}".ReplaceSpaceForPopup();
        }

        public Layer2MaskField() : this(null)
        {
        }

        public Layer2MaskField(int defaultMask) : this(null, defaultMask)
        {
        }

        public Layer2MaskField(string label, int defaultMask = 0)
            : base(
                label,
                new List<string>(),
                defaultMask,
                OnFormatSelectedValue,
                OnFormatListItem
            )
        {
            this.bindingPath = nameof(Layer2Mask._value);

            // Apply Classes
            {
                this.AddToClassList(ussClassName);
                this.labelElement.AddToClassList(labelUssClassName);
                this.Q(null, MaskField.inputUssClassName).AddToClassList(inputUssClassName);
            }

            UpdateChoices();

            void UpdateChoices()
            {
                using (_choicesPool.Get(out var choices))
                {
                    var settings = LayerSettingsSelection.activeSettings;

                    for (var i = 0; i < Layer.MaxCount; ++i)
                    {
                        var layerName = settings.LayerToName(i);
                        if (layerName.Length <= 0) continue;

                        choices.Add(layerName);
                    }

                    this.choices = choices;
                }
            }

            // Sync Choices
            {
                LayerSettingsSelection.changed += OnSettingsChanged;
                LayerSettingsSelection.activeSettings.changedSerializedObject += OnCurrentSettingsChanged;

                void OnSettingsChanged(ILayerSettings oldSettings, ILayerSettings newSettings)
                {
                    UpdateChoices();

                    if (oldSettings != null) oldSettings.changedSerializedObject -= OnCurrentSettingsChanged;
                    if (newSettings != null) newSettings.changedSerializedObject += OnCurrentSettingsChanged;
                }

                void OnCurrentSettingsChanged(ILayerSettings settings) => UpdateChoices();

                this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

                void OnDetachFromPanel(DetachFromPanelEvent _)
                {
                    LayerSettingsSelection.changed -= OnSettingsChanged;
                    LayerSettingsSelection.activeSettings.changedSerializedObject -= OnCurrentSettingsChanged;
                }
            }
        }
    }
}
