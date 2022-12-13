#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;

namespace Jagapippi.PackageTools
{
    public static class PackageExporter
    {
        private static PackageSettingAsset FindSetting()
        {
            var guid = AssetDatabase.FindAssets($"t: {nameof(PackageSettingAsset)}").First();
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<PackageSettingAsset>(path);
        }

        [MenuItem("Tools/PackageExporter/Export All")]
        public static void Export()
        {
            var setting = FindSetting();
            var path = setting.target.GetPath();
            var samplePath = $"{path}/Samples";
            var excludeSamplePath = $"{samplePath}~";

            void IncludeSamples()
            {
                if (Directory.Exists(samplePath))
                {
                    Directory.Delete(samplePath);
                }

                Directory.Move(excludeSamplePath, samplePath);

                AssetDatabase.Refresh();
            }

            void ExcludeSamples()
            {
                Directory.Move(samplePath, excludeSamplePath);
                Directory.CreateDirectory(samplePath);

                AssetDatabase.Refresh();
            }

            IncludeSamples();
            try
            {
                AssetDatabase.ExportPackage(
                    setting.target.GetPath(),
                    $"{setting.target.GetFileNameWithoutExtension()}.{setting.GetVersion()}.unitypackage",
                    ExportPackageOptions.Recurse
                );
            }
            finally
            {
                ExcludeSamples();
            }
        }
    }
}
#endif
