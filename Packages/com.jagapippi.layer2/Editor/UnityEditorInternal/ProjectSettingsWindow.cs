#if UNITY_EDITOR
using UnityEditor;
using InternalProjectSettingsWindow = UnityEditor.ProjectSettingsWindow;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class ProjectSettingsWindow
    {
        public static void Repaint()
        {
            if (HasOpenInstances())
            {
                var window = EditorWindow.GetWindow<InternalProjectSettingsWindow>();
                window.Repaint();
            }
        }

        private static bool HasOpenInstances()
        {
            return EditorWindow.HasOpenInstances<InternalProjectSettingsWindow>();
        }
    }
}
#endif
