#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(CustomEditorHelper.CreateScriptReadonlyField(this.serializedObject));

            var layerSettingsAsset = this.target as LayerSettingsAsset;

            var container = Assets.CreateContainer();

            // Physics Dimensions
            {
                const string fieldName = nameof(layerSettingsAsset._physicsDimensions);
                var enumField = new EnumField(ObjectNames.NicifyVariableName(fieldName));
                enumField.BindProperty(this.serializedObject.FindProperty(fieldName));

                container.Q("physics-dimensions").Add(enumField);
            }

            container.Q<Foldout>("list-view-foldout").contentContainer.Add(ListViewDrawer.CreateGUI(this.serializedObject));
            container.Q<Foldout>("matrix-view-foldout").contentContainer.Add(MatrixViewDrawer.CreateGUI(this.serializedObject));

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
                button.clicked += () =>
                {
                    switch (layerSettingsAsset.physicsDimensions)
                    {
                        case PhysicsDimensions.Three:
                        {
                            LayerSettingsSelection.Select(layerSettingsAsset);
                            break;
                        }
                        case PhysicsDimensions.Two:
                        {
                            LayerSettingsSelection2D.Select(layerSettingsAsset);
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException();
                    }
                };

                // Handle Enable
                {
                    EditorApplication.update += OnEditorUpdate;
                    button.RegisterCallback<DetachFromPanelEvent, EditorApplication.CallbackFunction>(
                        (_, callback) => EditorApplication.update -= callback,
                        OnEditorUpdate
                    );

                    void OnEditorUpdate()
                    {
                        switch (layerSettingsAsset.physicsDimensions)
                        {
                            case PhysicsDimensions.Three:
                            {
                                button.SetEnabled(LayerSettingsSelection.current != (ILayerSettings)layerSettingsAsset);
                                break;
                            }
                            case PhysicsDimensions.Two:
                            {
                                button.SetEnabled(LayerSettingsSelection2D.current != (ILayerSettings)layerSettingsAsset);
                                break;
                            }
                            default: throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            // Apply Button
            {
                var button = container.Q<Button>("apply-button");
                button.clicked += () =>
                {
                    switch (layerSettingsAsset.physicsDimensions)
                    {
                        case PhysicsDimensions.Three:
                        {
                            LayerSettingsSelection.Apply(layerSettingsAsset);
                            break;
                        }
                        case PhysicsDimensions.Two:
                        {
                            LayerSettingsSelection2D.Apply(layerSettingsAsset);
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException();
                    }

                    UnityEditorInternal.ProjectSettingsWindow.Repaint();
                };
            }

            root.Add(container);

            return root;
        }

        private void CopyProjectSettingsAndSave()
        {
            var layersProperty = this.serializedObject.FindProperty(nameof(LayerSettingsAsset._layers));

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerProperty = layersProperty.ElementAt(i);
                layerProperty.FindNameProperty().stringValue = LayerMask.LayerToName(i);

                var collisionMatrixProperty = layerProperty.FindCollisionMatrixProperty();
                var collisionMatrix = collisionMatrixProperty.intValue;

                for (var j = 0; j < Layer.MaxCount; j++)
                {
                    var physicsDimensions = ((LayerSettingsAsset)this.target).physicsDimensions;
                    var enable = GetCollision(physicsDimensions, i, j);

                    BitHelper.SetBit(ref collisionMatrix, j, enable);
                    collisionMatrixProperty.intValue = collisionMatrix;
                }
            }

            this.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(this.target);
        }

        private static bool GetCollision(PhysicsDimensions physicsDimensions, int layer1, int layer2)
        {
            return physicsDimensions switch
            {
                PhysicsDimensions.Three => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false),
                PhysicsDimensions.Two => (Physics2D.GetIgnoreLayerCollision(layer1, layer2) == false),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
#endif
