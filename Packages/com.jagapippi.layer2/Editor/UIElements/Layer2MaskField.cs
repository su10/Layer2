using System.Collections.Generic;
using System.Reflection;
using Jagapippi.Layer2.Editor.Extensions;
using Jagapippi.Layer2.Editor.UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    public class Layer2MaskField : Int64MaskField
    {
        public new class UxmlFactory : UxmlFactory<Layer2MaskField, UxmlTraits>
        {
        }

        public new class UxmlTraits : Int64MaskField.UxmlTraits
        {
            private static readonly FieldInfo PropertyPathFieldInfo = typeof(BindableElement.UxmlTraits).GetField("m_PropertyPath", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo LabelFieldInfo = typeof(Int64MaskField.UxmlTraits).GetField("m_Label", BindingFlags.Instance | BindingFlags.NonPublic);

            public UxmlTraits()
            {
                var propertyPath = (UxmlStringAttributeDescription)PropertyPathFieldInfo.GetValue(this);
                propertyPath.defaultValue = nameof(Layer2Mask.m_Bits);

                var label = (UxmlStringAttributeDescription)LabelFieldInfo.GetValue(this);
                label.defaultValue = nameof(Layer2Mask);
            }
        }

        public new static readonly string ussClassName = "unity-layer2-mask-field";
        public new static readonly string labelUssClassName = Layer2MaskField.ussClassName + "__label";
        public new static readonly string inputUssClassName = Layer2MaskField.ussClassName + "__input";

        private static string OnFormatSelectedValue(string layerName) => layerName;

        private static string OnFormatListItem(string layerName)
        {
            if (string.IsNullOrEmpty(layerName)) return null;

            var index = LayerSettingSelection.activeSetting.NameToLayer(layerName);
            return (0 <= index) ? $"{index}: {layerName}".ReplaceSpaceForPopup() : null;
        }

        private static readonly List<string> TempChoices = new();

        public Layer2MaskField() : this(null)
        {
        }

        public Layer2MaskField(int defaultMask) : this(null, defaultMask)
        {
        }

        public Layer2MaskField(string label, int defaultMask = 0) : base(label, defaultMask, OnFormatSelectedValue, OnFormatListItem)
        {
            this.bindingPath = nameof(Layer2Mask.m_Bits);

            {
                var labelElement = this.Q<VisualElement>(null, BasePopupField<long, string>.labelUssClassName);
                labelElement.style.minWidth = new StyleLength(120);

                var visualInput = this.Q<VisualElement>(null, BasePopupField<long, string>.inputUssClassName);

                {
                    this.AddToClassList(MaskField.ussClassName);
                    labelElement.AddToClassList(MaskField.labelUssClassName);
                    visualInput.AddToClassList(MaskField.inputUssClassName);
                }
                {
                    this.AddToClassList(LayerMaskField.ussClassName);
                    labelElement.AddToClassList(LayerMaskField.labelUssClassName);
                    visualInput.AddToClassList(LayerMaskField.inputUssClassName);
                }
                {
                    this.AddToClassList(Layer2MaskField.ussClassName);
                    labelElement.AddToClassList(Layer2MaskField.labelUssClassName);
                    visualInput.AddToClassList(Layer2MaskField.inputUssClassName);
                }
                {
                    labelElement.AddToClassList(PropertyField.labelUssClassName);
                    visualInput.AddToClassList(PropertyField.inputUssClassName);
                }
            }

            UpdateChoices();

            void UpdateChoices()
            {
                var setting = LayerSettingSelection.activeSetting;

                for (var i = 0; i < Layer.MaxCount; ++i)
                {
                    var layerName = setting.LayerToName(i);

                    TempChoices.Add(layerName);
                }

                this.choices = TempChoices;
            }

            // Sync Choices
            {
                LayerSettingSelection.changed += OnSettingChanged;
                LayerSettingSelection.activeSetting.changedSerializedObject += OnCurrentSettingChanged;

                void OnSettingChanged(ILayerSetting oldSetting, ILayerSetting newSetting)
                {
                    UpdateChoices();

                    if (oldSetting != null) oldSetting.changedSerializedObject -= OnCurrentSettingChanged;
                    if (newSetting != null) newSetting.changedSerializedObject += OnCurrentSettingChanged;
                }

                void OnCurrentSettingChanged(ILayerSetting setting) => UpdateChoices();

                this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

                void OnDetachFromPanel(DetachFromPanelEvent _)
                {
                    LayerSettingSelection.changed -= OnSettingChanged;
                    LayerSettingSelection.activeSetting.changedSerializedObject -= OnCurrentSettingChanged;
                }
            }
        }
    }
}
