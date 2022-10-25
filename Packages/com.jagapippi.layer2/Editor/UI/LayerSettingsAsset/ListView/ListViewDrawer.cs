#if UNITY_EDITOR
using System.Linq;
using Jagapippi.Layer2.Editor.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    internal static class ListViewDrawer
    {
        private static class Assets
        {
            private static readonly GUID RootUxmlGuid = new("f63e9a10887cc4c18b89c40a1f122f1b");

            private static VisualTreeAsset _visualTreeAsset;

            public static TemplateContainer CreateContainer()
            {
                if (_visualTreeAsset == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath(RootUxmlGuid);
                    _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                }

                return _visualTreeAsset.CloneTree();
            }
        }

        public static VisualElement CreateGUI(SerializedObject serializedObject)
        {
            var root = Assets.CreateContainer();
            root.BindProperty(serializedObject.FindLayersProperty());

            Construct(serializedObject, root);

            return root;
        }

        private static void Construct(SerializedObject serializedObject, VisualElement root)
        {
            var layersProperty = serializedObject.FindLayersProperty();

            // Ensure consistency of collisionMatrix value
            {
                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    // 3D
                    {
                        root.TrackPropertyValue(
                            layersProperty.ElementAt(i).FindCollisionMatrixProperty(),
                            UpdateCollisionMatrixProperties
                        );

                        void UpdateCollisionMatrixProperties(SerializedProperty changedCollisionMatrixProperty)
                        {
                            var changedLayerIndex = layersProperty.IndexOf(changedCollisionMatrixProperty.FindParentProperty());

                            for (var j = 0; j < Layer.MaxCount; j++)
                            {
                                if (j == changedLayerIndex) continue;

                                var layerProperty = layersProperty.ElementAt(j);
                                var collisionMatrixProperty = layerProperty.FindCollisionMatrixProperty();
                                var collisionMatrix = collisionMatrixProperty.intValue;

                                var enable = BitHelper.CheckBit(changedCollisionMatrixProperty.intValue, j);
                                BitHelper.SetBit(ref collisionMatrix, changedLayerIndex, enable);
                                collisionMatrixProperty.intValue = collisionMatrix;
                            }

                            layersProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                    }

                    // 2D
                    {
                        root.TrackPropertyValue(
                            layersProperty.ElementAt(i).FindCollisionMatrix2DProperty(),
                            UpdateCollisionMatrix2DProperties
                        );

                        void UpdateCollisionMatrix2DProperties(SerializedProperty changedCollisionMatrix2DProperty)
                        {
                            var changedLayerIndex = layersProperty.IndexOf(changedCollisionMatrix2DProperty.FindParentProperty());

                            for (var j = 0; j < Layer.MaxCount; j++)
                            {
                                if (j == changedLayerIndex) continue;

                                var layerProperty = layersProperty.ElementAt(j);
                                var collisionMatrix2DProperty = layerProperty.FindCollisionMatrix2DProperty();
                                var collisionMatrix2D = collisionMatrix2DProperty.intValue;

                                var enable = BitHelper.CheckBit(changedCollisionMatrix2DProperty.intValue, j);
                                BitHelper.SetBit(ref collisionMatrix2D, changedLayerIndex, enable);
                                collisionMatrix2DProperty.intValue = collisionMatrix2D;
                            }

                            layersProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                    }
                }
            }

            // Fixed layers
            {
                var fixedLayers = root.Q<FixedLayers>();
                var items = fixedLayers.listViewItems;

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    item.textField.label = GetLayerLabel(i);
                }
            }

            // Reorderable layers
            {
                var listView = root.Q<ListView>("layer-list-view");
                listView.itemsSource = layersProperty.ToEnumerable().Skip(FixedLayers.Count).ToList();
                listView.makeItem = () => new ListViewItem();

                var choices = Enumerable.Repeat("", Layer.MaxCount).ToList();

                UpdateCollisionMatrixChoices(applyToAllMaskFields: false);

                void UpdateCollisionMatrixChoices(bool applyToAllMaskFields)
                {
                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var layerProperty = layersProperty.ElementAt(i);
                        var layerName = layerProperty.FindNameProperty().stringValue;
                        choices[i] = $"{i}: {layerName}".ReplaceSpaceForPopup();
                    }

                    if (applyToAllMaskFields)
                    {
                        root.Query<MaskField>().ForEach(maskField => maskField.choices = choices);
                    }
                }

                listView.bindItem = (element, i) =>
                {
                    var shiftedIndex = FixedLayers.Count + i;
                    var layerProperty = layersProperty.ElementAt(shiftedIndex);
                    ((BindableElement)element).BindProperty(layerProperty);

                    var textField = element.Q<TextField>();
                    textField.label = GetLayerLabel(shiftedIndex);
                    textField.RegisterValueChangedCallback(_ => UpdateCollisionMatrixChoices(applyToAllMaskFields: true));

                    var maskField = element.Q<MaskField>();
                    maskField.choices = choices;
                };

                listView.itemIndexChanged += (src, dest) =>
                {
                    src += FixedLayers.Count;
                    dest += FixedLayers.Count;

                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var layerProperty = layersProperty.ElementAt(i);
                        {
                            var collisionMatrixProperty = layerProperty.FindCollisionMatrixProperty();
                            var collisionMatrix = collisionMatrixProperty.intValue;

                            BitHelper.Reorder(ref collisionMatrix, src, dest);
                            collisionMatrixProperty.intValue = collisionMatrix;
                        }
                        {
                            var collisionMatrix2DProperty = layerProperty.FindCollisionMatrix2DProperty();
                            var collisionMatrix2D = collisionMatrix2DProperty.intValue;

                            BitHelper.Reorder(ref collisionMatrix2D, src, dest);
                            collisionMatrix2DProperty.intValue = collisionMatrix2D;
                        }
                    }

                    layersProperty.MoveArrayElement(src, dest);

                    serializedObject.ApplyModifiedProperties();

                    UpdateCollisionMatrixChoices(applyToAllMaskFields: true);
                };

                UpdateCollisionMatrixChoices(applyToAllMaskFields: true);
            }
        }

        private static string GetLayerLabel(int index)
        {
            return BuiltinLayer.indexes.Contains(index)
                ? $"Builtin Layer {index}"
                : $"User Layer {index}";
        }
    }
}
#endif
