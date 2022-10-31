#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    [CustomEditor(typeof(LayerSettingsAsset), true, isFallback = true)]
    internal class LayerSettingsAssetDefaultEditor : UnityEditor.Editor
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

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(CustomEditorHelper.CreateScriptReadonlyField(this.serializedObject));

            var container = Assets.CreateContainer();

            container.Q<Foldout>("list-view-foldout").contentContainer.Add(ListViewDrawer.CreateGUI(this.serializedObject));
            container.Q<Foldout>("matrix-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject, PhysicsDimensions.Three));
            container.Q<Foldout>("matrix2d-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject, PhysicsDimensions.Two));

            // Save Button
            {
                var button = container.Q<Button>("save-button");
                button.clicked += () => AssetDatabase.SaveAssetIfDirty(this.target);

                // Handle Enable
                {
                    EditorApplication.update += OnEditorUpdate;
                    button.RegisterCallback<DetachFromPanelEvent, EditorApplication.CallbackFunction>(
                        (_, callback) => EditorApplication.update -= callback,
                        OnEditorUpdate
                    );

                    void OnEditorUpdate() => button.SetEnabled(EditorUtility.IsDirty(this.target));
                }
            }

            root.Add(container);

            return root;
        }
    }
}
#endif
