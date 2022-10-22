#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    [CustomEditor(typeof(LayerSettingsAsset), true, isFallback = true)]
    public class LayerSettingsAssetDefaultEditor : UnityEditor.Editor
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

        private static VisualElement CreateScriptReadonlyField(SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    var serializedProperty = iterator.Copy();

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    {
                        var propertyField = new VisualElement { name = $"PropertyField:{serializedProperty.propertyPath}" };
                        var objectField = new ObjectField("Script") { name = "unity-input-m_Script" };
                        objectField.BindProperty(serializedProperty);

                        objectField.focusable = false;
                        propertyField.Add(objectField);
                        propertyField.Q(null, "unity-object-field__selector")?.SetEnabled(false);
                        propertyField.Q(null, "unity-base-field__label")?.AddToClassList("unity-disabled");
                        propertyField.Q(null, "unity-base-field__input")?.AddToClassList("unity-disabled");

                        return propertyField;
                    }
                }
                while (iterator.NextVisible(false));
            }

            return null;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(CreateScriptReadonlyField(this.serializedObject));

            var layerSettingsAsset = this.target as LayerSettingsAsset;

            var container = Assets.CreateContainer();
            container.Q<Foldout>("list-view-foldout").contentContainer.Add(ListViewDrawer.CreateGUI(this.serializedObject));
            container.Q<Foldout>("matrix-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject));

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
            // Select Button
            {
                var button = container.Q<Button>("select-button");
                button.clicked += () => LayerSettingsSelection.Select(layerSettingsAsset);

                // Handle Enable
                {
                    EditorApplication.update += OnEditorUpdate;
                    button.RegisterCallback<DetachFromPanelEvent, EditorApplication.CallbackFunction>(
                        (_, callback) => EditorApplication.update -= callback,
                        OnEditorUpdate
                    );

                    void OnEditorUpdate()
                    {
                        button.SetEnabled(LayerSettingsSelection.current != (ILayerSettings)layerSettingsAsset);
                    }
                }
            }
            // Apply Button
            {
                var button = container.Q<Button>("apply-button");
                button.clicked += () =>
                {
                    LayerSettingsSelection.Apply(layerSettingsAsset);

                    UnityEditorInternal.ProjectSettingsWindow.Repaint();
                };
            }

            root.Add(container);

            return root;
        }
    }
}
#endif
