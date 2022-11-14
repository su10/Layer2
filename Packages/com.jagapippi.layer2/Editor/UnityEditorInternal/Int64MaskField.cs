using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public class Int64MaskField : BaseMaskField<long>
    {
        public new static readonly string ussClassName = "unity-mask-field";
        public new static readonly string labelUssClassName = MaskField.ussClassName + "__label";
        public new static readonly string inputUssClassName = MaskField.ussClassName + "__input";

        private static int Int64ToInt32(long @long)
        {
            var @uint = (uint)@long;
            return UnsafeUtility.As<uint, int>(ref @uint);
        }

        private static uint Int32ToUInt32(int @int)
        {
            return UnsafeUtility.As<int, uint>(ref @int);
        }

        protected Int64MaskField(
            string label,
            int defaultMask,
            Func<string, string> formatSelectedValueCallback,
            Func<string, string> formatListItemCallback
        )
            : base(label)
        {
            this.m_FormatListItemCallback = formatListItemCallback;
            this.m_FormatSelectedValueCallback = formatSelectedValueCallback;
            this.SetValueWithoutNotify(Int32ToUInt32(defaultMask));
            this.textElement.text = this.GetValueToDisplay();

            this.AddToClassList(MaskField.ussClassName);
            this.labelElement.AddToClassList(MaskField.labelUssClassName);
            this.Q(null, "unity-base-popup-field__input").AddToClassList(MaskField.inputUssClassName);
        }

        internal override long MaskToValue(int newMask) => Int32ToUInt32(newMask);
        internal override int ValueToMask(long value) => Int64ToInt32(value);

        internal override string GetListItemToDisplay(long item)
        {
            var itemIndex = Int64ToInt32(item);
            var listItemToDisplay = this.GetDisplayedValue(itemIndex);

            if (ShouldFormatListItem(itemIndex))
            {
                listItemToDisplay = this.m_FormatListItemCallback(listItemToDisplay);
            }

            return listItemToDisplay;

            bool ShouldFormatListItem(int index)
            {
                if (index is 0 or -1) return false;
                return (this.m_FormatListItemCallback != null);
            }
        }

        internal override string GetValueToDisplay()
        {
            var itemIndex = Int64ToInt32(this.value);
            var valueToDisplay = this.GetDisplayedValue(itemIndex);

            if (ShouldFormatSelectedValue())
            {
                valueToDisplay = this.m_FormatSelectedValueCallback(valueToDisplay);
            }

            return valueToDisplay;

            bool ShouldFormatSelectedValue()
            {
                if (this.value is 0 or uint.MaxValue) return false;
                if (this.m_FormatSelectedValueCallback == null) return false;

                return Mathf.IsPowerOfTwo(itemIndex);
            }
        }
    }
}
