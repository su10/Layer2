#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    [CustomEditor(typeof(LayerSettingsAsset), true, isFallback = true)]
    public class LayerSettingsAssetDefaultEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, this.serializedObject, this);
            return container;
        }
    }
}
#endif
