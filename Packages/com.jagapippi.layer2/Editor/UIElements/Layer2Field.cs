#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using Jagapippi.Layer2.Editor.Extensions;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    public class Layer2Field : PopupField<int>
    {
        public new class UxmlFactory : UxmlFactory<Layer2Field, UxmlTraits>
        {
        }

        public new class UxmlTraits : BaseField<int>.UxmlTraits
        {
            private static readonly FieldInfo PropertyPathFieldInfo = typeof(BindableElement.UxmlTraits).GetField("m_PropertyPath", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly FieldInfo LabelFieldInfo = typeof(BaseField<int>.UxmlTraits).GetField("m_Label", BindingFlags.Instance | BindingFlags.NonPublic);
            private readonly UxmlIntAttributeDescription _value = new() { name = "value" };

            public UxmlTraits()
            {
                var propertyPath = (UxmlStringAttributeDescription)PropertyPathFieldInfo.GetValue(this);
                propertyPath.defaultValue = nameof(Layer2Layer._value);

                var label = (UxmlStringAttributeDescription)LabelFieldInfo.GetValue(this);
                label.defaultValue = nameof(Layer2);
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((BaseField<int>)ve).value = _value.GetValueFromBag(bag, cc);
            }
        }

        public new static readonly string ussClassName = "unity-layer2-field";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        private static string OnFormatSelectedValue(int index)
        {
            return LayerSettingSelection.activeSetting.LayerToName(index);
        }

        private static string OnFormatListItem(int index)
        {
            var layerName = LayerSettingSelection.activeSetting.LayerToName(index);
            return $"{index}: {layerName}".ReplaceSpaceForPopup();
        }

        public override int value
        {
            get => base.value;
            set
            {
                if (this.choices.Contains(value) == false) return;
                base.value = value;
            }
        }

        public Layer2Field() : this(null)
        {
        }

        public Layer2Field(int defaultValue) : this(null, defaultValue)
        {
        }

        public Layer2Field(string label, int defaultValue = 0) : base(label, new List<int>(), defaultValue, OnFormatSelectedValue, OnFormatListItem)
        {
            this.bindingPath = nameof(Layer2Layer._value);

            // Apply Classes
            {
                var visualInput = this.Q(null, BaseField<int>.inputUssClassName);

                this.AddToClassList(LayerField.ussClassName);
                this.labelElement.AddToClassList(LayerField.labelUssClassName);
                visualInput.AddToClassList(LayerField.inputUssClassName);

                this.AddToClassList(ussClassName);
                this.labelElement.AddToClassList(labelUssClassName);
                visualInput.AddToClassList(inputUssClassName);
            }

            UpdateChoices();

            void UpdateChoices()
            {
                this.choices.Clear();

                var setting = LayerSettingSelection.activeSetting;

                for (var i = 0; i < Layer.MaxCount; ++i)
                {
                    var layerName = setting.LayerToName(i);
                    if (layerName.Length <= 0) continue;

                    this.choices.Add(i);
                }

                this.choices = this.choices;
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

        public override void SetValueWithoutNotify(int newValue)
        {
            if (this.choices.Contains(newValue) == false) return;
            base.SetValueWithoutNotify(newValue);
        }
    }
}
#endif
