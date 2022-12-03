#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using InternalScriptAttributeUtility = UnityEditor.ScriptAttributeUtility;

namespace Jagapippi.Layer2.Editor.UnityEditorInternal
{
    public static class ScriptAttributeUtility
    {
        public static FieldInfo GetFieldInfoAndStaticTypeFromProperty(SerializedProperty property, out Type type)
        {
            return InternalScriptAttributeUtility.GetFieldInfoAndStaticTypeFromProperty(property, out type);
        }

        public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            return InternalScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
        }
    }
}
#endif
