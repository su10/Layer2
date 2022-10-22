using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jagapippi.Layer2
{
    public interface ILayerSettings
    {
        string LayerToName(int layer);
        int NameToLayer(string name);
        bool GetCollision(int layer1, int layer2);
#if UNITY_EDITOR
        event Action<ILayerSettings> changed;
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

        internal static void GetNamesWithIndexNonAlloc(this ILayerSettings settings, IList<string> list)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var layerName = settings.LayerToName(i);

                if (string.IsNullOrEmpty(layerName) || string.IsNullOrWhiteSpace(layerName))
                {
                    list[i] = "";
                }
                else
                {
                    // SEE: https://qiita.com/su10/items/ab33adefda8c2f7423e3
                    list[i] = $"{i}: {layerName}".Replace(" ", "\u00A0");
                }
            }
        }

        public static void ApplyCollisionMatrix(this ILayerSettings settings)
        {
            for (var i = 0; i < Layer.MaxCount; i++)
            {
                for (var j = 0; j < Layer.MaxCount - i; j++)
                {
                    Physics.IgnoreLayerCollision(i, j, settings.GetIgnoreCollision(i, j));
                }
            }
        }
    }
}
