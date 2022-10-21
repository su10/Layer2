#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    public class CollisionMatrixRowToggles : BindableElement, INotifyValueChanged<int>
    {
        public new class UxmlFactory : UxmlFactory<CollisionMatrixRowToggles, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlIntAttributeDescription _toggleCount = new()
            {
                name = "toggle-count",
                defaultValue = Layer.MaxCount,
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var rowToggles = ve as CollisionMatrixRowToggles;
                rowToggles.toggleCount = _toggleCount.GetValueFromBag(bag, cc);
            }
        }

        public const string UssClassName = "collision-matrix-row-toggles";
        public const string ToggleUssClassName = UssClassName + "__toggle";

        private int _value = -1;

        private readonly Toggle[] _toggles = new Toggle[Layer.MaxCount];

        public int toggleCount
        {
            get => this.childCount;
            set
            {
                var old = this.toggleCount;
                value = Mathf.Clamp(value, 1, Layer.MaxCount);

                if (old == value) return;

                if (old < value)
                {
                    var countToAdd = (value - old);

                    for (var i = 0; i < countToAdd; i++)
                    {
                        var toggle = new Toggle { value = true };
                        toggle.AddToClassList(ToggleUssClassName);

                        var _i = i;
                        toggle.RegisterValueChangedCallback(e => OnToggleValueChanged(e, old + _i));

                        this.Add(toggle);
                        _toggles[old + i] = toggle;
                    }
                }
                else
                {
                    while (value < this.childCount)
                    {
                        var i = this.childCount - 1;
                        _toggles[i] = null;
                        this.RemoveAt(i);
                    }
                }
            }
        }

        private int offset => (Layer.MaxCount - this.toggleCount);

        public CollisionMatrixRowToggles()
        {
            this.AddToClassList(UssClassName);
        }

        private void OnToggleValueChanged(ChangeEvent<bool> e, int index)
        {
            var newValue = this.value;
            BitHelper.SetBit(ref newValue, index + this.offset, e.newValue);
            this.value = newValue;
        }

        public void ToggleVisible(int layerIndex, bool visible)
        {
            if (this.offset <= layerIndex)
            {
                var style = _toggles[layerIndex - this.offset].style;
                style.display = (visible ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }

        #region INotifyValueChanged

        public int value
        {
            get => _value;
            set
            {
                if (this.value == value) return;

                var old = this.value;
                this.SetValueWithoutNotify(value);

                using (var e = ChangeEvent<int>.GetPooled(old, value))
                {
                    e.target = this;
                    this.SendEvent(e);
                }
            }
        }

        public void SetValueWithoutNotify(int newValue)
        {
            _value = newValue;

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var toggle = _toggles[i];
                if (toggle == null) break;

                toggle.SetValueWithoutNotify(BitHelper.CheckBit(_value, i + this.offset));
            }
        }

        #endregion
    }
}
#endif
