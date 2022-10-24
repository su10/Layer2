#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor
{
    internal static class ListViewDrawer
    {
        private static class Assets
        {
            private static readonly GUID RootUxmlGuid = new("f63e9a10887cc4c18b89c40a1f122f1b");
            private static readonly GUID ListViewItemUxmlGuid = new("568dd86e806f406798bee9498fba13f1");

            private static VisualTreeAsset _visualTreeAsset;
            private static VisualTreeAsset _listViewItemAsset;

            public static TemplateContainer CreateContainer()
            {
                if (_visualTreeAsset == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath(RootUxmlGuid);
                    _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                }

                return _visualTreeAsset.CloneTree();
            }

            public static VisualElement CreateListViewItem()
            {
                if (_listViewItemAsset == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath(ListViewItemUxmlGuid);
                    _listViewItemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                }

                return _listViewItemAsset.CloneTree();
            }
        }

        public static VisualElement CreateGUI(SerializedObject serializedObject)
        {
            var root = Assets.CreateContainer();

            Construct(serializedObject, root);

            return root;
        }

        private static void Construct(SerializedObject serializedObject, VisualElement root)
        {
            var layersProperty = serializedObject.FindLayersProperty();
            var choices = Enumerable.Repeat("", Layer.MaxCount).ToList();

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerProperty = layersProperty.ElementAt(i);
                var layerName = layerProperty.FindNameProperty().stringValue;
                choices[i] = $"{i}: {layerName}";

                root.TrackPropertyValue(layerProperty.FindCollisionMatrixProperty(), UpdateCollisionMatrixProperties);

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

            var listView = root.Q<ListView>("layer-list-view");

            listView.makeItem = () => Assets.CreateListViewItem();

            listView.bindItem = (element, i) =>
            {
                var layerProperty = layersProperty.ElementAt(i);
                layerProperty.FindIndexProperty().intValue = i;

                var textField = element.Q<TextField>();
                textField.BindProperty(layerProperty.FindNameProperty());
                textField.label = GetLayerLabel(i);

                var maskField = element.Q<MaskField>();
                maskField.BindProperty(layerProperty.FindCollisionMatrixProperty());
                maskField.choices = choices;
            };

            listView.itemIndexChanged += (src, dest) =>
            {
                var container = listView.Q<VisualElement>("unity-content-container").Children().ToArray();

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var layerProperty = layersProperty.ElementAt(i);
                    layerProperty.FindIndexProperty().intValue = i;

                    var textField = container[i].Q<TextField>();
                    textField.BindProperty(layerProperty.FindNameProperty());
                    textField.label = GetLayerLabel(i);

                    var collisionMatrixProperty = layerProperty.FindCollisionMatrixProperty();
                    var collisionMatrix = collisionMatrixProperty.intValue;

                    BitHelper.Reorder(ref collisionMatrix, src, dest);
                    collisionMatrixProperty.intValue = collisionMatrix;
                }

                {
                    var movedItem = choices[src];
                    choices.RemoveAt(src);
                    choices.Insert(dest, movedItem);

                    var smallerIndex = Mathf.Min(src, dest);
                    var largerIndex = Mathf.Max(src, dest);

                    for (var i = smallerIndex; i < largerIndex + 1; i++)
                    {
                        var choice = choices[i];
                        var divided = choice.Split(':');
                        choices[i] = $"{i}:{string.Join(":", divided[1..])}";
                    }
                }

                serializedObject.ApplyModifiedProperties();
            };
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
