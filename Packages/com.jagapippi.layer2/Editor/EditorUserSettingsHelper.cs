#if UNITY_EDITOR
using System.Globalization;
using UnityEditor;

namespace Jagapippi.Layer2.Editor
{
    internal static class EditorUserSettingsHelper
    {
        public static string GetConfigValue(string name, string defaultValue)
        {
            var value = EditorUserSettings.GetConfigValue(name);
            return (string.IsNullOrEmpty(value) == false) ? value : defaultValue;
        }

        public static void SetConfigValue(string name, string value)
        {
            EditorUserSettings.SetConfigValue(name, value);
        }

        public static bool GetConfigValue(string name, bool defaultValue)
        {
            var value = EditorUserSettings.GetConfigValue(name);
            return (string.IsNullOrEmpty(value) == false) ? bool.Parse(value) : defaultValue;
        }

        public static void SetConfigValue(string name, bool value)
        {
            EditorUserSettings.SetConfigValue(name, value.ToString());
        }

        public static int GetConfigValue(string name, int defaultValue)
        {
            var value = EditorUserSettings.GetConfigValue(name);
            return (string.IsNullOrEmpty(value) == false) ? int.Parse(value) : defaultValue;
        }

        public static void SetConfigValue(string name, int value)
        {
            EditorUserSettings.SetConfigValue(name, value.ToString());
        }

        public static float GetConfigValue(string name, float defaultValue)
        {
            var value = EditorUserSettings.GetConfigValue(name);
            return (string.IsNullOrEmpty(value) == false) ? float.Parse(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        public static void SetConfigValue(string name, float value)
        {
            EditorUserSettings.SetConfigValue(name, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
#endif
