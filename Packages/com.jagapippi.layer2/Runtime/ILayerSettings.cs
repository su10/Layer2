using System;
using System.Collections.Generic;

namespace Jagapippi.Layer2
{
    public interface ILayerSettings
    {
        string name { get; }
        string LayerToName(int layer);
        int NameToLayer(string name);
        bool GetCollision(int layer1, int layer2);
#if UNITY_EDITOR
        event Action<ILayerSettings> changedSerializedObject;
#endif
    }

    public static class ILayerSettingsExtensions
    {
        public static bool GetIgnoreCollision(this ILayerSettings settings, int layer1, int layer2)
        {
            return (settings.GetCollision(layer1, layer2) == false);
        }

        public static string[] GetNames(this ILayerSettings settings)
        {
            var names = new string[Layer.MaxCount];

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                names[i] = settings.LayerToName(i);
            }

            return names;
        }

        public static void GetNamesNonAlloc(this ILayerSettings settings, IList<string> list)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                list[i] = settings.LayerToName(i);
            }
        }
    }
}
