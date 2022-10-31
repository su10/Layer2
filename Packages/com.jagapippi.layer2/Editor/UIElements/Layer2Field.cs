#if UNITY_EDITOR
using System.Collections.Generic;
using Jagapippi.Layer2.Editor.Extensions;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    public class Layer2Field : PopupField<int>
    {
        public new static readonly string ussClassName = "unity-layer2-field";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        private static string OnFormatSelectedValue(int index)
        {
            return LayerSettingsSelection.activeSettings.LayerToName(index);
        }

        private static string OnFormatListItem(int index)
        {
            var layerName = LayerSettingsSelection.activeSettings.LayerToName(index);
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

                var settings = LayerSettingsSelection.activeSettings;

                for (var i = 0; i < Layer.MaxCount; ++i)
                {
                    var layerName = settings.LayerToName(i);
                    if (layerName.Length <= 0) continue;

                    this.choices.Add(i);
                }

                this.choices = this.choices;
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

        public override void SetValueWithoutNotify(int newValue)
        {
            if (this.choices.Contains(newValue) == false) return;
            base.SetValueWithoutNotify(newValue);
        }
    }
}
#endif
