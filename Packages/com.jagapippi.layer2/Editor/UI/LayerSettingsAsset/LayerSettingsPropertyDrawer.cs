#if UNITY_EDITOR
using Jagapippi.Layer2.Editor.UnityEditorInternal;
using UnityEditor;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    [CustomPropertyDrawer(typeof(LayerSettings), true)]
    internal class LayerSettingsPropertyDrawer : PropertyDrawer
    {
        private static class Assets
        {
            private static readonly GUID UxmlGuid = new("f7052423f6629460ca3a22bca888d0f1");
            private static VisualTreeAsset _visualTreeAsset;

            public static TemplateContainer CreateContainer()
            {
                if (_visualTreeAsset == null)
                {
                    var uxmlPath = AssetDatabase.GUIDToAssetPath(UxmlGuid);
                    _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
                }

                return _visualTreeAsset.Instantiate();
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var layerSettings = (LayerSettings)fieldInfo.GetValue(property.serializedObject.targetObject);

            var root = Assets.CreateContainer();
            root.Q<Foldout>("list-view-foldout").contentContainer.Add(ListViewDrawer.CreateGUI(property));
            root.Q<Foldout>("matrix-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(property));
            root.Q<Button>("apply-button").clicked += () =>
            {
                layerSettings.ApplyCollisionMatrix();
                ProjectSettingsWindow.Repaint();
            };

            return root;
        }
    }
}
#endif
