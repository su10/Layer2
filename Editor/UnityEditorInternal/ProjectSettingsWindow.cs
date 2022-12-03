#if UNITY_EDITOR
using UnityEditor;
using InternalProjectSettingsWindow = UnityEditor.ProjectSettingsWindow;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class ProjectSettingsWindow
    {
        public static void Repaint()
        {
            var hasOpenInstances = EditorWindow.HasOpenInstances<InternalProjectSettingsWindow>();
            if (hasOpenInstances == false) return;

            var window = EditorWindow.GetWindowDontShow<InternalProjectSettingsWindow>();
            if (window.IsSelectedTab()) window.Repaint();
        }
    }
}
#endif
