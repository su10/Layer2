#if UNITY_EDITOR
using UnityEditor;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class ObjectSelector
    {
        public static bool HasOpenInstances()
        {
            return EditorWindow.HasOpenInstances<UnityEditor.ObjectSelector>();
        }
    }
}
#endif
