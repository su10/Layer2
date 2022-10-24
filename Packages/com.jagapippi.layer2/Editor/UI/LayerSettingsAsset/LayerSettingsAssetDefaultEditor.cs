#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
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

            var layerSettingsAsset = this.target as LayerSettingsAsset;

            var container = Assets.CreateContainer();

            container.Q<Foldout>("list-view-foldout").contentContainer.Add(ListViewDrawer.CreateGUI(this.serializedObject));
            container.Q<Foldout>("matrix-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject, PhysicsDimensions.Three));
            container.Q<Foldout>("matrix2d-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject, PhysicsDimensions.Two));

            // Copy ProjectSettings Button
            {
                var button = container.Q<Button>("copy-project-settings-button");
                button.clicked += this.CopyProjectSettingsAndSave;
            }
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
                        button.SetEnabled((ILayerSettings)layerSettingsAsset != LayerSettingsSelection.current);
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

        private void CopyProjectSettingsAndSave()
        {
            var layersProperty = this.serializedObject.FindLayersProperty();

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerProperty = layersProperty.ElementAt(i);
                layerProperty.FindNameProperty().stringValue = LayerMask.LayerToName(i);

                var collisionMatrixProperty = layerProperty.FindCollisionMatrixProperty();
                var collisionMatrix = collisionMatrixProperty.intValue;
                var collisionMatrix2DProperty = layerProperty.FindCollisionMatrix2DProperty();
                var collisionMatrix2D = collisionMatrix2DProperty.intValue;

                for (var j = 0; j < Layer.MaxCount; j++)
                {
                    {
                        var enable = (Physics.GetIgnoreLayerCollision(i, j) == false);
                        BitHelper.SetBit(ref collisionMatrix, j, enable);
                        collisionMatrixProperty.intValue = collisionMatrix;
                    }
                    {
                        var enable = (Physics2D.GetIgnoreLayerCollision(i, j) == false);
                        BitHelper.SetBit(ref collisionMatrix2D, j, enable);
                        collisionMatrix2DProperty.intValue = collisionMatrix2D;
                    }
                }
            }

            this.serializedObject.ApplyModifiedProperties();

            AssetDatabase.SaveAssetIfDirty(this.target);
        }
    }
}
#endif
