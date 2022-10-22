#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jagapippi.Layer2.Editor
{
    [InitializeOnLoad]
    internal static class InspectorHeaderGUI
    {
        static InspectorHeaderGUI()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= OnGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI(UnityEditor.Editor editor)
        {
            var gameObject = editor.target switch
            {
                GameObject go => go,
                Component component => component.gameObject,
                _ => null,
            };

            if (gameObject == null) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(40);

                EditorGUI.BeginChangeCheck();

                var selected = Layer2Popup.Draw(gameObject);

                if (EditorGUI.EndChangeCheck())
                {
                    if (-1 < selected)
                    {
                        gameObject.layer = selected;
                        EditorUtility.SetDirty(gameObject);
                    }
                    else
                    {
                        Object target = null;
                        var currentLayerSettings = Layer2Core.currentSettings;

                        switch (currentLayerSettings)
                        {
                            case UnityLayerSettings:
                            {
                                target = Unsupported.GetSerializedAssetInterfaceSingleton("TagManager");
                                break;
                            }
                            default:
                            {
                                if (currentLayerSettings is Object obj) target = obj;
                                break;
                            }
                        }

                        Selection.activeObject = target;
                    }
                }
            }
        }
    }
}
#endif
