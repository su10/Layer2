#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    internal class ListViewItem : BindableElement
    {
        public new class UxmlFactory : UxmlFactory<ListViewItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
        }

        public const string UssClassName = "list-view-item";

        public readonly TextField textField;
        public readonly MaskField maskField;

        public ListViewItem()
        {
            this.AddToClassList(UssClassName);

            this.textField = new TextField { bindingPath = nameof(SerializableLayer._name) };
            this.Add(this.textField);

            // TODO: 2D対応
            this.maskField = new MaskField { bindingPath = nameof(SerializableLayer._collisionMatrix) };
            this.Add(this.maskField);
        }
    }
}
#endif
