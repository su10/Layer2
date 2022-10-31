#if UNITY_EDITOR
using System;
using Jagapippi.Layer2.Editor.Extensions;
using Jagapippi.Layer2.Editor.UnityEditorInternal;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2.Editor.IMGUI
{
    [InitializeOnLoad]
    internal class LayerSettingsAssetInspectorHeaderGUI
    {
        static LayerSettingsAssetInspectorHeaderGUI()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= OnGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI(UnityEditor.Editor editor)
        {
            if (editor.target is not LayerSettingsAsset asset) return;

            if (asset == null) return;

            var active = LayerSettingsSelection.activeSettings;
            var isActive = ((ILayerSettings)asset == active);

            using (new EditorGUILayout.HorizontalScope())
            {
                {
                    const string label = "Active Settings";
                    var width = EditorStyles.label.CalcSize(new GUIContent(label)).x + 5;
                    EditorGUILayout.LabelField(label, GUILayout.Width(width));
                }

                if (active is LayerSettingsAsset activeAsset)
                {
                    var picked = (LayerSettingsAsset)EditorGUILayout.ObjectField(
                        activeAsset,
                        typeof(LayerSettingsAsset),
                        false
                    );

                    if (picked != activeAsset)
                    {
                        LayerSettingsSelection.Select(picked);
                    }
                }
                else
                {
                    {
                        var label = (active == EmptyLayerSettings.instance) ? "(None)" : active.name;
                        var style = (active == EmptyLayerSettings.instance) ? EditorStyles.boldLabel : EditorStyles.label;
                        style.clipping = TextClipping.Overflow;
                        EditorGUILayout.LabelField(label, style, GUILayout.MaxWidth(0));
                    }
                    {
                        var oldRect = GUILayoutUtility.GetLastRect();

                        GUILayout.FlexibleSpace();

                        var newRect = GUILayoutUtility.GetLastRect();
                        var rect = new Rect(oldRect.x, oldRect.y, newRect.width + 5, EditorGUIUtility.singleLineHeight);

                        rect.x -= 4;
                        GUI.Box(rect, "");
                    }
                    {
                        _ = (LayerSettingsAsset)EditorGUILayout.ObjectField(
                            null,
                            typeof(LayerSettingsAsset),
                            false,
                            GUILayout.Width(20)
                        );

                        if (ObjectSelector.HasOpenInstances())
                        {
                            var picked = (LayerSettingsAsset)EditorGUIUtility.GetObjectPickerObject();

                            if ((ILayerSettings)picked != active)
                            {
                                LayerSettingsSelection.Select(picked);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.LabelField($"{asset.name}");

            var serializedObject = new SerializedObject(asset);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (CreateGUIScope((isActive == false) && (Application.isPlaying == false)))
                {
                    if (GUILayout.Button(new GUIContent("Select", "Set as active. Please use \"Apply\" in play mode.")))
                    {
                        LayerSettingsSelection.Select(asset);

                        ProjectSettingsWindow.Repaint();
                    }
                }

                const string tooltip = "Set as active and apply to Physics." +
                                       "\n\nWARNING:" +
                                       "\nThis operation will change files below in edit mode." +
                                       "\n- DynamicsManager.asset" +
                                       "\n- Physics2DSettings.asset" +
                                       "\n- TagManager.asset";

                if (GUILayout.Button(new GUIContent("Apply", tooltip)))
                {
                    LayerSettingsSelection.Apply(asset);

                    ProjectSettingsWindow.Repaint();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("Copy", "Convert to JSON and copy to clipboard.")))
                {
                    Debug.Log(EditorJsonUtility.ToJson(asset));
                    EditorGUIUtility.systemCopyBuffer = EditorJsonUtility.ToJson(asset);
                }

                if (GUILayout.Button(new GUIContent("Paste", "Load JSON from clipboard.")))
                {
                    try
                    {
                        Undo.RegisterCompleteObjectUndo(asset, $"Modified Properties in {asset.name}");

                        var originalName = asset.name;
                        {
                            var json = EditorGUIUtility.systemCopyBuffer;
                            EditorJsonUtility.FromJsonOverwrite(json, asset);
                        }
                        asset.name = originalName;

                        AssetDatabase.SaveAssetIfDirty(asset);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);

                        Undo.RevertAllInCurrentGroup();

                        GUIUtility.ExitGUI();
                    }
                }

                if (GUILayout.Button("Load Project Settings"))
                {
                    CopyProjectSettingsAndSave();
                }
            }

            void CopyProjectSettingsAndSave()
            {
                var layersProperty = serializedObject.FindLayersProperty();

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

                serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssetIfDirty(asset);
            }
        }

        private static void HorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private static IDisposable CreateLabelWidthScope(float width)
        {
            var old = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
            return Disposable.Create(() => EditorGUIUtility.labelWidth = old);
        }

        private static IDisposable CreateGUIScope(bool enabled)
        {
            var old = GUI.enabled;
            GUI.enabled = enabled;
            return Disposable.Create(() => GUI.enabled = old);
        }
    }
}
#endif
