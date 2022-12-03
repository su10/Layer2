#if UNITY_EDITOR
namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class InspectorWindow
    {
        public static void RepaintAllInspectors()
        {
            UnityEditor.InspectorWindow.RepaintAllInspectors();
        }
    }
}
#endif
