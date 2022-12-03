using System;
using System.Collections.Generic;

namespace Jagapippi.Layer2
{
    public interface ILayerSetting
    {
        string name { get; }
        string LayerToName(int layer);
        int NameToLayer(string name);
        bool GetCollision(int layer1, int layer2);
        bool GetCollision2D(int layer1, int layer2);
#if UNITY_EDITOR
        event Action<ILayerSetting> changedSerializedObject;
#endif
    }

    public static class ILayerSettingExtensions
    {
        public static bool GetIgnoreCollision(this ILayerSetting setting, int layer1, int layer2)
        {
            return (setting.GetCollision(layer1, layer2) == false);
        }

        public static bool GetIgnoreCollision2D(this ILayerSetting setting, int layer1, int layer2)
        {
            return (setting.GetCollision2D(layer1, layer2) == false);
        }

        public static string[] GetNames(this ILayerSetting setting)
        {
            var names = new string[Layer.MaxCount];

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                names[i] = setting.LayerToName(i);
            }

            return names;
        }

        public static void GetNamesNonAlloc(this ILayerSetting setting, IList<string> list)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                list[i] = setting.LayerToName(i);
            }
        }
    }
}
