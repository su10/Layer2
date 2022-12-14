#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Jagapippi.Layer2.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Jagapippi.Layer2.Editor.IMGUI
{
    internal static class Layer2Popup
    {
        private const string LabelText = "Layer2";
        private const int ListLength = Layer.MaxCount + 2;

        private static readonly ObjectPool<string[]> _pool = new(
            createFunc: () => new string[ListLength],
            actionOnRelease: target =>
            {
                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    target[i] = null;
                }
            },
            collectionCheck: true
        );

        public static int Draw(GameObject gameObject)
        {
            using (_pool.Get(out var displayedOptions))
            {
                var currentLayerSetting = LayerSettingSelection.activeSetting;
                currentLayerSetting.GetNamesWithIndexNonAlloc(displayedOptions);

                displayedOptions[^1] = $"Edit {currentLayerSetting.name} ...";

                using (SetLabelWidth(CalcLabelSize(LabelText).x))
                {
                    var selected = EditorGUILayout.Popup(LabelText, gameObject.layer, displayedOptions);
                    return (selected < Layer.MaxCount) ? selected : -1;
                }
            }
        }

        private static void GetNamesWithIndexNonAlloc(this ILayerSetting setting, IList<string> list)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerName = setting.LayerToName(i);

                if (string.IsNullOrEmpty(layerName) || string.IsNullOrWhiteSpace(layerName))
                {
                    list[i] = "";
                }
                else
                {
                    list[i] = $"{i}: {layerName}".ReplaceSpaceForPopup();
                }
            }
        }

        private static IDisposable SetLabelWidth(float width)
        {
            var old = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
            return Disposable.Create(() => EditorGUIUtility.labelWidth = old);
        }

        private static Vector2 CalcLabelSize(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text));
        }
    }
}
#endif
