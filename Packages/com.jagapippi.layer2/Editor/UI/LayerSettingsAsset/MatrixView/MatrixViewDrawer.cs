#if UNITY_EDITOR
using System;
using Jagapippi.Layer2.Editor.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    internal static class MatrixViewDrawer
    {
        private static class Assets
        {
            private static readonly GUID UxmlGuid = new("78f6b95442b44463eb75bb47c16e0fe3");

            private static VisualTreeAsset _visualTreeAsset;

            public static TemplateContainer CreateContainer()
            {
                if (_visualTreeAsset == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath(UxmlGuid);
                    _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                }

                return _visualTreeAsset.Instantiate();
            }
        }

        public static VisualElement CreateGUI(SerializedObject serializedObject, PhysicsDimensions physicsDimensions)
        {
            var root = Assets.CreateContainer();
            root.BindProperty(serializedObject);
            Construct(serializedObject, root, physicsDimensions);

            return root;
        }

        private static void Construct(SerializedObject serializedObject, VisualElement root, PhysicsDimensions physicsDimensions)
        {
            var layersProperty = serializedObject.FindLayersProperty();
            var collisionMatrixVisualElement = root.Q<CollisionMatrix>();
            collisionMatrixVisualElement.physicsDimensions = physicsDimensions;

            // Labels
            {
                var leftLabelsContainer = root.Q("left-labels");
                var topLabelsContainer = root.Q("top-labels");

                var leftLabels = leftLabelsContainer.Query<Label>().ToList();
                var topLabels = topLabelsContainer.Query<Label>().ToList();

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var leftLabel = leftLabels[i];
                    var topLabel = topLabels[i];

                    {
                        var bindingPath = layersProperty.ElementAt(i).FindNameProperty().propertyPath;
                        leftLabel.bindingPath = bindingPath;
                        topLabel.bindingPath = bindingPath;
                    }
                    {
                        leftLabel.RegisterValueChangedCallback(e => UpdateLabelVisible(e, leftLabel));
                        topLabel.RegisterValueChangedCallback(e => UpdateLabelVisible(e, topLabel));

                        void UpdateLabelVisible(ChangeEvent<string> e, VisualElement element)
                        {
                            var display = (IsVisibleLayerName(e.newValue) ? DisplayStyle.Flex : DisplayStyle.None);
                            element.style.display = display;
                        }
                    }
                    {
                        leftLabelsContainer.RegisterCallback<GeometryChangedEvent>(AdjustHeight);

                        void AdjustHeight(GeometryChangedEvent e)
                        {
                            topLabelsContainer.style.height = new StyleLength(leftLabelsContainer.contentRect.width);
                        }
                    }
                }
            }
            // Toggles
            {
                collisionMatrixVisualElement.BindProperty(layersProperty);

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    UpdateAllTogglesVisible();

                    var layerProperty = layersProperty.ElementAt(i);
                    root.TrackPropertyValue(layerProperty.FindNameProperty(), _ => UpdateAllTogglesVisible());
                }

                void UpdateAllTogglesVisible()
                {
                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var layerProperty = layersProperty.ElementAt(i);
                        var row = collisionMatrixVisualElement.rows[i];
                        var rowVisible = IsVisibleLayerName(layerProperty.FindNameProperty().stringValue);

                        if (rowVisible == false)
                        {
                            for (var j = 0; j < Layer.MaxCount; j++)
                            {
                                row.ToggleVisible(j, false);
                            }
                        }
                        else
                        {
                            for (var j = 0; j < Layer.MaxCount; j++)
                            {
                                var otherLayerProperty = layersProperty.ElementAt(j);
                                var toggleVisible = IsVisibleLayerName(otherLayerProperty.FindNameProperty().stringValue);
                                row.ToggleVisible(j, toggleVisible);
                            }
                        }
                    }
                }
            }
            // Buttons
            {
                var disableAllButton = root.Q<Button>("disable-all-button");
                var enableAllButton = root.Q<Button>("enable-all-button");

                disableAllButton.clicked += () => SetAllToggles(false);
                enableAllButton.clicked += () => SetAllToggles(true);

                void SetAllToggles(bool value)
                {
                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var layerProperty = layersProperty.ElementAt(i);
                        var matrixProperty = physicsDimensions switch
                        {
                            PhysicsDimensions.Three => layerProperty.FindCollisionMatrixProperty(),
                            PhysicsDimensions.Two => layerProperty.FindCollisionMatrix2DProperty(),
                            _ => throw new ArgumentOutOfRangeException(),
                        };
                        matrixProperty.intValue = (value ? -1 : 0);
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private static bool IsVisibleLayerName(string layerName)
        {
            return (string.IsNullOrEmpty(layerName) == false);
        }
    }
}
#endif
