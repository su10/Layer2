#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    internal class FixedLayers : BindableElement
    {
        public new class UxmlFactory : UxmlFactory<FixedLayers, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
        }

        public const int Count = 6;
        public const string UssClassName = "fixed-layers-list";

        private readonly ListViewItem[] _listViewItems = new ListViewItem[Count];
        public IReadOnlyList<ListViewItem> listViewItems => _listViewItems;

        public FixedLayers()
        {
            this.AddToClassList(UssClassName);

            for (var i = 0; i < _listViewItems.Length; i++)
            {
                var listViewItem = new ListViewItem { bindingPath = $"Array.data[{i}]" };
                listViewItem.textField.SetEnabled(false);

                _listViewItems[i] = listViewItem;
                this.Add(listViewItem);
            }
        }

        public void SetChoices(List<string> choices)
        {
            foreach (var listViewItem in _listViewItems)
            {
                listViewItem.maskField.choices = choices;
            }
        }
    }
}
#endif
