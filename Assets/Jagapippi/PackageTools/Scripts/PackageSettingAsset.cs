#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.PackageTools
{
    [CreateAssetMenu(menuName = "Jagapippi/Package Setting")]
    internal class PackageSettingAsset : ScriptableObject
    {
        [SerializeField] private TextAsset _packageJsonAsset;
        [field: SerializeField] public PackageTarget target { get; private set; }

        public Version GetVersion()
        {
            var versionString = JsonUtility.FromJson<PackageJson>(_packageJsonAsset.text).version;
            return new Version(versionString);
        }

        [Serializable]
        private struct PackageJson
        {
            public string version;
        }

        [Serializable]
        public struct PackageTarget
        {
            [SerializeField] internal DefaultAsset _directory;

            public string GetPath() => AssetDatabase.GetAssetPath(_directory);

            public string GetFileNameWithoutExtension()
            {
                var path = this.GetPath();
                return Path.GetFileNameWithoutExtension(path);
            }
        }
    }
}
#endif
