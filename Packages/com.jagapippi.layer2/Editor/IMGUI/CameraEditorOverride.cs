#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Jagapippi.Layer2.Editor.IMGUI
{
    [CustomEditor(typeof(Camera), true)]
    internal class CameraEditorOverride : UnityEditor.Editor
    {
        private static string MaskFieldLabel;

        private static readonly ObjectPool<string[]> _pool = new(
            createFunc: () => new string[Layer.MaxCount],
            actionOnRelease: target =>
            {
                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    target[i] = null;
                }
            },
            collectionCheck: true
        );

        void OnEnable()
        {
            MaskFieldLabel ??= $"{ObjectNames.NicifyVariableName(nameof(Camera.cullingMask))} (Layer2)";
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            var property = this.serializedObject.FindProperty("m_CullingMask");

            EditorGUI.BeginChangeCheck();

            using (_pool.Get(out var displayedOptions))
            {
                LayerSettingSelection.activeSetting.GetNamesNonAlloc(displayedOptions);

                var cullingMask = EditorGUILayout.MaskField(MaskFieldLabel, property.intValue, displayedOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = cullingMask;

                    this.serializedObject.ApplyModifiedProperties();
                }
            }

            base.OnInspectorGUI();
        }
    }
}
#endif
